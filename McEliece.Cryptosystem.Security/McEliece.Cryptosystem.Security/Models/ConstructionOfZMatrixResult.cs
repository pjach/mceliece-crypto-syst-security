using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace MIF.VU.PJach.McElieceSecurity.Models
{
    public class ConstructionOfZMatrixResult
    {
        public Matrix<float> ZMatrix;
        public IList<int> LIndicesEquivalent = new List<int>();

        public ConstructionOfZMatrixResult(Matrix<float> zMatrix)
        {
            ZMatrix = zMatrix;
        }
    }
}