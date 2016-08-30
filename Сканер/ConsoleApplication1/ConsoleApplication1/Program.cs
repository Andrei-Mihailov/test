using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Timers;
namespace ConsoleApplication1
{
    class Program
    {
        static bool result = false;

        private static Timer aTimer;
        static int count = 0, i = 0;
        static string tmp_str = "";
        static public SerialPort serialPort_Scaner = null;
        static public Queue<string> queue = new Queue<string>();
        public event EventHandler<NewBarcodeEventArgs> NewBarcodeEvent;
       
        public class NewBarcodeEventArgs : EventArgs
        {
            public string Barcode { get; set; }

        }

        static void Main(string[] args)
        {
            serialPort_Scaner = new SerialPort();
            serialPort_Scaner.PortName = "COM12";
            serialPort_Scaner.BaudRate = 9600;
            serialPort_Scaner.DataBits = 8;
            serialPort_Scaner.StopBits = StopBits.One;
            serialPort_Scaner.Parity = Parity.None;
            serialPort_Scaner.DataReceived += new SerialDataReceivedEventHandler(serialPortScaner_DataReceived);
            serialPort_Scaner.Open();
            
            Console.ReadLine();
        }

        static private string ByteArrayToHexString(byte data)
        {
            string sb = " ";
            sb = Convert.ToString(data, 16);
            return sb.ToString().ToUpper();
        }
        static int timer_started = 0;
        static int buffer_recieved = 0;
        static public void serialPortScaner_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int to_read_data = serialPort_Scaner.BytesToRead;
                byte[] ScanBuffer = new byte[to_read_data];
                serialPort_Scaner.Read(ScanBuffer, 0, to_read_data);
                buffer_recieved += to_read_data;

                for (int i_tmp = 0; i_tmp < to_read_data; i_tmp++)
                {
                    string a = ByteArrayToHexString(ScanBuffer[i_tmp]);
                    string a_to_tmp_str = " ";
                    if ((result = a.Equals("00")) == true)
                        a_to_tmp_str = " ";
                    if ((result = a.Equals("20")) == true)
                        a_to_tmp_str = " ";
                    if ((result = a.Equals("21")) == true)
                        a_to_tmp_str = "!";
                    if ((result = a.Equals("23")) == true)
                        a_to_tmp_str = "#";
                    if ((result = a.Equals("24")) == true)
                        a_to_tmp_str = "$";
                    if ((result = a.Equals("25")) == true)
                        a_to_tmp_str = "%";
                    if ((result = a.Equals("26")) == true)
                        a_to_tmp_str = "&";
                    if ((result = a.Equals("27")) == true)
                        a_to_tmp_str = "'";
                    if ((result = a.Equals("28")) == true)
                        a_to_tmp_str = "(";
                    if ((result = a.Equals("29")) == true)
                        a_to_tmp_str = ")";
                    if ((result = a.Equals("2A")) == true)
                        a_to_tmp_str = "*";
                    if ((result = a.Equals("2B")) == true)
                        a_to_tmp_str = "+";
                    if ((result = a.Equals("2C")) == true)
                        a_to_tmp_str = ",";
                    if ((result = a.Equals("2D")) == true)
                        a_to_tmp_str = "-";
                    if ((result = a.Equals("2E")) == true)
                        a_to_tmp_str = ".";
                    if ((result = a.Equals("30")) == true)
                        a_to_tmp_str = "0";
                    if ((result = a.Equals("31")) == true)
                        a_to_tmp_str = "1";
                    if ((result = a.Equals("32")) == true)
                        a_to_tmp_str = "2";
                    if ((result = a.Equals("33")) == true)
                        a_to_tmp_str = "3";
                    if ((result = a.Equals("34")) == true)
                        a_to_tmp_str = "4";
                    if ((result = a.Equals("35")) == true)
                        a_to_tmp_str = "5";
                    if ((result = a.Equals("36")) == true)
                        a_to_tmp_str = "6";
                    if ((result = a.Equals("37")) == true)
                        a_to_tmp_str = "7";
                    if ((result = a.Equals("38")) == true)
                        a_to_tmp_str = "8";
                    if ((result = a.Equals("39")) == true)
                        a_to_tmp_str = "9";
                    if ((result = a.Equals("3A")) == true)
                        a_to_tmp_str = ":";
                    if ((result = a.Equals("3B")) == true)
                        a_to_tmp_str = ";";
                    if ((result = a.Equals("3C")) == true)
                        a_to_tmp_str = "<";
                    if ((result = a.Equals("3D")) == true)
                        a_to_tmp_str = "=";
                    if ((result = a.Equals("3E")) == true)
                        a_to_tmp_str = ">";
                    if ((result = a.Equals("3F")) == true)
                        a_to_tmp_str = "?";
                    if ((result = a.Equals("40")) == true)
                        a_to_tmp_str = "@";
                    if ((result = a.Equals("41")) == true)
                        a_to_tmp_str = "A";
                    if ((result = a.Equals("42")) == true)
                        a_to_tmp_str = "B";
                    if ((result = a.Equals("43")) == true)
                        a_to_tmp_str = "C";
                    if ((result = a.Equals("44")) == true)
                        a_to_tmp_str = "D";
                    if ((result = a.Equals("45")) == true)
                        a_to_tmp_str = "E";
                    if ((result = a.Equals("46")) == true)
                        a_to_tmp_str = "F";
                    if ((result = a.Equals("47")) == true)
                        a_to_tmp_str = "G";
                    if ((result = a.Equals("48")) == true)
                        a_to_tmp_str = "H";
                    if ((result = a.Equals("49")) == true)
                        a_to_tmp_str = "I";
                    if ((result = a.Equals("4A")) == true)
                        a_to_tmp_str = "J";
                    if ((result = a.Equals("4B")) == true)
                        a_to_tmp_str = "K";
                    if ((result = a.Equals("4C")) == true)
                        a_to_tmp_str = "L";
                    if ((result = a.Equals("4D")) == true)
                        a_to_tmp_str = "M";
                    if ((result = a.Equals("4E")) == true)
                        a_to_tmp_str = "N";
                    if ((result = a.Equals("4F")) == true)
                        a_to_tmp_str = "O";
                    if ((result = a.Equals("50")) == true)
                        a_to_tmp_str = "P";
                    if ((result = a.Equals("51")) == true)
                        a_to_tmp_str = "Q";
                    if ((result = a.Equals("52")) == true)
                        a_to_tmp_str = "R";
                    if ((result = a.Equals("53")) == true)
                        a_to_tmp_str = "S";
                    if ((result = a.Equals("54")) == true)
                        a_to_tmp_str = "T";
                    if ((result = a.Equals("55")) == true)
                        a_to_tmp_str = "U";
                    if ((result = a.Equals("56")) == true)
                        a_to_tmp_str = "V";
                    if ((result = a.Equals("57")) == true)
                        a_to_tmp_str = "W";
                    if ((result = a.Equals("58")) == true)
                        a_to_tmp_str = "X";
                    if ((result = a.Equals("59")) == true)
                        a_to_tmp_str = "Y";
                    if ((result = a.Equals("5A")) == true)
                        a_to_tmp_str = "Z";
                    if ((result = a.Equals("5B")) == true)
                        a_to_tmp_str = "[";
                    if ((result = a.Equals("5D")) == true)
                        a_to_tmp_str = "]";
                    if ((result = a.Equals("5E")) == true)
                        a_to_tmp_str = "^";
                    if ((result = a.Equals("5F")) == true)
                        a_to_tmp_str = "_";
                    if ((result = a.Equals("60")) == true)
                        a_to_tmp_str = "'";
                    if ((result = a.Equals("61")) == true)
                        a_to_tmp_str = "a";
                    if ((result = a.Equals("62")) == true)
                        a_to_tmp_str = "b";
                    if ((result = a.Equals("63")) == true)
                        a_to_tmp_str = "c";
                    if ((result = a.Equals("64")) == true)
                        a_to_tmp_str = "d";
                    if ((result = a.Equals("65")) == true)
                        a_to_tmp_str = "e";
                    if ((result = a.Equals("66")) == true)
                        a_to_tmp_str = "f";
                    if ((result = a.Equals("67")) == true)
                        a_to_tmp_str = "g";
                    if ((result = a.Equals("68")) == true)
                        a_to_tmp_str = "h";
                    if ((result = a.Equals("69")) == true)
                        a_to_tmp_str = "i";
                    if ((result = a.Equals("6A")) == true)
                        a_to_tmp_str = "j";
                    if ((result = a.Equals("6B")) == true)
                        a_to_tmp_str = "k";
                    if ((result = a.Equals("6C")) == true)
                        a_to_tmp_str = "l";
                    if ((result = a.Equals("6D")) == true)
                        a_to_tmp_str = "m";
                    if ((result = a.Equals("6E")) == true)
                        a_to_tmp_str = "n";
                    if ((result = a.Equals("6F")) == true)
                        a_to_tmp_str = "o";
                    if ((result = a.Equals("70")) == true)
                        a_to_tmp_str = "p";
                    if ((result = a.Equals("71")) == true)
                        a_to_tmp_str = "q";
                    if ((result = a.Equals("72")) == true)
                        a_to_tmp_str = "r";
                    if ((result = a.Equals("73")) == true)
                        a_to_tmp_str = "s";
                    if ((result = a.Equals("74")) == true)
                        a_to_tmp_str = "t";
                    if ((result = a.Equals("75")) == true)
                        a_to_tmp_str = "u";
                    if ((result = a.Equals("76")) == true)
                        a_to_tmp_str = "v";
                    if ((result = a.Equals("77")) == true)
                        a_to_tmp_str = "w";
                    if ((result = a.Equals("78")) == true)
                        a_to_tmp_str = "x";
                    if ((result = a.Equals("79")) == true)
                        a_to_tmp_str = "y";
                    if ((result = a.Equals("7A")) == true)
                        a_to_tmp_str = "z";
                    if ((result = a.Equals("7B")) == true)
                        a_to_tmp_str = "{";
                    if ((result = a.Equals("7C")) == true)
                        a_to_tmp_str = "|";
                    if ((result = a.Equals("7D")) == true)
                        a_to_tmp_str = "}";
                    if ((result = a.Equals("7E")) == true)
                        a_to_tmp_str = "~";
                    tmp_str += a_to_tmp_str;
                }
                if (timer_started == 0)
                {
                    aTimer = new Timer(50);//1 sec
                    aTimer.Elapsed += OnTimedEvent;
                    aTimer.AutoReset = false;
                    aTimer.Enabled = true;
                    timer_started = 1;
                }
            }
            catch (Exception e1)
            {
                //ErrorInf.Add("Ошибка при приеме информации по COM-порту! " + e1.Message.ToString());
            }
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (buffer_recieved > 5)
            {
                OnNewBarcodeEvent(tmp_str, buffer_recieved);
            }
                tmp_str = "";
                aTimer.Enabled = false;
                timer_started = 0;
                buffer_recieved = 0;
            
        }
        static public void OnNewBarcodeEvent(string NewBarcode, int data)
        {
            serialPort_Scaner.Close();
            string str = "";
            int Lenth_str = 0;
            if ((NewBarcode.IndexOf(" ", 0) != -1)&(data > 5))
            {
                str = NewBarcode.Substring(0, NewBarcode.IndexOf(" ", 0));
                Lenth_str = str.Length;
            }
            
            queue.Enqueue(NewBarcode);
            Console.WriteLine(queue.Dequeue());
            
           // Console.WriteLine("Lenth is {0}", Lenth_str);
            Console.WriteLine("Lenth is {0}", data.ToString());

            serialPort_Scaner.Open();

        }
        
    }
}
