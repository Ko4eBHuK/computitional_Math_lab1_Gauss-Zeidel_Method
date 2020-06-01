using System;
using System.IO;

namespace lab1GaussZeidel
{
    internal class MatrixManager
    {
        private short Size;                                     //размер матрицы
        private double Inaccuracy;                              //погрешность
        private double[,] Matrix = new double[20, 20];          //матрица
        private double[] FreeTerms = new double[20];            //столбец свободных членов
        private double[] AttacVector;                           //вектор решений
        private double[] InaccuracyVector = new double[20];     //вектор погрешностей
        private ConsoleKey Choise;
        private int[] IndexesOfMaxValues = new int[20];
        private string[] Buffer;

        public void FilelInputAndSolution()
        {
            string inputFilePath = "../../../../";
            
            Console.WriteLine("Данные в файле должны находиться в таком формате:\n" +
                              "1)В первой строке задан размер матрицы - n, в виде натурального числа от 2 до 20;\n" +
                              "2)В последующих n строках заданы коэффициенты и свободные члены, по одному уравнени на строку;\n" +
                              "3)Точность решения.");
            Console.WriteLine("Введите имя(*) файла ( *.txt должен существовать и находится в корневой директории проекта): ");
            inputFilePath += Console.ReadLine();
            inputFilePath += ".txt";
            
            try
            {
                StreamReader file = new StreamReader(inputFilePath);

                Size = short.Parse(file.ReadLine());

                if (Size > 20 | Size < 2)
                {
                    Console.WriteLine("Некорректный размер матрицы.");
                    return;
                }

                for (int i = 0; i < Size; i++)
                {
                    Buffer = file.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < Size; j++)
                    {
                        Matrix[i, j] = Double.Parse(Buffer[j]);
                    }
                    FreeTerms[i] = Double.Parse(Buffer[Size]);
                }
                Inaccuracy = Double.Parse(file.ReadLine());

                file.Close();
            }
            catch
            {
                Console.WriteLine("С данными что-то не так");
                return;
            }

            Console.Write("\nПолучена система:");
            for (int i = 0; i < Size; i++)
            {
                Console.WriteLine();
                for (int j = 0; j < Size; j++)
                {
                    Console.Write("{0}*x{1}", Math.Round(Matrix[i, j], 9), j + 1);
                    if (j < Size - 1) Console.Write(" + ");
                    else Console.Write(" = {0}", FreeTerms[i]);
                }
            }
            Console.WriteLine("\n\nДопустимая погрешность решения: " + Inaccuracy);
            Console.WriteLine();

            MatrixReview();

            SolveAndTransformEquationSystem();
        }

        public void ManualInputAndSolution()
        {

            // ввод размерности
            while (Size > 20 | Size < 2)
            {
                Console.Write("Введите размерность матрицы (от 2 до 20): ");
                try {
                    Size = short.Parse(Console.ReadLine());
                }
                catch { }
                if (Size < 2 | Size > 20) Console.WriteLine("Некорректный ввод.");
            }

            Console.WriteLine("Задать коэффицинты рандомно?[y/n]");
            Choise = Console.ReadKey().Key;
            Console.WriteLine();
            
            while (Choise != ConsoleKey.Y & Choise != ConsoleKey.N)
            {
                Console.WriteLine("Ожидалось, что Вы нажмёте либо y либо n\nВы же нажали {0} \nПопробуйте снова:", Choise.ToString() );
                Choise = Console.ReadKey().Key;
            }
            
            // рандомное заданиче коэффициентов
            if (Choise == ConsoleKey.Y)
            {
                do
                {
                    RandomMatrixGenerator matrixGenerator = new RandomMatrixGenerator(Size);
                    Matrix = matrixGenerator.GetMatrix();
                    FreeTerms = matrixGenerator.GetFreeTerms();

                    for (int i = 0; i < Size; i++)
                    {
                        Console.WriteLine();
                        for (int j = 0; j < Size; j++)
                        {
                            Console.Write("{0}*x{1}", Math.Round(Matrix[i, j], 9), j + 1);
                            if (j < Size - 1) Console.Write(" + ");
                            else Console.Write(" = {0}", FreeTerms[i]);
                        }
                    }
                    Console.WriteLine("\nПерегенерировать?\nДа - любая кнопка, Нет - Esc\n");
                } while (Console.ReadKey().Key != ConsoleKey.Escape);
            }
            //ручной ввод матрицы
            else if(Choise == ConsoleKey.N)
            {
                bool correctFlag;
                Console.WriteLine("Введите матрицу по одному элементу включая свободные члены:\n" +
                    "Сначала коэффициенты одной строки, потом свободный член, потом следующую строку ");
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        correctFlag = false;
                        while (!correctFlag)
                        {
                            Console.Write("введите a[{0},{1}]: ", i + 1, j + 1);
                            try
                            {
                                Matrix[i, j] = Double.Parse(Console.ReadLine());
                                correctFlag = true;
                            }
                            catch { }
                        }
                    }
                    correctFlag = false;
                    while (!correctFlag)
                    {
                        Console.Write("введите b[{0}]: ", i + 1);
                        try
                        {
                            FreeTerms[i] = Double.Parse(Console.ReadLine());
                            correctFlag = true;
                        }
                        catch { }
                    }
                }

                Console.WriteLine("Введённая Вами матрица:");
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        if (j < Size - 1) Console.Write("{0}*x{1} + ", Matrix[i,j], j+1);
                        else Console.Write("{0}*x{1}", Matrix[i, j], j+1);
                    }
                    Console.Write(" = {0}\n", FreeTerms[i]);
                }

                MatrixReview();
            }

            // ввод погрешности
            Inaccuracy = 102;
            while (Inaccuracy > 101)
            {
                Console.Write("Введите погрешность решения: ");
                try
                {
                    Inaccuracy = double.Parse(Console.ReadLine());
                }
                catch {
                    Console.WriteLine("Некорректный ввод");
                }
            }

            SolveAndTransformEquationSystem();
        }

        public bool IsTransformRequaired()
        {
            bool requairedFlag = false;
            int i = 0, j;

            while (!requairedFlag && i < Size)
            {
                j = 0;
                while (!requairedFlag && j < Size)
                {
                    if(i != j) requairedFlag = Math.Abs(Matrix[i, i]) < Math.Abs(Matrix[i, j]);
                    j++;
                }
                i++;
            }

            return requairedFlag;
        }

        public bool isTransformPossible()
        {
            double rowMax;
            int indexRowMax;

            for(int i = 0; i < Size; i++)
            {
                rowMax = Math.Abs(Matrix[i,0]);
                indexRowMax = 0;
                for (int j = 1; j < Size; j++)
                {
                    if(Math.Abs(Matrix[i, j]) > rowMax)
                    {
                        rowMax = Math.Abs(Matrix[i, j]);
                        indexRowMax = j;
                    }
                }
                IndexesOfMaxValues[i] = indexRowMax;
            }

            for (int i = 0; i < Size; i++)
            {
                for (int j = 1; j < Size; j++)
                {
                    if (i != j)
                    {
                        if (IndexesOfMaxValues[i] == IndexesOfMaxValues[j]) return false;
                    }
                }
            }

            return true;
        }

        public void Transform()
        {
            double[,] newMatrix = new double[20, 20];
            double[] newFreeTerms = new double[20];

            for (int i = 0; i < Size; i++)
            {
                if (IndexesOfMaxValues[i] != i)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        newMatrix[IndexesOfMaxValues[i], j] = Matrix[i, j];
                    }
                    newFreeTerms[IndexesOfMaxValues[i]] = FreeTerms[i];
                }
                else
                {
                    for (int j = 0; j < Size; j++)
                    {
                        newMatrix[i, j] = Matrix[i, j];
                    }
                    newFreeTerms[i] = FreeTerms[i];
                }
            }

            Matrix = newMatrix;
            FreeTerms = newFreeTerms;
        }

        public bool IsMatrixDiagonallyDominant()
        {
            bool requairedFlag = true;
            double rowSum;
            for (int i = 0; i < Size; i++)
            {
                rowSum = Math.Abs(Matrix[i,i]);
                for (int j = 0; j < Size; j++)
                {
                    rowSum -= Math.Abs(Matrix[i, j]);
                }
                rowSum += Math.Abs(Matrix[i, i]);
                requairedFlag = rowSum > 0;
                if (!requairedFlag) return requairedFlag;
            }

            return requairedFlag;
        }

        public bool IsMatrixDiagonallyDominantStrictOnceOrMore()
        {
            bool strictFlag = true;
            int i = 0, j;
            double rowSum;

            while (strictFlag && i < Size)
            {
                j = 0;
                rowSum = 0;
                while (strictFlag && j < Size)
                {
                    if (i != j) rowSum += Math.Abs(Matrix[i, j]);
                    strictFlag = Math.Abs(Matrix[i, i]) > rowSum;
                    j++;
                }
                i++;
            }

            return strictFlag;
        }

        private void SolveAndTransformEquationSystem()
        {
            // преобразование матрицы для решения
            GaussZeidelMethod solution = new GaussZeidelMethod(Size, Matrix, FreeTerms, Inaccuracy);
            Matrix = solution.GetMatrix();
            FreeTerms = solution.GetFreeTerms();
            Console.Write("\nПреобразованный вид матрицы таков:\n");
            for (int i = 0; i < Size; i++)
            {
                Console.Write("x{0} = ", i + 1);
                for (int j = 0; j < Size; j++)
                {
                    if (j != i)
                    {
                        Console.Write("{0}*x{1} + ", Math.Round(Matrix[i, j], 9), j + 1);
                    }
                }
                Console.Write("{0}\n", Math.Round(FreeTerms[i], 9));
            }

            Console.WriteLine();
            //решение
            solution.Solution();
            Console.WriteLine("Число итераций: {0}", solution.GetIterationCount());

            Console.WriteLine("Вектор решений:");
            AttacVector = solution.GetAttackVector();
            for (int i = 0; i < Size; i++)
            {
                Console.WriteLine("x{0} = {1}", i + 1, AttacVector[i]);
            }

            Console.WriteLine("Вектор погрешностей:");
            InaccuracyVector = solution.GetInaccuracyVector();
            for (int i = 0; i < Size; i++)
            {
                Console.WriteLine("delta{0} = {1}", i + 1, InaccuracyVector[i]);
            }
        }

        private void MatrixReview()
        {
            // проверки условий сходимости
            if (IsTransformRequaired())
            {
                Console.WriteLine("Необходимо преобразование матрицы.");
                if (isTransformPossible())
                {
                    Transform();
                    Console.WriteLine("Матрица преобразована.");
                    for (int i = 0; i < Size; i++)
                    {
                        for (int j = 0; j < Size; j++)
                        {
                            if (j < Size - 1) Console.Write("{0}*x{1} + ", Matrix[i, j], j + 1);
                            else Console.Write("{0}*x{1}", Matrix[i, j], j + 1);
                        }
                        Console.Write(" = {0}\n", FreeTerms[i]);
                    }
                }
                else
                {
                    Console.WriteLine("Преобразование матрицы невозможно.");
                    //statusCode = 2; // код провала при преобразовании матрицы
                }
            }
            if (IsMatrixDiagonallyDominant())
            {
                Console.WriteLine("В матрице присутсвует диагональное преобладание.");
                if (IsMatrixDiagonallyDominantStrictOnceOrMore())
                {
                    Console.WriteLine("Достаточные условия сходимости метода не нарушены.");
                    //statusCode = 3; // код того, что матрица соблюдает все достаточные условия сходимости метода
                }
                else
                {
                    Console.WriteLine("Достаточные условия сходимости метода нарушены.\n" +
                        "Возможны неправильные ответы.");
                    //statusCode = 4; // код того, что матрица не соблюдает строгость хотя бы одного неравенства преобладания
                }
            }
            else
            {
                Console.WriteLine("Достаточные условия сходимости метода нарушены.\n" +
                        "Возможны неправильные ответы.");
                //statusCode = 5; // код несоблюдения условий сходимости метода
            }
        }
    }

}
