﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Threads
{
    /* class ThreadTest
     {
         //В главном потоке создается новый поток t, исполняющий метод, который непрерывно печатает символ ‘y’. 
         //Одновременно главный поток непрерывно печатает символ ‘x’.
         static void Main()
         {
             Thread t = new Thread(WriteY);
             t.Start();            // Выполнить WriteY в новом потоке
             while (true)
                 Console.Write("x"); // Все время печатать 'x'
         }

         static void WriteY()
         {
             while (true)
                 Console.Write("y"); // Все время печатать 'y'
         }

         /*
          // Отдельный экземпляр переменной cycles создается в стеке каждого потока, так что выводится, как и ожидалось, десять знаков ‘?’.
                 static void Main()
                 {
                     new Thread(Go).Start();      // Выполнить Go() в новом потоке
                     Go();                         // Выполнить Go() в главном потоке
                 }

                 static void Go()
                 {
                     // Определяем и используем локальную переменную 'cycles'
                     for (int cycles = 0; cycles < 5; cycles++)
                         Console.Write('?');
                 }


             */

    //  }
    /*
        class ThreadTest
        {
            bool done;

            static void Main()
            {
                ThreadTest tt = new ThreadTest(); // Создаем общий объект
                new Thread(tt.Go).Start();
                tt.Go();
            }

            // Go сейчас – экземплярный метод
            void Go()
            {
                if (!done) { done = true; Console.WriteLine("Done"); }
            }
        }
        //Так как оба потока вызывают метод Go() одного и того же экземпляра ThreadTest, 
        //они разделяют поле done. Результат – “Done”, напечатанное один раз вместо двух
        
     // Вот тот же самый пример, но со статическим полем done:

        class ThreadTest 
        {
          static bool done;    // Статическое поле, разделяемое потоками
  
          static void Main() 
          {
            new Thread(Go).Start();
            Go();
          }
  
          static void Go() 
          {
            if (!done) { done = true; Console.WriteLine("Done"); }
          }
        }

       // Оба примера демонстрируют также другое ключевое понятие – потоковую безопасность (или скорее её отсутствие). Фактически результат исполнения программы не определен: возможно (хотя и маловероятно), "Done" будет напечатано дважды. Однако если мы поменяем порядок вызовов в методе Go(), шансы увидеть “Done” напечатанным два раза повышаются радикально:

            static void Go() 
            {
              if (!done)
              {
                Console.WriteLine("Done");
                done = true;
              }
            }
     //Проблема состоит в том, что один поток может выполнить оператор if, пока другой поток выполняет WriteLine, т.е. до того как done будет установлено в true.
    //Лекарство состоит в получении эксклюзивной блокировки на время чтения и записи разделяемых полей. C# обеспечивает это при помощи оператора lock:
     
        class ThreadSafe 
        {
          static bool done;
          static object locker = new object();
 
          static void Main() 
          {
            new Thread(Go).Start();
            Go();
          }
 
          static void Go() 
          {
            lock (locker) 
            {
              if (!done)
              {
                Console.WriteLine("Done");
                done = true;
              }
            }
          }
        }
     
    //Когда два потока одновременно борются за блокировку (в нашем случае объекта locker), один поток переходит к ожиданию (блокируется), пока блокировка не освобождается. В данном случае это гарантирует, что только один поток может одновременно исполнять критическую секцию кода, и "Done" будет напечатано только один раз. Код, защищенный таким образом от неопределённости в плане многопоточного исполнения, называется потокобезопасным.

       //Временная приостановка (блокирование) – основной способ координации, или синхронизации действий потоков. Ожидание эксклюзивной блокировки – это одна из причин, по которым поток может блокироваться. Другая причина – если поток приостанавливается (Sleep) на заданный промежуток времени:

        Thread.Sleep(TimeSpan.FromSeconds(30)); // Блокировка на 30 секунд

    //Также поток может ожидать завершения другого потока, вызывая его метод Join:

        Thread t = new Thread(Go);     // Go – статический метод
        t.Start();
        t.Join();                       // Ожидаем завершения потока
    //Будучи блокированным, поток не потребляет ресурсов CPU.
     
     
     */



    //Для создания потоков используется конструктор класса Thread, принимающий в качестве параметра делегат типа ThreadStart, 
    //указывающий метод, который нужно выполнить.Делегат ThreadStart определяется так:

    public delegate void ThreadStart();
    //Вызов метода Start начинает выполнение потока.Поток продолжается до выхода из исполняемого метода. Вот пример, 
    //использующий полный синтаксис C# для создания делегата ThreadStart:

    class ThreadTest
    {
        /*
         static void Main()
         {
             Thread t = new Thread(new ThreadStart);
             t.Start();   // Выполнить Go() в новом потоке.
             Go();        // Одновременно запустить Go() в главном потоке.
             Console.ReadLine();
         }

         static void Go()
         {
             Console.WriteLine("hello!");
         }*/
        //Допустим, что в рассматриваемом выше примере мы захотим более явно различать вывод каждого из потоков, 
        //например, по регистру символов.Можно добиться этого, передавая соответствующий флаг в метод Go(), 
        //но в этом случае нельзя использовать делегат ThreadStart, так он не принимает аргументов.
        //К счастью, .NET Framework определяет другую версию делегата – ParameterizedThreadStart, которая может 
        //принимать один аргумент:
        // public delegate void ParameterizedThreadStart(object obj);
        /* static void Main()
         {
             Thread t = new Thread(Go);
             t.Start(true);             // == Go(true) 
             Go(false);
             Console.ReadLine();
         }

         static void Go(object upperCase)
         {
             bool upper = (bool)upperCase;
             Console.WriteLine(upper ? "HELLO!" : "hello!");
         }*/

        // Поток можно проименовать, используя свойство Name.Это предоставляет большое удобство при отладке: 
        //имена потоков можно вывести в Console.WriteLine и увидеть в окне Debug – Threads в Microsoft Visual 
        //Studio.Имя потоку может быть назначено в любой момент, но только один раз – при попытке изменить его 
        //будет сгенерировано исключение.
        // Главному потоку приложения также можно назначить имя – в следующем примере доступ к главному потоку 
        //осуществляется через статическое свойство CurrentThread класса Thread:
        /*
        class ThreadNaming
        {
            static void Main()
            {
                Thread.CurrentThread.Name = "main";
                Thread worker = new Thread(Go);
                worker.Name = "worker";
                worker.Start();
                Go();
                Console.ReadLine();
            }

            static void Go()
            {
                Console.WriteLine("Hello from " + Thread.CurrentThread.Name);
            }
        }*/

        //По умолчанию потоки создаются как основные, что означает, что приложение не будет завершено, 
        //пока один из таких потоков будет исполняться.C# также поддерживает фоновые потоки, они не продлевают 
        //жизнь приложению, а завершаются сразу же, как только все основные потоки будут завершены.

        //ПРИМЕЧАНИЕ
        //Изменение статуса потока с основного на фоновый не изменяет его приоритет или статус в планировщике потоков.
        //Статус потока переключается с основного на фоновый при помощи свойства IsBackground, как показано 
        //в следующем примере:

        class PriorityTest
        {
            static void Main(string[] args)
            {
                Thread worker = new Thread(delegate () { Console.ReadLine(); });

                if (args.Length > 0)
                    worker.IsBackground = true;

                worker.Start();
            }
        }
        // Если программа вызывается без аргументов, рабочий поток выполняется по умолчанию как основной поток и 
        //ожидает на ReadLine, пока пользователь не нажмет Enter.Тем временем главный поток завершается, но 
        //приложение продолжает исполняться, так как рабочий поток еще жив.
        //  Если же программу запустить с аргументами командной строки, рабочий поток получит статус фонового и 
        //программа завершится практически сразу после завершения главного потока, с уничтожением потока, 
        //ожидающего ввода пользователя с помощью метода ReadLine.
        //  Когда фоновый поток завершается таким способом, все блоки finally внутри потока игнорируются.
        //Поскольку невыполнение кода в finally обычно нежелательно, будет правильно ожидать завершения 
        //всех фоновых потоков перед выходом из программы, назначив нужный таймаут (при помощи Thread.Join). 
        //Если по каким-то причинам рабочий поток не завершается за выделенное время, можно попытаться аварийно 
        //завершить его(Thread.Abort), а если и это не получится, позволить умереть ему вместе с процессом
        //(также не помешает записать информацию о проблеме в лог).
        //  Превращение рабочего потока в фоновый может быть последним шансом завершить приложение, так как не 
        //умирающий основной поток не даст приложению завершиться.Зависший основной поток особенно коварен в 
        //приложениях Windows Forms, так как приложение завершается, когда завершается его главный поток 
        //(по крайней мере, для пользователя), но его процесс продолжает выполняться.
        //В диспетчере задач оно исчезнет из списка приложений, хотя имя его исполняемого файла останется 
        //в списке исполняющихся процессов.Пока пользователь не найдет и не прибьет его, процесс продолжит 
        //потреблять ресурсы и, возможно, будет препятствовать запуску или нормальному функционированию 
        //вновь запущенного экземпляра приложения.
      //  ПРЕДУПРЕЖДЕНИЕ
      //  Обычная причина появления приложений, которые не могут завершиться должным образом – это такие “забытые” основные потоки.
    }
    
}
