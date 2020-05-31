using System;

namespace lab1GaussZeidel
{
    class RandomMatrixGenerator
    {
        private short size;             //размер матрицы
        private double[,] matrix = new double[20,20];      //матрица
        private double[] freeTerms = new double[20];     //столбец свободных членов
        private bool strictFlag;        //строгость хотя бы одного неравенства
        private double strictAdding;
        private Random randomCoefficientGenerator = new Random();
        private byte[] strictInequationCount;

        public RandomMatrixGenerator(short outerSize) {
            size = outerSize;
            generateMatrix();
            generateFreeTerms();
        }

        public void generateMatrix()
        {
            for (int i = 0; i < size; i++)
            {
                matrix[i,i] = 0;
                for (int j = 0; j < size; j++) 
                {
                    if(i != j)
                    {
                        matrix[i,j] = 0;
                        matrix[i,j] += randomCoefficientGenerator.Next(201) - 100;
                        if (matrix[i, j] >= 0) matrix[i, j] += (double)randomCoefficientGenerator.Next(1000000000) / 1000000000;
                        else matrix[i,j] -= (double)randomCoefficientGenerator.Next(1000000000) / 1000000000;
                        matrix[i,i] += Math.Abs(matrix[i,j]);
                    }
                }
                if(randomCoefficientGenerator.Next(2) == 1) matrix[i,i] *= -1;
            }

            strictFlag = false;
            strictInequationCount = new Byte[size];
            while (!strictFlag) 
            {
                randomCoefficientGenerator.NextBytes(strictInequationCount);
                for (int i = 0; i < size; i++)
                {
                    if (strictInequationCount[i] % 2 == 0)
                    {
                        int randomExtent = (int)Math.Truncate(Math.Abs(matrix[i,i]));
                        strictFlag = true;
                        strictAdding = randomCoefficientGenerator.Next(randomExtent + 1) + randomExtent;
                        strictAdding += (double)randomCoefficientGenerator.Next(1000000000) / 1000000000;
                        if (matrix[i,i] > 0) matrix[i,i] += strictAdding;
                        else if (matrix[i,i] < 0) matrix[i,i] -= strictAdding;
                        else matrix[i,i] += 1;
                    }
                }
            }

            

        }

        private void generateFreeTerms()
        {            
            for (int i = 0; i < size; i++)
            {
                freeTerms[i] = randomCoefficientGenerator.Next(200 * size + 1) - 100 * size;
                if (freeTerms[i] >= 0) freeTerms[i] += (double)randomCoefficientGenerator.Next(1000000000) / 1000000000;
                else freeTerms[i] -= (double)randomCoefficientGenerator.Next(1000000000) / 1000000000;
            }
        }

        public double[,] getMatrix() {
            return matrix;
        }

        public double[] getFreeTerms()
        {
            return freeTerms;
        }
    }
}
