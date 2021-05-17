using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MIF.VU.PJach.McElieceSecurity.Utilities.Helpers
{
    public static class RandomnessHelper
    {
        public static Vector<float> GenerateRandomVector(int length, Randomizer randomizer)
        {
            var vector = Vector<float>.Build.Dense(length, 0);

            for (int i = 0; i < length; i++)
            {
                vector[i] = randomizer.Next(0, 2);
            }

            return vector;
        }

        public static Vector<float> GenerateErrorVector(int length, int amountOfErrors, Randomizer randomizer)
        {
            var vector = Vector<float>.Build.Dense(length, 0);

            for (int i = 0; i < amountOfErrors; i++)
            {
                int randomIndex = randomizer.Next(0, length - 1);
                if (vector[randomIndex] != 0)
                {
                    i--;
                }
                vector[randomIndex] = 1;
            }

            return vector;
        }

        public static IList<int> GenerateKnumberOfRandomColumnsIndices(int k, IList<int> L0Set, Randomizer randomizer)
        {
            var columnIndices = new List<int>();
            var usedColumnsIndices = new ArrayList();

            for (int i = 0; i < k; i++)
            {
                try
                {
                    int randomIndex = randomizer.Next(0, L0Set.Count - 1);
                    if (usedColumnsIndices.Contains(randomIndex))
                    {
                        i--;
                    }
                    else
                    {
                        columnIndices.Add(L0Set[randomIndex]);
                        usedColumnsIndices.Add(randomIndex);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    i--;
                }
            }

            return columnIndices;
        }

        public static int GenerateRandomIndex(IList<int> list, Randomizer randomizer)
        {
            return randomizer.Next(list.Count);
        }

        public static int GenerateRandomIndexForFLWC(IList<int> indices, int maxValue, Randomizer randomizer)
        {
            while (true)
            {
                try
                {
                    int randomIndex = randomizer.Next(0, maxValue);
                    if (!indices.Contains(randomIndex))
                    {
                        return randomIndex;
                    }
                }
                catch (ArgumentOutOfRangeException) { }
            }
        }

        public static IList<int> GenerateRandomIndices(int maxValue, int numberOfElements, Randomizer randomizer)
        {
            var randomIndicesList = new List<int>();

            for (int i = 0; i < numberOfElements; i++)
            {
                try
                {
                    int randomIndex = randomizer.Next(0, maxValue);
                    if (randomIndicesList.Contains(randomIndex))
                    {
                        i--;
                    }
                    else
                    {
                        randomIndicesList.Add(randomIndex);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    i--;
                }
            }

            return randomIndicesList;
        }
    }
}