// Реализуем программу реагирующую на события
// нажатия клавиш - вызывающих специализированные команды консоли
using System;

namespace ConsoleApplication1
{
    // Производный класс от EventArgs
    class MyEventArgs : EventArgs
    {
        public char ch;
    }

    class KeyEvent
    {
        // Создадим событие, используя обобщенный делегат
        public event EventHandler<MyEventArgs> KeyDown;

        public void OnKeyDown(char ch)
        {
            MyEventArgs c = new MyEventArgs();

            if (KeyDown != null)
            {
                c.ch = ch;
                KeyDown(this, c);
            }
        }
    }

    class Program
    {
        static void Main()
        {
            KeyEvent evnt = new KeyEvent();
            evnt.KeyDown += (sender, e) =>
            {
                switch (e.ch)
                {
                    case 'C':
                        {
                            MyColor(true);
                            break;
                        }
                    case 'B':
                        {
                            MyColor(false);
                            break;
                        }
                    case 'S':
                        {
                            Console.Write("\nВведите длину: ");
                            try
                            {
                                int Width = int.Parse(Console.ReadLine()) / 8;
                                Console.Write("Введите ширину: ");
                                int Height = int.Parse(Console.ReadLine()) / 8;
                                Console.WindowWidth = Width;
                                Console.WindowHeight = Height;
                                Console.WriteLine();
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine("Неверный формат!");
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Console.WriteLine("Окно настолько не растянется!");
                            }
                            break;
                        }
                    case 'T':
                        {
                            Console.Write("\nВведите новый заголовок: ");
                            string s = Console.ReadLine();
                            Console.Title = s;
                            Console.WriteLine();
                            break;
                        }
                    case 'R':
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.WriteLine();
                            break;
                        }
                    case 'E':
                        {
                            Console.Beep();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("\nТакая команда не найдена!");
                            break;
                        }
                }
            };

            ConsoleTitle();
            char ch;
            do
            {
                Console.Write("Введите комманду: ");
                ConsoleKeyInfo key;
                key = Console.ReadKey();
                ch = key.KeyChar;
                evnt.OnKeyDown(key.KeyChar);
            }
            while (ch != 'E');
        }

        // Несколько вспомогательных методов
        static void ConsoleTitle()
        {
            CC(ConsoleColor.Green);
            Console.WriteLine("***************************\n\nПрограмма настройки консоли"
                + "\n___________________________\n");
            CC(ConsoleColor.Yellow);
            Console.WriteLine("Управляющие команды: \n");
            Command("C", "Поменять цвет текста");
            Command("B", "Поменять цвет фона");
            Command("S", "Поменять размер окна");
            Command("T", "Поменять заголовок");
            Command("R", "Сбросить изменения");
            Command("E", "Выход");
            Console.WriteLine();
        }

        static void CC(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        static void Command(string s1, string s2)
        {
            CC(ConsoleColor.Red);
            Console.Write(s1);
            CC(ConsoleColor.White);
            Console.Write(" - " + s2 + "\n");
        }

        static void MyColor(bool F_or_B)
        {
            link1:
            Console.Write("\nВведите цвет: ");
            string s = Console.ReadLine();
            switch (s)
            {
                case "Black":
                    {
                        if (F_or_B)
                            Console.ForegroundColor = ConsoleColor.Black;
                        else
                            Console.BackgroundColor = ConsoleColor.Black;
                        break;
                    }
                case "Yellow":
                    {
                        if (F_or_B)
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        else
                            Console.BackgroundColor = ConsoleColor.Yellow;
                        break;
                    }
                case "Green":
                    {
                        if (F_or_B)
                            Console.ForegroundColor = ConsoleColor.Green;
                        else
                            Console.BackgroundColor = ConsoleColor.Green;
                        break;
                    }
                case "Red":
                    {
                        if (F_or_B)
                            Console.ForegroundColor = ConsoleColor.Red;
                        else
                            Console.BackgroundColor = ConsoleColor.Red;
                        break;
                    }
                case "Blue":
                    {
                        if (F_or_B)
                            Console.ForegroundColor = ConsoleColor.Blue;
                        else
                            Console.BackgroundColor = ConsoleColor.Blue;
                        break;
                    }
                case "Gray":
                    {
                        if (F_or_B)
                            Console.ForegroundColor = ConsoleColor.Gray;
                        else
                            Console.BackgroundColor = ConsoleColor.Gray;
                        break;
                    }
                case "White":
                    {
                        if (F_or_B)
                            Console.ForegroundColor = ConsoleColor.White;
                        else
                            Console.BackgroundColor = ConsoleColor.White;
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Такой цвет я не знаю :(");
                        goto link1;
                    }
            }
            Console.WriteLine("Цвет изменился!");
        }
    }
}
