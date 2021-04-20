using MIF.VU.PJach.McElieceSecurity.AttacksExecutors;
using MIF.VU.PJach.McElieceSecurity.Contracts;
using MIF.VU.PJach.McElieceSecurity.Models;
using MIF.VU.PJach.McElieceSecurity.Statistics;
using MIF.VU.PJach.McElieceSecurity.Utilities;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

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


            //var gisdAttack = new GisdAttackExecutor(fileWriter, fileReader, _logger, false);
            //gisdAttack.ExecuteAttack(100000, 1, 2);

            var gisdAttackData = fileReader.ReadFromFile(ConfigurationManager.AppSettings["gisd_statistics"]);
            IStatistics statistics = new GisdStatistics(_logger);
            statistics.PrintStatistics(gisdAttackData);


            string relatedAttackData = fileReader.ReadFromFile(ConfigurationManager
                            .AppSettings["related_statistics"]);
            string resendAttackData = fileReader.ReadFromFile(ConfigurationManager
                            .AppSettings["resend_statistics"]);

            IStatistics resendStatistics = new RelatedMessageStatistics(_logger);

            _logger.Debug("Failure under related message attack statistics");
            resendStatistics.PrintStatistics(relatedAttackData);

            _logger.Debug("Failure under resend message attack statistics");
            resendStatistics.PrintStatistics(resendAttackData);
        }
    }
}