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

        static void Main(string[] args)
        {
            uint count = 0;
            while (true)
            {
                uint CompanyProductID = 22841809 + count;
                serialPort_Printer = new SerialPort();
                serialPort_Printer.PortName = "COM1";
                serialPort_Printer.BaudRate = 115200;
                serialPort_Printer.DataBits = 8;
                serialPort_Printer.StopBits = StopBits.One;
                serialPort_Printer.Parity = Parity.None;
                serialPort_Printer.Open();
                serialPort_Printer.Write(Write_str(CompanyProductID));
                serialPort_Printer.Close();
                Thread.Sleep(1000);
                count++;
            }
        }

        static string Write_str (uint ID)
        {
            string esc = "\x1B";//управляющая команда
            string NewLine = "\x0D";//перенос на новую строку
            string Size_Lable = esc + "A1406609";
            string StartString = esc + "A";// начало ZPL - кода
            string DireNewLineionPrint = esc + "%" + "1";// 1 - нормальное, 2 - поворт на 90 градусов влево, 3 - поворот на 180 градусов, 4 - поворот на 270 градусов
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
            product_ZPL += StartString + esc + "E10"+ Size_Lable + DireNewLineionPrint + Horisontal_Space_Text + Vertical_Space + esc + "PS";
            if (!string.IsNullOrWhiteSpace(SortingIndex))
            {
                product_ZPL += esc + "XS" /*тип шрифта*/ + SortingIndex + Arrow + NewLine + SortingIndex1 + Arrow + NewLine + SortingIndex2 +
                    esc + "H" + (strt_hor_text + 3*fac).ToString() + Vertical_Space + esc + "XM" + Name + Model;// Текст: "unit.SortingIndex"
            }
            else
            {
                product_ZPL += "" + NewLine;
            }
            product_ZPL += Vertical_Space + Horisontal_Space_Bar_Code1 + esc + "B103080" + ID.ToString() + NewLine + //бар-код "CompanyProductID" "B103100*12345*"
                esc + "XB0" /*тип шрифта*/ + ID.ToString();//Текст: CompanyProductID;
            product_ZPL += Vertical_Space + Horisontal_Space_Bar_Code2 + esc + "BC02080" + SN + NewLine +//бар-код "SN"
                esc + "WB0" /*тип шрифта*/ + SN +
                 esc + "H0390" + Vertical_Space + esc + "XS" + index;//Текст: CompanyProductID;
            product_ZPL += Quantity + EndString; //количество этикеток, отправка на печать
            return product_ZPL;
        }
    }
}
