﻿using System.Collections.Generic;

namespace MIF.VU.PJach.McElieceSecurity.Models
{
    public class RelatedAttackStatisticsEntry
    {
        public RelatedAttackStatisticsEntry()
        {
            GuessesUntilErrorFreeColumnsSelected = new List<int>();
        }

        public int TotalIterationsCount { get; set; }
        public int NumberOfSuccessfulGaussianEliminations { get; set; }
        public long SpentTime { get; set; }
        public int L0SetCount { get; set; }
        public int L1SetCount { get; set; }
        public IList<int> GuessesUntilErrorFreeColumnsSelected { get; set; }
    }
}