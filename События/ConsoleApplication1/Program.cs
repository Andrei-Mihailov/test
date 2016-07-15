using System;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Создадим экземпляр класса ClassCounter. 
            //А также создадим по экземпляру классов, которые должны запуститься. (Они должны быть public).

            ClassCounter Counter = new ClassCounter();
            Handler_I Handler1 = new Handler_I();
            Handler_II Handler2 = new Handler_II();

            //Подписались на событие
            Counter.onCount += Handler1.Message;
            Counter.onCount += Handler2.Message;

            //Запустили счетчик
            Counter.Count();

            Console.ReadLine();
        }
    }
    
    class ClassCounter  //Это класс - в котором производится счет.
    {
        public int c = 5;//наследуемая переменная в других классах

        //Синтаксис по сигнатуре метода, на который мы создаем делегат: 
        //delegate <выходной тип> ИмяДелегата(<тип входных параметров>);
        //Мы создаем на void Message(). Он должен запуститься, когда условие выполнится.
        
        public delegate void MethodContainer();
        //Событие OnCount c типом делегата MethodContainer.
        public event MethodContainer onCount;

        public void Count()
        {
            for (int i = 0; i < 100; i++)
            {
                if (i == c)
                {
                    onCount?.Invoke();
                }

            }
        }
    }

    class Handler_I: ClassCounter //Это класс, реагирующий на событие (счет равен 71) записью строки в консоли.
    {
        public void Message()
        {
            //Не забудьте using System 
            //для вывода в консольном приложении
            Console.WriteLine("Пора действовать, ведь уже {0}!", c);
        }
    }

    class Handler_II: ClassCounter
    {
        public void Message()
        {
            Console.WriteLine("Точно, уже {0}!", c);
        }
    }
}
