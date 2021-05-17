using MathNet.Numerics.LinearAlgebra;
using MIF.VU.PJach.McElieceSecurity.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MIF.VU.PJach.McElieceSecurity.Utilities.Helpers
{
    public static class CalculationHelper
    {
        public static ColumnAndRowIndices GetIndexOfApplicableMatrixColumn(Matrix<float> publicMatrix, IList<int> informationSet, IList<int> zSet,
            Randomizer randomizer)
        {
            var randomIndexFromZSet = RandomnessHelper.GenerateRandomIndex(zSet, randomizer);
            while (true)
            {
                if (VectorHelper.IsZeroVector(publicMatrix.Column(zSet[randomIndexFromZSet])))
                {
                    randomIndexFromZSet = RandomnessHelper.GenerateRandomIndex(zSet, randomizer);
                }
                else
                {
                    break;
                }
            }
            var randomIndexForInformationSet = VectorHelper.GetNonZeroIndexFromVector(
                publicMatrix.Column(zSet[randomIndexFromZSet]), randomizer);

            var valueForChange = informationSet[randomIndexForInformationSet];
            informationSet[randomIndexForInformationSet] = zSet[randomIndexFromZSet];
            zSet[randomIndexFromZSet] = valueForChange;

            return new ColumnAndRowIndices()
            {
                ColumnIndex = informationSet[randomIndexForInformationSet],
                RowIndex = randomIndexForInformationSet
            };
        }

        public static string GetHashTableKeyForFLWC(IList<int> indices, Vector<float> vector)
        {
            return string.Join("", indices.Select(index => vector[index]));
        }

        public static Vector<float> MultipyMatrixWithVector(Matrix<float> matrix, Vector<float> vector)
        {
            var finalVectorSize = matrix.Row(0).Count;
            var finalVector = Vector<float>.Build.Dense(finalVectorSize);

            for (int i = 0; i < finalVectorSize; i++)
            {
                var temporaryVector = matrix.Column(i);
                float sum = 0;
                for (int j = 0; j < temporaryVector.Count; j++)
                {
                    sum = AddBinaryNumbers(sum, temporaryVector[j] * vector[j]);
                }

                finalVector[i] = sum;
            }

            return finalVector;
        }

        public static float AddBinaryNumbers(float firstNumber, float secondNumber)
        {
            return (firstNumber + secondNumber) % 2;
        }

        public static void CalculateLocationSets(IList L0Set, IList L1Set, Vector<float> firstCipheredVector,
                                                                          Vector<float> secondCipheredVector)
        {
            for (int i = 0; i < firstCipheredVector.Count; i++)
            {
                if (AddBinaryNumbers(firstCipheredVector[i], secondCipheredVector[i]).Equals(0))
                {
                    L0Set.Add(i);
                }
                else
                {
                    L1Set.Add(i);
                }
            }
        }

        public static bool AreThereAnyGarbledIndices(IList<int> randomIndices, Vector<float> errorVector1, Vector<float> errorVector2)
        {
            foreach (int index in randomIndices)
            {
                if (errorVector1[index] == 1 || errorVector2[index] == 1) return true;
            }

            return false;
        }

        public static IList<int> CalculateZSet(IList<int> informationSet, int sizeOfNSet)
        {
            var zSet = new List<int>();

            for (int i = 0; i < sizeOfNSet; i++)
            {
                if (!informationSet.Contains(i))
                {
                    zSet.Add(i);
                }
            }

            return zSet;
        }
    }
}