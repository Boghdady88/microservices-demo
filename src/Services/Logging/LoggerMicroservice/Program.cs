using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LoggerMicroservice
{
    public class Program
    {
        private static IWebHost BuildWebHost(string[] args, IConfiguration configuration) => WebHost.CreateDefaultBuilder(args).CaptureStartupErrors(false)
        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
        .UseStartup<Startup>()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseSerilog()
        .Build();

        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);
            try
            {
                var host = BuildWebHost(args, configuration);

                Log.Information("Starting host...");

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }



        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            // If directory does not exist, create it
            var logPath = configuration["Serilog:LogPath"];
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logstashUrl = configuration["Serilog:LogstashgUrl"];

            return new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.WithProperty("IdentityServer", "Identity")
            .Enrich.FromLogContext()
            .WriteTo.File($"{logPath}/log.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.Elasticsearch().WriteTo.Elasticsearch(ConfigureElasticSink(configuration, "Development"))
            .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
            .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        }



        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfiguration configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["Serilog:ElasticConfiguration"]))
            {
                BufferCleanPayload = (failingEvent, statuscode, exception) =>
                {
                    dynamic e = JObject.Parse(failingEvent);
                    return JsonConvert.SerializeObject(new Dictionary<string, object>()
                    {
                        {"@timestamp",e["@timestamp"]},
                        {"level","Error"},
                        {"message","Error: "+e.message},
                        {"messageTemplate",e.messageTemplate},
                        {"failingStatusCode", statuscode},
                        {"failingException", exception}
                    });
                },
                MinimumLogEventLevel = LogEventLevel.Verbose,
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
            EmitEventFailureHandling.WriteToFailureSink |
            EmitEventFailureHandling.RaiseCallback
            };
        }



        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });




        private static IConfiguration GetConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Machine);
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
