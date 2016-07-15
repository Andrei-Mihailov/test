using System;
using System.Threading;
using System.Collections.Generic;


namespace ConsoleApplication1
{
    public class Program
    {
        static Queue<byte[]> queue = new Queue<byte[]>();
        static object locker = new object();
        static int[,] myArr = new int[1,6];
        static byte[] reqFlag_ProductExist = { 11, 03, 00, 00, 00, 01 };
        static int i=0, Mode = 0;
        static void Main()
        {
            bool Flag_ProductExist = false;

            
            while (true)
            {
                
                if (Mode == 0)
                {
                    if (Flag_ProductExist)//появился товар
                    {
                        Mode = 1;
                        //Flag_ProductExist = false;
                    }
                    else
                    {
                        Console.WriteLine("T");//опрос контроллера на наличие товара
                        if(Console.ReadLine() == "1")
                        {
                            Flag_ProductExist = true;
                        }
                    }
                }
                else if (Mode == 1)
                {
                    Thread reqContr = new Thread(TransmitMassageToController);
                    reqContr.Name = "reqContr";
                    reqContr.IsBackground = true;
                    reqContr.Start();
                    Mode = 0;
                }
            }
        }
        static void TransmitMassageToController()
        {
            lock (locker)
            {
                Thread.Sleep(1000);//N ms
                Console.WriteLine("ProductList");
                arr();
                


            }
        }
        static void arr()
        {
          
            string strToSend = "";
            int index = 5;
            long startProcessingTime = 30;

            for (int c = 0; c < 1; c++)
            {
                //strToSend = (index + i).ToString() + ',' + startProcessingTime.ToString();
                byte[] CommandSortingError = { Convert.ToByte(i), 03, 00, 00, 00, 01 };
                queue.Enqueue(CommandSortingError);
                for (int j = 0; j < CommandSortingError.Length; j++)
                {
                    myArr[0, j] = CommandSortingError[j];
                }

             }
            while (queue.Count != 0)
            {
                string str = queue.Dequeue().ToString();//читаем и удаляем первый элемент очереди
                Console.WriteLine("{0} {1} {2} {3} {4} {5}", myArr[0, 0], myArr[0, 1], myArr[0, 2], myArr[0, 3], myArr[0, 4], myArr[0, 5]);
            }
            i++;
        }
        
    }
}
