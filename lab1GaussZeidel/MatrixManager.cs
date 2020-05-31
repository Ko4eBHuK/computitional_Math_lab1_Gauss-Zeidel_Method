using System;
using System.IO;

namespace lab1GaussZeidel
{
    internal class MatrixManager
    {
        private short size;                                     //размер матрицы
        private double inaccuracy;                              //погрешность
        private double[,] matrix = new double[20, 20];          //матрица
        private double[] freeTerms = new double[20];            //столбец свободных членов
        private double[] attacVector;                           //вектор решений
        private double[] inaccuracyVector = new double[20];     //вектор погрешностей
        private ConsoleKey choise;
        private int[] indexesOfMaxValues = new int[20];
        private string[] buffer;

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

                size = short.Parse(file.ReadLine());

                if (size > 20 | size < 2)
                {
                    Console.WriteLine("Некорректный размер матрицы.");
                    return;
                }

                for (int i = 0; i < size; i++)
                {
                    buffer = file.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < size; j++)
                    {
                        matrix[i, j] = Double.Parse(buffer[j]);
                    }
                    freeTerms[i] = Double.Parse(buffer[size]);
                }
                inaccuracy = Double.Parse(file.ReadLine());

                file.Close();
            }
            catch
            {
                Console.WriteLine("С данными что-то не так");
                return;
            }

            Console.Write("\nПолучена система:");
            for (int i = 0; i < size; i++)
            {
                Console.WriteLine();
                for (int j = 0; j < size; j++)
                {
                    Console.Write("{0}*x{1}", Math.Round(matrix[i, j], 9), j + 1);
                    if (j < size - 1) Console.Write(" + ");
                    else Console.Write(" = {0}", freeTerms[i]);
                }
            }
            Console.WriteLine("\n\nДопустимая погрешность решения: " + inaccuracy);
            Console.WriteLine();

            // проверки условий сходимости
            if (isTransformRequaired())
            {
                Console.WriteLine("Необходимо преобразование матрицы.");
                if (isTransformPossible())
                {
                    transform();
                    Console.WriteLine("Матрица преобразована.");
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            if (j < size - 1) Console.Write("{0}*x{1} + ", matrix[i, j], j + 1);
                            else Console.Write("{0}*x{1}", matrix[i, j], j + 1);
                        }
                        Console.Write(" = {0}\n", freeTerms[i]);
                    }
                }
                else
                {
                    Console.WriteLine("Преобразование матрицы невозможно.");
                    //statusCode = 2; // код провала при преобразовании матрицы
                }
            }
            if (isMatrixDiagonallyDominant())
            {
                Console.WriteLine("В матрице присутсвует диагональное преобладание.");
                if (isMatrixDiagonallyDominantStrictOnceOrMore())
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

            solveAndTransformEquationSystem();
        }

        public void ManualInputAndSolution()
        {

            // ввод размерности
            while (size > 20 | size < 2)
            {
                Console.Write("Введите размерность матрицы (от 2 до 20): ");
                try {
                    size = short.Parse(Console.ReadLine());
                }
                catch { }
                if (size < 2 | size > 20) Console.WriteLine("Некорректный ввод.");
            }

            Console.WriteLine("Задать коэффицинты рандомно?[y/n]");
            choise = Console.ReadKey().Key;
            Console.WriteLine();
            
            while (choise != ConsoleKey.Y & choise != ConsoleKey.N)
            {
                Console.WriteLine("Ожидалось, что Вы нажмёте либо y либо n\nВы же нажали {0} \nПопробуйте снова:", choise.ToString() );
                choise = Console.ReadKey().Key;
            }
            
            // рандомное заданиче коэффициентов
            if (choise == ConsoleKey.Y)
            {
                do
                {
                    RandomMatrixGenerator matrixGenerator = new RandomMatrixGenerator(size);
                    matrix = matrixGenerator.getMatrix();
                    freeTerms = matrixGenerator.getFreeTerms();

                    for (int i = 0; i < size; i++)
                    {
                        Console.WriteLine();
                        for (int j = 0; j < size; j++)
                        {
                            Console.Write("{0}*x{1}", Math.Round(matrix[i, j], 9), j + 1);
                            if (j < size - 1) Console.Write(" + ");
                            else Console.Write(" = {0}", freeTerms[i]);
                        }
                    }
                    Console.WriteLine("\nПерегенерировать?\nДа - любая кнопка, Нет - Esc\n");
                } while (Console.ReadKey().Key != ConsoleKey.Escape);
            }
            //ручной ввод матрицы
            else if(choise == ConsoleKey.N)
            {
                bool correctFlag;
                Console.WriteLine("Введите матрицу по одному элементу включая свободные члены:\n" +
                    "Сначала коэффициенты одной строки, потом свободный член, потом следующую строку ");
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        correctFlag = false;
                        while (!correctFlag)
                        {
                            Console.Write("введите a[{0},{1}]: ", i + 1, j + 1);
                            try
                            {
                                matrix[i, j] = Double.Parse(Console.ReadLine());
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
                            freeTerms[i] = Double.Parse(Console.ReadLine());
                            correctFlag = true;
                        }
                        catch { }
                    }
                }

                Console.WriteLine("Введённая Вами матрица:");
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (j < size - 1) Console.Write("{0}*x{1} + ", matrix[i,j], j+1);
                        else Console.Write("{0}*x{1}", matrix[i, j], j+1);
                    }
                    Console.Write(" = {0}\n", freeTerms[i]);
                }

                // проверки условий сходимости
                if (isTransformRequaired())
                {
                    Console.WriteLine("Необходимо преобразование матрицы.");
                    if (isTransformPossible())
                    {
                        transform();
                        Console.WriteLine("Матрица преобразована.");
                        for (int i = 0; i < size; i++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                if (j < size - 1) Console.Write("{0}*x{1} + ", matrix[i, j], j + 1);
                                else Console.Write("{0}*x{1}", matrix[i, j], j + 1);
                            }
                            Console.Write(" = {0}\n", freeTerms[i]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Преобразование матрицы невозможно.");
                        //statusCode = 2; // код провала при преобразовании матрицы
                    }
                }
                if (isMatrixDiagonallyDominant())
                {
                    Console.WriteLine("В матрице присутсвует диагональное преобладание.");
                    if (isMatrixDiagonallyDominantStrictOnceOrMore())
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

            // ввод погрешности
            inaccuracy = 102;
            while (inaccuracy > 101)
            {
                Console.Write("Введите погрешность решения: ");
                try
                {
                    inaccuracy = double.Parse(Console.ReadLine());
                }
                catch {
                    Console.WriteLine("Некорректный ввод");
                }
            }

            //// преобразование матрицы для решения
            //GaussZeidelMethod solution = new GaussZeidelMethod(size, matrix, freeTerms, inaccuracy);
            //matrix = solution.getMatrix();
            //freeTerms = solution.getFreeTerms();
            //Console.Write("Преобразованный вид матрицы таков:\n");
            //for(int i = 0; i < size; i++)
            //{
            //    Console.Write("x{0} = ", i + 1);
            //    for(int j = 0; j < size; j++)
            //    {
            //        if(j != i)
            //        {
            //            Console.Write("{0}*x{1} + ", Math.Round(matrix[i, j], 9), j + 1);
            //        }
            //    }
            //    Console.Write("{0}\n", Math.Round(freeTerms[i], 9));
            //}

            //Console.WriteLine();
            ////решение
            //solution.solution();
            //Console.WriteLine("Число итераций: {0}", solution.getIterationCount());

            //Console.WriteLine("Вектор решений:");
            //attacVector = solution.getAttackVector();
            //for (int i = 0; i < size; i++)
            //{
            //    Console.WriteLine("x{0} = {1}", i + 1, attacVector[i]);
            //}

            //Console.WriteLine("Вектор погрешностей:");
            //inaccuracyVector = solution.getInaccuracyVector();
            //for (int i = 0; i < size; i++)
            //{
            //    Console.WriteLine("delta{0} = {1}", i + 1, inaccuracyVector[i]);
            //}

            solveAndTransformEquationSystem();
        }

        public bool isTransformRequaired()
        {
            bool requairedFlag = false;
            int i = 0, j;

            while (!requairedFlag && i < size)
            {
                j = 0;
                while (!requairedFlag && j < size)
                {
                    if(i != j) requairedFlag = Math.Abs(matrix[i, i]) < Math.Abs(matrix[i, j]);
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

            for(int i = 0; i < size; i++)
            {
                rowMax = Math.Abs(matrix[i,0]);
                indexRowMax = 0;
                for (int j = 1; j < size; j++)
                {
                    if(Math.Abs(matrix[i, j]) > rowMax)
                    {
                        rowMax = Math.Abs(matrix[i, j]);
                        indexRowMax = j;
                    }
                }
                indexesOfMaxValues[i] = indexRowMax;
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 1; j < size; j++)
                {
                    if (i != j)
                    {
                        if (indexesOfMaxValues[i] == indexesOfMaxValues[j]) return false;
                    }
                }
            }

            return true;
        }

        public void transform()
        {
            double[,] newMatrix = new double[20, 20];
            double[] newFreeTerms = new double[20];

            for (int i = 0; i < size; i++)
            {
                if (indexesOfMaxValues[i] != i)
                {
                    for (int j = 0; j < size; j++)
                    {
                        newMatrix[indexesOfMaxValues[i], j] = matrix[i, j];
                    }
                    newFreeTerms[indexesOfMaxValues[i]] = freeTerms[i];
                }
                else
                {
                    for (int j = 0; j < size; j++)
                    {
                        newMatrix[i, j] = matrix[i, j];
                    }
                    newFreeTerms[i] = freeTerms[i];
                }
            }

            matrix = newMatrix;
            freeTerms = newFreeTerms;
        }

        public bool isMatrixDiagonallyDominant()
        {
            bool requairedFlag = false;
            double rowSum;
            for (int i = 0; i < size; i++)
            {
                rowSum = Math.Abs(matrix[i,i]);
                for (int j = 0; j < size; j++)
                {
                    rowSum -= Math.Abs(matrix[i, j]);
                }
                rowSum += Math.Abs(matrix[i, i]);
                requairedFlag = rowSum > 0;
                if (requairedFlag) return requairedFlag;
            }

            return requairedFlag;
        }

        public bool isMatrixDiagonallyDominantStrictOnceOrMore()
        {
            bool strictFlag = true;
            int i = 0, j;
            double rowSum;

            while (strictFlag && i < size)
            {
                j = 0;
                rowSum = 0;
                while (strictFlag && j < size)
                {
                    if (i != j) rowSum += Math.Abs(matrix[i, j]);
                    strictFlag = Math.Abs(matrix[i, i]) > rowSum;
                    j++;
                }
                i++;
            }

            return strictFlag;
        }

        private void solveAndTransformEquationSystem()
        {
            // преобразование матрицы для решения
            GaussZeidelMethod solution = new GaussZeidelMethod(size, matrix, freeTerms, inaccuracy);
            matrix = solution.getMatrix();
            freeTerms = solution.getFreeTerms();
            Console.Write("\nПреобразованный вид матрицы таков:\n");
            for (int i = 0; i < size; i++)
            {
                Console.Write("x{0} = ", i + 1);
                for (int j = 0; j < size; j++)
                {
                    if (j != i)
                    {
                        Console.Write("{0}*x{1} + ", Math.Round(matrix[i, j], 9), j + 1);
                    }
                }
                Console.Write("{0}\n", Math.Round(freeTerms[i], 9));
            }

            Console.WriteLine();
            //решение
            solution.solution();
            Console.WriteLine("Число итераций: {0}", solution.getIterationCount());

            Console.WriteLine("Вектор решений:");
            attacVector = solution.getAttackVector();
            for (int i = 0; i < size; i++)
            {
                Console.WriteLine("x{0} = {1}", i + 1, attacVector[i]);
            }

            Console.WriteLine("Вектор погрешностей:");
            inaccuracyVector = solution.getInaccuracyVector();
            for (int i = 0; i < size; i++)
            {
                Console.WriteLine("delta{0} = {1}", i + 1, inaccuracyVector[i]);
            }
        }


    }

}
