using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Timers;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static public SerialPort serialPort_Printer = null;
        public volatile static List<string> ErrorInf;
        public static bool result = false;
        public static string tmp_str = "";
        static void Main(string[] args)
        {
            uint count = 0;

            while (true)
            {
                uint CompanyProductID = 22841809 + count;
                string CAN = "\x12";
                string Print_Start = "\x11";
                string Clear_Error = "\x1A";
                string Get_Status = "\x05";
                serialPort_Printer = new SerialPort();
                serialPort_Printer.PortName = "COM1";
                serialPort_Printer.BaudRate = 115200;
                serialPort_Printer.DataBits = 8;
                serialPort_Printer.StopBits = StopBits.One;
                serialPort_Printer.Parity = Parity.None;
                serialPort_Printer.Open();
                serialPort_Printer.Write(Write_str(CompanyProductID));
                serialPort_Printer.Close();
                serialPort_Printer.DataReceived += new SerialDataReceivedEventHandler(serialPortPrinter_DataReceived);
                //while (true)
                //{
                //    if (!string.IsNullOrWhiteSpace(tmp_str))
                //    {
                //        Console.WriteLine(tmp_str);
                //    }
                //    Console.Read();
                //}
                Thread.Sleep(1000);
                count++;

            // чтоб очистить ошибку надо отправить Clear_Error()  и потом Get_Printer_Status(), чтоб подтвердить выполнение
            }
        }

        static string Write_str (uint ID)
        {
            string esc = "\x1B";//управляющая команда
            string NewLine = "\x0D";//перенос на новую строку
            string Size_Lable = esc + "A1406609";
            string StartString = esc + "A";// начало ZPL - кода
            string Line_distance = esc + "E" + "10";// расстояние между строками в точках
            string DirectNewLineOnPrint = esc + "%" + "1";// 1 - нормальное, 2 - поворт на 90 градусов влево, 3 - поворот на 180 градусов, 4 - поворот на 270 градусов
            string Vertical_Space = esc + "V" + "0500";//отступ по горизонтали 430
            uint strt_hor_text = 0060;
            string Horisontal_Space_Text = esc + "H" + strt_hor_text.ToString();//отступ по вертикали текста 80
            string Horisontal_Space_Bar_Code1 = esc + "H" + "0160";//отступ по вертикали бар-кода 1
            string Horisontal_Space_Bar_Code2 = esc + "H" + "0290";//отступ по вертикали бар-кода 1
            string Quantity = esc + "Q" + "1";// количество этикеток
            string EndString = esc + "Z";// конец ZPL - кода

            string product_ZPL = "";
            string SortingIndex = "Electronics";
            string Arrow = " -> ";
            string SortingIndex1 = "Headphones & Portable Speakers";
            string SortingIndex2 = "Portable Speakers";
            string Name = "Beats" + " ";
            string Model = "MH822AM/A";
            
            string SN = "S/N 0IEF30451Q6T";
            string index = "[2]";
            uint fac = 20;

            product_ZPL += StartString + Line_distance + Size_Lable + DirectNewLineOnPrint + Horisontal_Space_Text + Vertical_Space + esc + "PS";

            if (!string.IsNullOrWhiteSpace(SortingIndex))
            {
                product_ZPL += esc + "XS" /*тип шрифта*/ + SortingIndex + Arrow + NewLine + SortingIndex1 + Arrow + NewLine + SortingIndex2 +
                    esc + "H" + (strt_hor_text + 3*fac).ToString() + Vertical_Space + esc + "XM" + Name + Model;
            } else {
                product_ZPL += "" + NewLine;
            }
            product_ZPL += Vertical_Space + Horisontal_Space_Bar_Code1 + esc + "B103080" + ID.ToString() + NewLine + //бар-код "CompanyProductID"
                esc + "XB0" /*тип шрифта*/ + ID.ToString();//Текст: CompanyProductID;
            product_ZPL += Vertical_Space + Horisontal_Space_Bar_Code2 + esc + "BC02080" + SN + NewLine + //бар-код "SN"
                esc + "WB0" /*тип шрифта*/ + SN + //текст "SN"
                 esc + "H0390" + Vertical_Space + esc + "XS" + index; //текст "CompanyProductID";
            product_ZPL += Quantity + EndString;

            return product_ZPL;
        }

        static string Get_Printer_Status()
        {
            string DataStart = "\x02";
            string DataEnd = "\x03";
            string Get_Status = "\x05";
            //string Print_Status_Req = DataStart + "\x01{" + Get_Status + "}00000" + DataEnd;
            string Print_Status_Req = DataStart + Get_Status + DataEnd;
            return Print_Status_Req;
        }
      
        static private string ByteArrayToHexString(byte data)
        {
            string sb = " ";
            sb = Convert.ToString(data, 16);
            return sb.ToString().ToUpper();
        }
        static void serialPortPrinter_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] ScanBuffer = new byte[serialPort_Printer.BytesToRead];
                serialPort_Printer.Read(ScanBuffer, 0, serialPort_Printer.BytesToRead);

                for (int i_tmp = 0; i_tmp < serialPort_Printer.BytesToRead; i_tmp++)
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
            }
            catch (Exception e1)
            {
                ErrorInf.Add("Ошибка при приеме информации по COM-порту! " + e1.Message.ToString());
            }
        }
    }
}
