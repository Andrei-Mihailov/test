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
        static public SerialPort serialPort_Printer = null;

        static void Main(string[] args)
        {
            serialPort_Printer = new SerialPort();
            serialPort_Printer.PortName = "COM13";
            serialPort_Printer.BaudRate = 9600;
            serialPort_Printer.DataBits = 8;
            serialPort_Printer.StopBits = StopBits.One;
            serialPort_Printer.Parity = Parity.None;
            //serialPort_Printer.Open();
            //serialPort_Printer.Write(Write_str());
            //serialPort_Printer.Close();
            Console.WriteLine(Write_str());
            Console.ReadLine();
        }

        static string Write_str ()
        {
            string SortingIndex = "Hello World";
            string product_ZPL = "";
            uint CompanyProductID = 125496254;

            product_ZPL += "<ESC>A"/*начало строки*/ + "<ESC>H0025"/*отступ по горизонтали*/+ "<ESC>V0025" /*отступ по вертикали*/; //начало строки
            if (!string.IsNullOrWhiteSpace(SortingIndex))
            {
                product_ZPL += "<ESC>WB0" /*тип шрифта*/ + SortingIndex + ">CR>"/*перенос нановую строку*/;// текст "unit.SortingIndex"
            }
            else
            {
                product_ZPL += "" + ">CR>";
            }

            product_ZPL += "<ESC>D303150"/*параметры бар-кода*/+ "TRG-" + CompanyProductID + ">CR>";//бар-код "TRG-CompanyProductID"
            product_ZPL += "<ESC>WB0" /*тип шрифта*/ + "TRG-" + CompanyProductID + ">CR>";//Текст: "TRG-CompanyProductID"
            product_ZPL += "<ESC>Q1" + "<ESC>Z"; //конец строки, отправка на печать
            return product_ZPL;
        }
    }
}
