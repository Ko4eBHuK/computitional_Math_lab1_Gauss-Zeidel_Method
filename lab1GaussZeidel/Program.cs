using System;

namespace lab1GaussZeidel
{
    class Program
    {
        

        static void Main(string[] args)
        {
            ConsoleKey key;

            do
            {
                Console.Clear();
                Console.WriteLine("Выберите формат ввода данных: \nm - ручной ввод | f - прочитать из файла");
                key = Console.ReadKey().Key;
                MatrixManager matrixInput = new MatrixManager();
                Console.WriteLine();

                switch (key)
                {
                    case ConsoleKey.M:
                        matrixInput.ManualInputAndSolution();
                        break;
                    case ConsoleKey.F:
                        matrixInput.FilelInputAndSolution();
                        break;
                    default:
                        Console.WriteLine("Ваш выбор не m или f.");
                        break;
                }

                Console.WriteLine("Повторить работу программы?\nЛюбая клавиша, кроме Esc - да | Esc - нет");
                key = Console.ReadKey().Key;
            } while (key != ConsoleKey.Escape);

        }
    }
}
