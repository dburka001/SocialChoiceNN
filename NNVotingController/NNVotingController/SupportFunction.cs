using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNVotingController
{
    static class SupportFunction
    {
        /// <summary>
        /// Creates an integer distribution for the given parameters
        /// </summary>
        /// <param name="distributionLength"></param>
        /// <param name="distributableValue"></param>
        /// <param name="rnd"></param>
        /// <returns></returns>
        public static int[] CreatePreferenceDistribution(int distributionLength, int distributableValue, Random rnd)
        {
            int[] distribution = new int[distributionLength];
            for (int prefID = 0; prefID < distribution.Length; prefID++)
            {
                distribution[prefID] = 0;
            }
            for (int voterID = 0; voterID < distributableValue; voterID++)
            {
                int tempID = rnd.Next(distribution.Length);
                distribution[tempID]++;
            }
            return distribution;
        }

        /// <summary>
        /// Creates an array of orders from the available orders for a given distribution
        /// </summary>
        /// <returns></returns>
        public static int[][] CreateOrderArray(int[,] availOrders, int[] voterPreferenceDistributon)
        {
            List<int[]> orderList = new List<int[]>();            
            for (int prefID = 0; prefID < voterPreferenceDistributon.Length; prefID++)
            {
                for (int i = 0; i < voterPreferenceDistributon[prefID]; i++)
                {
                    int[] currentOrder = new int[availOrders.GetLength(1)];
                    for (int orderID = 0; orderID < currentOrder.Length; orderID++)
                    {
                        currentOrder[orderID] = availOrders[prefID, orderID] - 1;
                    }
                    orderList.Add(currentOrder);
                }
            }
            return orderList.ToArray();
        }
        /// <summary>
        /// Creates an array of orders from the available orders for a given distribution
        /// </summary>
        /// <returns></returns>
        public static int[][] CreateOrderArray(int[,] availOrders, int[] voterPreferenceDistributon, int[] rndOrder)
        {
            List<int[]> orderList = new List<int[]>();
            for (int prefID = 0; prefID < voterPreferenceDistributon.Length; prefID++)
            {
                for (int i = 0; i < voterPreferenceDistributon[prefID]; i++)
                {
                    int[] currentOrder = new int[availOrders.GetLength(1)];
                    for (int orderID = 0; orderID < currentOrder.Length; orderID++)
                    {
                        currentOrder[orderID] = availOrders[prefID, orderID] - 1;
                    }
                    orderList.Add(currentOrder);
                }
            }
            return rndOrder.Select(i => orderList[i]).ToArray();            
        }

        /// <summary>
        /// Returns an array of arrays with all the possible permutations of the input
        /// </summary>
        /// <param name="nbrOfNominees"></param>
        public static int[,] GetPermutations(int numbers)
        {
            int[] numberArray = new int[numbers];
            for (int i = 0; i < numbers; i++)
            {
                numberArray[i] = i + 1;
            }

            return SupportFunction.GetPermutations(numberArray);
        }
        /// <summary>
        /// Returns an array of arrays with all the possible permutations of the input
        /// </summary>
        /// <param name="nbrOfNominees"></param>
        public static int[,] GetPermutations(int[] numbers)
        {
            int[,] output = new int[SupportFunction.Factorial(numbers.Length), numbers.Length];

            if (numbers.Length == 1)
            {
                output[0, 0] = numbers[0];
            }
            else
            {
                int[] lessNumbers = new int[numbers.Length - 1];
                for (int i = 0; i < numbers.Length - 1; i++)
                {
                    lessNumbers[i] = numbers[i + 1];
                }
                int[,] lessOutput = SupportFunction.GetPermutations(lessNumbers);

                int rowCounter = 0;
                for (int i = 0; i < lessOutput.GetLength(0); i++)
                {
                    // Loop through the positions of the new number
                    for (int j = 0; j < lessOutput.GetLength(1) + 1; j++)
                    {
                        output[rowCounter, j] = numbers[0];
                        // Copy the other numbers in the remaining spaces
                        for (int k = 0; k < lessOutput.GetLength(1); k++)
                        {
                            if (k < j) output[rowCounter, k] = lessOutput[i, k];
                            if (k >= j) output[rowCounter, k + 1] = lessOutput[i, k];
                        }
                        rowCounter++;
                    }
                }
            }            

            return output;
        }
        

        /// <summary>
        /// Calculates factorial of n
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Factorial(int n)
        {
            if (n == 0) return 1;

            int output = 1;
            for (int i = 2; i <= n; i++)
            {
                output *= i;
            }
            return output;
        }

        /// <summary>
        /// Converts binary outputs to summary types
        /// </summary>
        /// <param name="fileName"></param>
        public static void ConvertToSum(string fileName, string fileType, int nbrOfNominees, int nbrOfVoters)
        {
            double[,] data;
            int nbrOfRankingColumns = nbrOfNominees * (nbrOfNominees - 1) / 2;

            // Read from file
            StreamReader sr = new StreamReader(fileName + fileType, Encoding.Default);

            int nbrOfRows = 0;
            while (!sr.EndOfStream) { sr.ReadLine(); nbrOfRows++; }
            sr.BaseStream.Position = 0;
            sr.DiscardBufferedData();

            if (fileType == ".noncond")
            {
                data = new double[nbrOfRows, 2 * nbrOfRankingColumns];    
            }
            else
            {
                data = new double[nbrOfRows, 2 * nbrOfRankingColumns + nbrOfNominees];
            }

            int currentRow = 0;
            while (!sr.EndOfStream) 
            { 
                string[] currentLine = sr.ReadLine().Split(';');
                int counter = 0;
                for (int voterID = 0; voterID < nbrOfVoters; voterID++)
                {                    
                    for (int i = 0; i < nbrOfRankingColumns; i++)
                    {
                        double currentValue = Convert.ToDouble(currentLine[counter]);
                        if (currentValue == 1)
                            data[currentRow, i] += 1;
                        else
                            data[currentRow, nbrOfRankingColumns + i] += 1;
                        counter++;
                    }                    
                }
                if (fileType != ".noncond")
                {
                    for (int i = 0; i < nbrOfNominees; i++)
                    {
                        data[currentRow, 2 * nbrOfRankingColumns + i] = Convert.ToDouble(currentLine[counter]);
                        counter++;
                    }
                }
                currentRow++;
            }

            sr.Close();

            // Finalize data
            for (int i = 0; i < nbrOfRows; i++)
            {
                for (int j = 0; j < 2 * nbrOfRankingColumns; j++)
                {
                    data[i, j] /= nbrOfVoters;
                }
            }

            // Write to file
            StreamWriter sw = new StreamWriter(fileName  + "_SUM" + fileType, false, Encoding.Default);

            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (i != 0) sw.Write("\n");
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (j != 0) sw.Write(";");
                    sw.Write(data[i, j].ToString().Replace(',', '.'));
                }                
            }

            sw.Close();
        }
    }
}
