using MIF.VU.PJach.McElieceSecurity.AttacksExecutors;
using MIF.VU.PJach.McElieceSecurity.Contracts;
using MIF.VU.PJach.McElieceSecurity.Statistics;
using MIF.VU.PJach.McElieceSecurity.Utilities;
using Serilog;
using System.Configuration;

namespace MIF.VU.PJach.McElieceSecurity
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var _logger = new LoggerConfiguration()
                    .ReadFrom.AppSettings()
                    .CreateLogger();

            IFileReader fileReader = new FileReader();
            IFileWriter fileWriter = new FileWriter();
            string relatedAttackData = fileReader.ReadFromFile(ConfigurationManager
                            .AppSettings["related_statistics"]);
            string resendAttackData = fileReader.ReadFromFile(ConfigurationManager
                            .AppSettings["resend_statistics"]);

            var statistics = new RelatedMessageStatistics(_logger);

            _logger.Debug("Failure under related message attack statistics");
            statistics.CalculateAndPrintStatistics(relatedAttackData);

            _logger.Debug("Failure under resend message attack statistics");
            statistics.CalculateAndPrintStatistics(resendAttackData);

            var attackExecutor = new RelatedMessageAttackExecutor(fileWriter, fileReader, _logger);
            attackExecutor.ExecuteBothAttacks(1);
        }
    }
}