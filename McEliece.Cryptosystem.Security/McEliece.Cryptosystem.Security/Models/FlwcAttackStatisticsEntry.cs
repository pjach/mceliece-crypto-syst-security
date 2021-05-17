namespace MIF.VU.PJach.McElieceSecurity.Models
{
    public class FlwcAttackStatisticsEntry
    {
        public long SpentTime { get; set; }
        public long InitialGaussianEliminationTime { get; set; }
        public int TotalIterationsCount { get; set; }
        public long FinalGaussianEliminationTime { get; set; }
    }
}