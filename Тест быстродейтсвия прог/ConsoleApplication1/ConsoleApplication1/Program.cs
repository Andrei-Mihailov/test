using System;
using System.Threading;
using System.Collections.Generic;


namespace ConsoleApplication1
{
    public class Program
    {
        static Queue<string> queue = new Queue<string>();
        static object locker = new object();
        static int[,] myArr = new int[1,6];
        static byte[] reqFlag_ProductExist = { 11, 03, 00, 00, 00, 01 };
        static void Main()
        {
            bool Flag_ProductExist = false;
            
            int Mode = 0;
            while (true)
            {
                for (int i=0; i< reqFlag_ProductExist.Length; i++)
                {
                    myArr[0, i] = reqFlag_ProductExist[i];
                }
                if (Mode == 0)
                {
                    if (Flag_ProductExist)//появился товар
                    {
                        Mode = 1;
                    }
                    else
                    {
                        Console.WriteLine("T");//опрос контроллера на наличие товара
                        if(Console.ReadLine() == "T1")
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

            for (int i = 0; i < 5; i++)
            {
                strToSend = (index + i).ToString() + ',' + startProcessingTime.ToString();
                //queue.Enqueue(strToSend);
                //byte[] CommandSortingError = { 11, 03, 00, 00, 00, 01 };
                queue.Enqueue(strToSend);
            }
            while(queue.Count!=0)
            {
                string str = queue.Dequeue().ToString();//читаем и удаляем первый элемент очереди
                Console.WriteLine("{0} {1} {2} {3} {4} {5}", myArr[0, 0], myArr[0,1], myArr[0, 2], myArr[0, 3], myArr[0, 4], myArr[0, 5]);
            }
        }
    }
}
