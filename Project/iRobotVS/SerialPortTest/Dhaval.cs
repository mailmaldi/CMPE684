using System;

namespace SerialPortTest
{

    public class Class1
    {
        //public static int[][] matrix = new int[5][5];

        public static void test(int[,] rssi)
        {
            Console.Out.WriteLine("Hello world");
            double[,] distancematrix = new double[5, 5];
            int onemeter = -7;
            double constant = 3.5;
            //int[,] rssi = new int[5, 5] { { 15, 12, 13, 14, 15 }, { 12, 13, 14, 15, 11 }, { 13, 14, 15, 11, 12 }, { 14, 15, 11, 12, 13 }, { 15, 14, 13, 12, 11 } };

            Console.Out.Write("distance matrix ");
            for (int i = 0; i < 5; i++)
            {
                Console.Out.WriteLine("\n");

                for (int j = 0; j < 5; j++)
                {
                    if (i == j)
                    {
                        distancematrix[i, j] = 1000;
                        Console.Out.Write(distancematrix[i, j] + " ");
                    }

                    else
                    {
                        double temp1 = 3 * (rssi[i, j] - 1);
                        double temp2 = temp1 - 91;
                        double temp3 = (onemeter - temp2) / (10*constant);
                        distancematrix[i, j] = Math.Pow(10,temp3);

                        Console.Out.Write(distancematrix[i, j] + " ");
                    }

                }
            }

            double[,] coordinates = new double[5, 2];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    coordinates[i, j] = 1000;
                }
            }

            double[] theta;
            theta = new double[5];

            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                {
                    coordinates[i, 0] = 0;
                    coordinates[i, 1] = 0;
                    theta[i] = 1000;
                }
                else if (i == 2)
                {
                    coordinates[i, 0] = distancematrix[0, 2];
                    coordinates[i, 1] = 0;
                    theta[i] = 1000;
                }

                else
                {
                    double temp1 = Math.Pow(distancematrix[0, 2], 2) + Math.Pow(distancematrix[0, i], 2) - Math.Pow(distancematrix[2, i], 2);
                    double temp2 = temp1 / (2 * distancematrix[0, 2] * distancematrix[0, 1]);
                    theta[i] = Math.Acos(temp2);

                }
            }

            Console.Out.WriteLine("Theta");
            for (int i = 0; i < 5; i++)
            {
                Console.Out.WriteLine("\n");
                Console.Out.Write(theta[i] + " ");
            }

            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                    continue;
                else if (i == 2)
                    continue;
                else
                {
                    coordinates[i, 0] = Math.Cos(theta[i]) * distancematrix[0, i];
                    coordinates[i, 1] = Math.Sin(theta[i]) * distancematrix[0, i];
                }
            }
            for (int i = 0; i < 5; i++)
            {
                Console.Out.WriteLine("\n");
                for (int j = 0; j < 2; j++)
                {
                    Console.Out.Write(coordinates[i, j] + " ");
                }
            }

        }

    }
}

