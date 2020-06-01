using System;

namespace lab1GaussZeidel
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleKey pressedKey;

            do
            {
                Console.Clear();
                Console.WriteLine("Выберите формат ввода данных: \nm - ручной ввод | f - прочитать из файла");
                pressedKey = Console.ReadKey().Key;
                MatrixManager matrixInput = new MatrixManager();
                Console.WriteLine();

                switch (pressedKey)
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

                Console.WriteLine("\nПовторить работу программы?\nЛюбая клавиша, кроме Esc - да | Esc - нет");
                pressedKey = Console.ReadKey().Key;
            } while (pressedKey != ConsoleKey.Escape);

        }
    }
}
