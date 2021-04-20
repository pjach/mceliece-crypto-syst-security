using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace MIF.VU.PJach.McElieceSecurity.Models
{
    internal class ZMatrixRowsAndTheirIndices
    {
        public Vector<float> ZMatrixRow;
        public IList<int> ZMatrixRowsIndices;
    }
}