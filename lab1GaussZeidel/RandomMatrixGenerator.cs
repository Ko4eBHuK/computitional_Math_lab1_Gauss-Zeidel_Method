using System;

namespace lab1GaussZeidel
{
    class RandomMatrixGenerator
    {
        private short Size;             //размер матрицы
        private double[,] Matrix = new double[20,20];      //матрица
        private double[] FreeTerms = new double[20];     //столбец свободных членов
        private bool StrictFlag;        //строгость хотя бы одного неравенства
        private double StrictAdding;
        private Random RandomCoefficientGenerator = new Random();
        private byte[] StrictInequationCount;

        public RandomMatrixGenerator(short outerSize) {
            Size = outerSize;
            GenerateMatrix();
            GenerateFreeTerms();
        }

        public void GenerateMatrix()
        {
            for (int i = 0; i < Size; i++)
            {
                Matrix[i,i] = 0;
                for (int j = 0; j < Size; j++) 
                {
                    if(i != j)
                    {
                        Matrix[i,j] = 0;
                        Matrix[i,j] += RandomCoefficientGenerator.Next(201) - 100;
                        if (Matrix[i, j] >= 0) Matrix[i, j] += (double)RandomCoefficientGenerator.Next(1000000000) / 1000000000;
                        else Matrix[i,j] -= (double)RandomCoefficientGenerator.Next(1000000000) / 1000000000;
                        Matrix[i,i] += Math.Abs(Matrix[i,j]);
                    }
                }
                if(RandomCoefficientGenerator.Next(2) == 1) Matrix[i,i] *= -1;
            }

            StrictFlag = false;
            StrictInequationCount = new Byte[Size];
            while (!StrictFlag) 
            {
                RandomCoefficientGenerator.NextBytes(StrictInequationCount);
                for (int i = 0; i < Size; i++)
                {
                    if (StrictInequationCount[i] % 2 == 0)
                    {
                        int randomExtent = (int)Math.Truncate(Math.Abs(Matrix[i,i]));
                        StrictFlag = true;
                        StrictAdding = RandomCoefficientGenerator.Next(randomExtent + 1) + randomExtent;
                        StrictAdding += (double)RandomCoefficientGenerator.Next(1000000000) / 1000000000;
                        if (Matrix[i,i] > 0) Matrix[i,i] += StrictAdding;
                        else if (Matrix[i,i] < 0) Matrix[i,i] -= StrictAdding;
                        else Matrix[i,i] += 1;
                    }
                }
            }

            

        }

        private void GenerateFreeTerms()
        {            
            for (int i = 0; i < Size; i++)
            {
                FreeTerms[i] = RandomCoefficientGenerator.Next(200 * Size + 1) - 100 * Size;
                if (FreeTerms[i] >= 0) FreeTerms[i] += (double)RandomCoefficientGenerator.Next(1000000000) / 1000000000;
                else FreeTerms[i] -= (double)RandomCoefficientGenerator.Next(1000000000) / 1000000000;
            }
        }

        public double[,] GetMatrix() {
            return Matrix;
        }

        public double[] GetFreeTerms()
        {
            return FreeTerms;
        }
    }
}
