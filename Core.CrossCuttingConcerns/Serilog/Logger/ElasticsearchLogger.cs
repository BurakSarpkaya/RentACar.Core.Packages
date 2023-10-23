using Core.CrossCuttingConcerns.Serilog.ConfigurationModels;
using Core.CrossCuttingConcerns.Serilog.Messages;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace Core.CrossCuttingConcerns.Serilog.Logger
{
    public class ElasticsearchLogger : LoggerServiceBase
    {
        public ElasticsearchLogger(IConfiguration configuration)
        {
            ElasticsearchConfiguration logConfiguration =
                configuration.GetSection("Serilog:ElasticsearchConfiguration").Get<ElasticsearchConfiguration>()
                ?? throw new Exception(SerilogMessages.NullOptionsMessage);

            global::Serilog.Core.Logger seriLogConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Async(writeTo => writeTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(logConfiguration.Url))
                {
                    TypeName = null,
                    AutoRegisterTemplate = true,
                    IndexFormat = $"{logConfiguration.AppName.ToLower()}-{DateTime.UtcNow:yyyy-MM}",
                    BatchAction = ElasticOpType.Create,
                    CustomFormatter = new ElasticsearchJsonFormatter(),
                    OverwriteTemplate = true,
                    DetectElasticsearchVersion = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    NumberOfReplicas = 1,
                    NumberOfShards = 2,
                    FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                                               EmitEventFailureHandling.WriteToFailureSink |
                                                               EmitEventFailureHandling.RaiseCallback |
                                                               EmitEventFailureHandling.ThrowException
                }))
               .CreateLogger();

            Logger = seriLogConfig;
        }
    }
}
