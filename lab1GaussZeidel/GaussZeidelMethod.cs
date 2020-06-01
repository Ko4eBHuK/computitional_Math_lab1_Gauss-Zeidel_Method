using System;
using System.Linq;

namespace lab1GaussZeidel
{
    class GaussZeidelMethod
    {
        private short Size;                                     //размер матрицы
        private double Inaccuracy;                              //погрешность
        private double[,] Matrix = new double[20, 20];          //матрица
        private double[] FreeTerms = new double[20];            //столбец свободных членов
        private double[] AttackVector = new double[20];         //вектор решений
        private double[] InaccuracyVector = new double[20];     //вектор погрешностей
        private double[] PreviousAttackVector = new double[20];
        private int IterationCount;
        private double MaxDifference;

        public GaussZeidelMethod(short outerSize, double[,] outerMatrix, double[] outerFreeTerms, double outerInaccuracy)
        {
            Size = outerSize;
            Inaccuracy = outerInaccuracy;

            for (int i = 0; i < Size; i++)
            {
                FreeTerms[i] = outerFreeTerms[i] / outerMatrix[i, i];

                for (int j = 0; j < Size; j++)
                {
                    Matrix[i, j] = (-1 * outerMatrix[i, j]) / outerMatrix[i, i];
                }
            }
        }

        public void Solution()
        {

            for (int i = 0; i < Size; i++)
            {
                AttackVector[i] = FreeTerms[i];
            }
            IterationCount = 0;

            do
            {
                IterationCount++;
                for (int i = 0; i < Size; i++)
                {
                    PreviousAttackVector[i] = AttackVector[i];
                }

                for (int i = 0; i < Size; i++)
                {
                    AttackVector[i] = 0;
                    for(int j = 0; j < Size; j++)
                    {
                        if(i != j)
                        {
                            AttackVector[i] += AttackVector[j] * Matrix[i, j];
                        }
                    }
                    AttackVector[i] += FreeTerms[i];
                    AttackVector[i] = Math.Round(AttackVector[i], 9);
                    InaccuracyVector[i] = Math.Abs(AttackVector[i] - PreviousAttackVector[i]);
                }

                MaxDifference = InaccuracyVector.Max();

            } while (MaxDifference > Inaccuracy);
        }

        public double[,] GetMatrix()
        {
            return Matrix;
        }

        public double[] GetFreeTerms()
        {
            return FreeTerms;
        }

        public double[] GetAttackVector()
        {
            return AttackVector;
        }

        public int GetIterationCount()
        {
            return IterationCount;
        }

        public double[] GetInaccuracyVector()
        {
            return InaccuracyVector;
        }
    }
}