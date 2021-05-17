using MIF.VU.PJach.McElieceSecurity.Contracts;
using MIF.VU.PJach.McElieceSecurity.Statistics;
using MIF.VU.PJach.McElieceSecurity.Utilities.IO;
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

            string flwcAttackData = fileReader.ReadFromFile(ConfigurationManager
                            .AppSettings["flwc_statistics"]);

            string relatedAttackData = fileReader.ReadFromFile(ConfigurationManager
                .AppSettings["related_statistics"]);

            string resendAttackData = fileReader.ReadFromFile(ConfigurationManager
                            .AppSettings["resend_statistics"]);

            IStatistics statistics = new FlwcStatistics(_logger);

            _logger.Debug("Finding low-weight codeword mattack statistics");
            statistics.PrintStatistics(flwcAttackData);

            statistics = new RelatedMessageStatistics(_logger);

            _logger.Debug("Failure under related message attack statistics");
            statistics.PrintStatistics(relatedAttackData);

            _logger.Debug("Failure under resend message attack statistics");
            statistics.PrintStatistics(resendAttackData);
        }
    }
}