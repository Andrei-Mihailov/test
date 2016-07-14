using System;
using System.Threading;
using System.Collections.Generic;


namespace ConsoleApplication1
{
    public class Program
    {
        static Queue<string> queue = new Queue<string>();
        static object locker = new object();
        static void Main()
        {
            
            while (true)
             {
                Thread reqContr = new Thread(TransmitMassageToController);
                DateTime myDate = DateTime.Now;
                reqContr.Name = "reqContr";
                reqContr.IsBackground = true;
                reqContr.Start();
                Thread.Sleep(20);//N ms
                Console.WriteLine("Дата в формате d: {0:d}", myDate);
             }
        }
        static void TransmitMassageToController()
        {
            lock (locker)
            {
                Thread.Sleep(100);//N ms
                Console.WriteLine("ProductList");//"Время в формате T: {0:T}", myDate);
                arr();
            }
        }
        static void arr()
        {
            string strToSend = "";
            int index = 1;
            long startProcessingTime = 30;
           

            for (int i = 0; i < 5; i++)
            {
                strToSend = (index + i).ToString() + ',' + startProcessingTime.ToString();
                queue.Enqueue(strToSend);
            }
            while(queue.Count!=0)
            {
                string str = queue.Dequeue();//читаем и удаляем первый элемент очереди
                Console.WriteLine(str);
            }

        }
    }
}
