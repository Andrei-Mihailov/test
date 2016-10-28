using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.StepMotor;
using Photo3D.StepMotor;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Photo3D;
using System.Threading.Tasks;
using System.Diagnostics;

namespace StepMotor
{
    public class WiFiServer_V2 : IDisposable
    {
        IPAddress _IP = null;
        int _port = 0;
        public event EventHandler EventChanged;
         IPEndPoint _ipEndPoint = null;

        public String msg = "";
        DateTime _lastConnectionTime = DateTime.MinValue;

        TcpListener server;
        TcpClient client;
        byte[] RxBuffer;

        public event EventHandler DataReceived;
        public event EventHandler Disposed;

        public bool CheckDeviceConnected()
        {
            var span = DateTime.Now - _lastConnectionTime;
            if (span < new TimeSpan(0, 0, 0, 40, 0))
                return true;
            else
                return false;
        }
        private static object _staticSyncRoot = new object();
        public static WiFiServer_V2 GetInstance(string IP, int port)
        {
            lock (_staticSyncRoot)
            {
                if (_instance == null || _instance._disposing)
                    _instance = new WiFiServer_V2(IP, port);

                return _instance;
            }
        }

        private static WiFiServer_V2 _instance = null;


        private WiFiServer_V2(string IP, int port)
        {
            _IP = IPAddress.Parse(IP);
            _port = port;
            _ipEndPoint = new IPEndPoint(_IP, _port);
            server = new TcpListener(_ipEndPoint.Address, _ipEndPoint.Port);
            server.Start();
            server.BeginAcceptTcpClient(onCompleteAcceptTcpClient, server);
        }

        void onCompleteAcceptTcpClient(IAsyncResult iar)
        {
            TcpListener tcplistener = (TcpListener)iar.AsyncState;
            try
            {
                client = tcplistener.EndAcceptTcpClient(iar);
                tcplistener.BeginAcceptTcpClient(onCompleteAcceptTcpClient, tcplistener);
                RxBuffer = new byte[10240];
                client.GetStream().BeginRead(RxBuffer, 0, RxBuffer.Length, onCompleteReadFromTCPClientStream, client); //async method
            }
            catch (Exception e)
            {
                msg = "Exception: " + e.Message + "\r\n";
                if (EventChanged != null)
                    EventChanged(this, new EventArgs());
            }
        }
        void onCompleteReadFromTCPClientStream(IAsyncResult iar)
        {
            TcpClient tcpclient;
            int nCountReadBytes = 0;
            string strReceived;

            try
            {
                tcpclient = (TcpClient)iar.AsyncState;
                nCountReadBytes = tcpclient.GetStream().EndRead(iar);
                if (nCountReadBytes == 0)
                {
                    msg = "Client disconnected" + "\r\n";
                    if (EventChanged != null)
                        EventChanged(this, new EventArgs());
                    return;
                }
                strReceived = Encoding.ASCII.GetString(RxBuffer, 0, nCountReadBytes);

                msg = "Received: " + strReceived + "\r\n";
                if (EventChanged != null)
                    EventChanged(this, new EventArgs());
                RxBuffer = new byte[512];
                tcpclient.GetStream().BeginRead(RxBuffer, 0, RxBuffer.Length, onCompleteReadFromTCPClientStream, tcpclient);
            }
            catch (Exception e)
            {
                msg = "Exception: " + e.Message + "\r\n";
                if (EventChanged != null)
                    EventChanged(this, new EventArgs());
            }
        }
        public void SendCommand(string cmd)
        {
            byte[] TxBuffer = new byte[10240];
            try
            {
                if (client != null)
                {
                    if (client.Client.Connected)
                    {
                        TxBuffer = Encoding.ASCII.GetBytes(cmd);
                        client.GetStream().BeginWrite(TxBuffer, 0, TxBuffer.Length, onCompleteWriteToClientStream, client);
                        msg = "Send: " + cmd + "\r\n";
                        if (EventChanged != null)
                            EventChanged(this, new EventArgs());
                    }
                }
            }
            catch (Exception e)
            {
                msg = "Exception: " + e.Message + "\r\n";
                if (EventChanged != null)
                    EventChanged(this, new EventArgs());
            }
        }
        private void onCompleteWriteToClientStream(IAsyncResult iar)
        {
            try
            {
                TcpClient tcpclient = (TcpClient)iar.AsyncState;
                tcpclient.GetStream().EndWrite(iar);
            }
            catch (Exception e)
            {
                msg = "Exception: " + e.Message + "\r\n";
                if (EventChanged != null)
                    EventChanged(this, new EventArgs());
            }
        }

        private bool _disposing = false;
        private string _guid = new Guid().ToString();
        public string Guid { get { return _guid; } }

        [DebuggerStepThrough]
        public void Dispose()
        {
            try
            {
                if (server != null)
                    server.Stop();
                if (client != null)
                    client.Close();
                if (_instance == this)
                    _instance = null;
            }
            catch { }
            _instance = null;
        }
    }

    public class WifiStepMotorAPI_V2 : BaseStepMotorAPI, IDisposable, IStepMotorAPI, ISerialPortAPI
    {
        
        internal class WiFiCommandRunner : CommandRunnerBase
        {
            public WiFiCommandRunner(ISerialPortAPI api)
                : base(api)
            { }

            protected override MotorPortResponse ConvertResponse(string command, string response)
            {
                if (response.Contains("#DIE#"))
                    return MotorPortResponse.VRFail;

                string lastCommand = "#" + command.Split('#').Last();
                var res = GetResponseCodes().Where(c => response.Contains(lastCommand + c)).SingleOrDefault();
                return res;
            }
        }
        public event EventHandler EventChanged;
        int version = 1;
        MotorPortStatus _lastPortStatus = MotorPortStatus.Disconnected;
        object _syncRoot = new object();

        public string msg = "";

        private WiFiServer_V2 _server = null;
        protected string _IP;
        protected int _port;
        protected DateTime _initializationTime = DateTime.Now;
        public DateTime InitializationTime { get { return _initializationTime; } }

        public MotorPortStatus Connect()
        {
            _lastPortStatus = MotorPortStatus.Disconnected;
            _stepsPerRoundPredefined = 0;

            MotorPortResponse cres = null;
            lock (_syncRoot)
            {
                bool locked = LockPort(false);

                var commandRunner = new WiFiCommandRunner(this);
                _waitPortReady = false;
                cres = commandRunner.Run("#n1.");
                _waitPortReady = true;

                UnlockPort(locked, false);
            }


            if (cres == MotorPortResponse.VRSuccess)
            {
                _lastPortStatus = MotorPortStatus.Ready;

                var initializationData = cres.Message;
                string[] ssData = initializationData.Replace("\r", "").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                string sData = ssData.Where(s => s.Split(':').Length == 2 && s.Split(':')[0] == "StepsPerRound").FirstOrDefault();
                if (sData != null)
                {
                    _stepsPerRoundPredefined = int.Parse(sData.Split(':')[1]);
                }
            }

            var res = GetPortStatus();

            OnPortStatusChanged(res);

            return res;
        }

        public WifiStepMotorAPI_V2(string IP, int port)
        {
            _IP = IP;
            _port = port;

            InitializePortControlTimer();
            SetResponseCodes(new MotorPortResponse[] {
				MotorPortResponse.VRSuccess,
				MotorPortResponse.VRFail
			});
        }

        public Task RotateAsync(RotationParameters parameters)
        {
            return Task.Factory.StartNew(() => Rotate(parameters));
        }

        public bool CheckDeviceConnected()
        {
            if (_server != null)
            {
                bool res = _server.CheckDeviceConnected();
                if (!res)
                    _lastPortStatus = MotorPortStatus.Disconnected;

                return res;
            }
            else
            {
                _lastPortStatus = MotorPortStatus.Disconnected;
                return false;
            }
        }
        public void InfinityRotate(RotationParameters parameters)
        {
            bool locked = false;
            try
            {
                locked = LockPort();

                string command = "#e1.";

                if (parameters.UseZeroSensor)
                {
                    command += "#z1.";
                    command += "#u" + parameters.ZeroSensorCancellingInterval.ToString() + ".";
                }
                else
                {
                    command += "#z0.";
                }

                command += "#s" + (parameters.RotationSpeed == 0 ? 10 : parameters.RotationSpeed) * 25 + ".";

                if (parameters.Acceleration > 0)
                    command += "#a" + parameters.Acceleration * 10 + ".";
                else
                    command += "#a" + (parameters.RotationSpeed == 0 ? 10 : parameters.RotationSpeed) * 10 + ".";

                command += (parameters.InfinityRotate) ? ("#r" + parameters.NSteps.ToString() + ".") : ("#c0.");

                if (parameters.RotationDelay != 0)
                    Thread.Sleep(parameters.RotationDelay * 1000);
                else
                    Thread.Sleep(20);

                int timeout =
                    10000 + // timeout for command
                    parameters.NSteps / (parameters.RotationSpeed == 0 ? 10 : parameters.RotationSpeed) * 45 + // timeout for rotation
                    parameters.NSteps * parameters.RotationSpeed / 5000; // timeout for acceleration


                _server.SendCommand(command);//.command.Enqueue(command);
                //var res = RunCommand(command, true, timeout);

                if (parameters.TurnOffEnginePower)
                    _server.SendCommand("#e0.");
                   // RunCommand("#e0.");
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            finally
            {
                UnlockPort(locked);
            }
        }
        public void Rotate(RotationParameters parameters)
        {
            bool locked = false;
            try
            {
                locked = LockPort();

                string command = "#e1.";

                if (parameters.UseZeroSensor)
                {
                    command += "#z1.";
                    command += "#u" + parameters.ZeroSensorCancellingInterval.ToString() + ".";
                }
                else
                {
                    command += "#z0.";
                }

                command += "#s" + (parameters.RotationSpeed == 0 ? 10 : parameters.RotationSpeed) * 25 + ".";

                if (parameters.Acceleration > 0)
                    command += "#a" + parameters.Acceleration * 10 + ".";
                else
                    command += "#a" + (parameters.RotationSpeed == 0 ? 10 : parameters.RotationSpeed) * 10 + ".";

                command += "#p" + parameters.NSteps + ".";

                if (parameters.RotationDelay != 0)
                    Thread.Sleep(parameters.RotationDelay * 1000);
                else
                    Thread.Sleep(20);

                int timeout =
                    10000 + // timeout for command
                    parameters.NSteps / (parameters.RotationSpeed == 0 ? 10 : parameters.RotationSpeed) * 45 + // timeout for rotation
                    parameters.NSteps * parameters.RotationSpeed / 5000; // timeout for acceleration

                //var res = RunCommand(command, true, timeout);
                //_server.command.Enqueue(command);
                _server.SendCommand(command);

                if (parameters.TurnOffEnginePower)
                    //RunCommand("#e0.");
                    _server.SendCommand("#e0.");
                //_server.command.Enqueue(command);

            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
            finally
            {
                UnlockPort(locked);
            }
        }

        public void TurnOffEngine()
        {
            try
            {
                bool locked = LockPort();
                _server.SendCommand("#e0.");
                //var res = RunCommand("#e0.");
                UnlockPort(locked);
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
        }

        public void TurnOnEngine()
        {
            try
            {
                bool locked = LockPort();
                _server.SendCommand("#e1.");
                //var res = RunCommand("#e1.");
                UnlockPort(locked);
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
        }


        protected override void ProcessError(Exception ex)
        {
            _lastPortStatus = MotorPortStatus.Disconnected;
            _isPortBusyInternal = false;
            _isPortBusy = false;
            OnPortStatusChanged(GetPortStatus());
        }

        public void CancelRotation()
        {
            try
            {
                bool locked = LockPort();
                _server.SendCommand("#c100.");
                //var res = RunCommand("#c100.");

                Thread.Sleep(20);

                UnlockPort(locked);
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
        }


        protected override MotorPortStatus GetPortStatusInternal()
        {
            if (_server == null)
                return MotorPortStatus.Disconnected;

            if (_isPortBusyInternal && _lastPortStatus == MotorPortStatus.Ready)
                return _lastPortStatus;

            if (_isPortBusy)
                return MotorPortStatus.Busy;

            if (!CheckDeviceConnected())
                return MotorPortStatus.Disconnected;

            return _lastPortStatus;
        }

        void WiFiServerEvent(object sender, EventArgs e)
        {
            msg = _server.msg;
            if (EventChanged != null)
                EventChanged(this, new EventArgs());
        }

        protected override void PortControlTimerHandler(object state)
        {
            //lock (_syncRoot)
            {
                if (_server == null)
                {
                    _server = WiFiServer_V2.GetInstance(_IP, _port);
                    _server.DataReceived += new EventHandler(OnPortDataReceived);
                    _server.EventChanged += new EventHandler(WiFiServerEvent);
                }

                if (_isPortBusyInternal)
                    return;

                if (_lastPortStatus == MotorPortStatus.Ready && !CheckDeviceConnected())
                {
                    try
                    {
                        var res = RunCommand("#q0.", false);
                        //_server.SendCommand("#c100.");
                        if (res != MotorPortResponse.VRSuccess)
                            throw new Photo3DMotorException("Cannot connect to WiFi device");
                    }
                    catch (Exception ex)
                    {
                        _lastPortStatus = MotorPortStatus.Disconnected;
                        OnPortStatusChanged(MotorPortStatus.Disconnected);
                    }
                }
                else if (_lastPortStatus == MotorPortStatus.Disconnected)
                {
                    OnPortStatusChanged(MotorPortStatus.Disconnected);
                }
            }
        }

        public MotorPortResponse RunCommand(string command, bool fireStatusEvents = true, int timeoutMiliseconds = 0)
        {
            lock (_syncRoot)
            {
                bool locked = LockPort(fireStatusEvents);

                WaitPortReady();

                var commandRunner = new WiFiCommandRunner(this) { TimeoutMilliseconds = timeoutMiliseconds };
                var res = commandRunner.Run(command);

                UnlockPort(locked, fireStatusEvents);

                if (res == MotorPortResponse.VRSuccess)
                    _lastPortStatus = MotorPortStatus.Ready;

                return res;
            }
        }


        public event EventHandler PortDataReceived;
        public event EventHandler Disposed;

        void OnPortDataReceived(object sender, EventArgs e)
        {
            if (PortDataReceived != null)
                PortDataReceived(this, new EventArgs());
        }

        public void WritePortData(string data)
        {
            /*lock (_syncRoot)
            {
                WaitPortReady();
                _server.WriteData(data);
            }*/
        }

        public void WritePortData(byte[] data)
        {
            /*lock (_syncRoot)
            {
                WaitPortReady();
                _server.WriteData(data, 0, data.Length);
            }*/
        }

        public string ReadPortData()
        {
            {
                return null;
                /*try
                {
                    return _server.ReadData();
                }
                catch (UnauthorizedAccessException ex)
                {
                    ProcessError(ex);
                    throw new Photo3DMotorException("Unable to connect to WiFi device", ex);
                }*/
            }
        }

        public void StartRotation(RotationParameters parameters)
        {
            return;
        }

        public void Dispose()
        {
            StopPortControlTimer();

            if (this.Disposed != null)
                Disposed(this, new EventArgs());

            if (_server != null)
                _server.Dispose();
        }
    }
}
