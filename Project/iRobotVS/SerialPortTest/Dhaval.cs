using System;

namespace SerialPortTest
{

    public class Class1
    {
        //public static int[][] matrix = new int[5][5];

        public static void test()
        {
            Console.Out.WriteLine("Hello world");
            double[,] distancematrix = new double[5, 5];
            int onemeter = -7;
            double constant = 3.5;
            int[,] rssi = new int[5, 5] { { 1, 2, 3, 4, 5 }, { 2, 3, 4, 5, 1 }, { 3, 4, 5, 1, 2 }, { 4, 5, 1, 2, 3 }, { 5, 4, 3, 2, 1 } };

            for (int i = 0; i < 5; i++)
            {
                Console.Out.WriteLine("\n");
                for (int j = 0; j < 5; j++)
                {
                    Console.Out.Write(rssi[i, j] + " ");
                }
            }



            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (i == j)
                        distancematrix[i, j] = 1000;
                    else
                    {
                        double temp1 = 3 * (rssi[i, j] - 1);
                        double temp2 = temp1 - 91;
                        double temp3 = (onemeter - temp2) / constant;
                        distancematrix[i, j] = Math.Pow(temp3, 10);
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

            coordinates[0, 0] = 0;
            coordinates[0, 1] = 0;
            coordinates[2, 0] = distancematrix[0, 2];
            coordinates[2, 1] = 0;

            double[] theta;
            theta = new double[5];

            for (int i = 0; i < 5; i++)
            {
                if (i == 0 || i == 2)
                    break;
                else
                {
                    double temp1 = Math.Pow(distancematrix[0, 2], 2) + Math.Pow(distancematrix[0, i], 2) - Math.Pow(distancematrix[2, i], 2);
                    double temp2 = temp1 / (2 * distancematrix[0, 2] * distancematrix[0, 1]);
                    theta[i] = Math.Acos(temp2);

                }
            }

            for (int i = 0; i < 5; i++)
            {
                if (i == 0 || i == 2)
                    break;
                else
                {
                    coordinates[i, 0] = Math.Cos(theta[i]) * distancematrix[0, i];
                    coordinates[i, 1] = Math.Sin(theta[i]) * distancematrix[0, i];
                }
            }
        }
    }
}

