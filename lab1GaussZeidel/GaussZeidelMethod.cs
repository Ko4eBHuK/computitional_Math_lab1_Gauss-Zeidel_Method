using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lab1GaussZeidel
{
    class GaussZeidelMethod
    {
        private short size;                                     //размер матрицы
        private double inaccuracy;                              //погрешность
        private double[,] matrix = new double[20, 20];          //матрица
        private double[] freeTerms = new double[20];            //столбец свободных членов
        private double[] attackVector = new double[20];         //вектор решений
        private double[] inaccuracyVector = new double[20];     //вектор погрешностей
        private double[] previousAttackVector = new double[20];
        private int iterationCount;
        private double maxDifference;

        public GaussZeidelMethod(short outerSize, double[,] outerMatrix, double[] outerFreeTerms, double outerInaccuracy)
        {
            size = outerSize;
            inaccuracy = outerInaccuracy;

            for (int i = 0; i < size; i++)
            {
                freeTerms[i] = outerFreeTerms[i] / outerMatrix[i, i];

                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = (-1 * outerMatrix[i, j]) / outerMatrix[i, i];
                }
            }
        }

        public void solution()
        {

            for (int i = 0; i < size; i++)
            {
                attackVector[i] = freeTerms[i];
            }
            iterationCount = 0;

            do
            {
                iterationCount++;
                for (int i = 0; i < size; i++)
                {
                    previousAttackVector[i] = attackVector[i];
                }

                for (int i = 0; i < size; i++)
                {
                    attackVector[i] = 0;
                    for(int j = 0; j < size; j++)
                    {
                        if(i != j)
                        {
                            attackVector[i] += attackVector[j] * matrix[i, j];
                        }
                    }
                    attackVector[i] += freeTerms[i];
                    attackVector[i] = Math.Round(attackVector[i], 9);
                    inaccuracyVector[i] = Math.Abs(attackVector[i] - previousAttackVector[i]);
                }

                maxDifference = inaccuracyVector.Max();

            } while (maxDifference > inaccuracy);
        }

        public double[,] getMatrix()
        {
            return matrix;
        }

        public double[] getFreeTerms()
        {
            return freeTerms;
        }

        public double[] getAttackVector()
        {
            return attackVector;
        }

        public int getIterationCount()
        {
            return iterationCount;
        }

        public double[] getInaccuracyVector()
        {
            return inaccuracyVector;
        }
    }
}