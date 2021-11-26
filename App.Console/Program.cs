using System;
using System.IO;
using System.Threading.Tasks;
using App.Core.Models.Configuration;
using App.Core.Models.Options;
using App.Core.Services.Configuration;
using App.Services;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace App.Console
{
    class Program
    {
        private static readonly IServiceProvider ServiceProvider;
        private static readonly IConfigurationRoot Configuration;
        private static ILogger<Program> _logger;

        static Program()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Configuration"))
                .AddJsonFile("file.json", optional: false, reloadOnChange: false)
                //.AddJsonFile($"file.{environmentName}.json", optional: true, reloadOnChange: false)
                .AddJsonFile("search.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var fileSettings = Configuration.GetSection(nameof(FileSettings)).Get<FileSettings>();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(fileSettings.LogDirectory, $"{DateTime.Now:yyyyMMdd_HHmmss}.log"))   // log to file system
                .WriteTo.Console()                                                                              // log to console
                .CreateLogger();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // logging
            services.AddLogging(configure => configure.AddSerilog());

            // configuration
            services.Configure<FileSettings>(options => Configuration.GetSection(nameof(FileSettings)).Bind(options));
            services.AddSingleton<IFileSettingsService, FileSettingsService>();
            
            services.Configure<SearchSettings>(options => Configuration.GetSection(nameof(SearchSettings)).Bind(options));
            services.AddSingleton<ISearchSettingsService, SearchSettingsService>();

            // services
            services.AddSingleton<IProcessService, ProcessService>();
        }

        /// <summary>
        /// App entry point. Use CommandLineParser to configure any command line arguments.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            _logger = ServiceProvider.GetService<ILogger<Program>>();

            await Parser.Default.ParseArguments<RunOptions>(args)
                .WithParsedAsync(RunAsync);
        }

        /// <summary>
        /// Run on 'run' verb, e.g. App.Console.exe run
        /// </summary>
        /// <returns></returns>
        private static async Task RunAsync(RunOptions options)
        {
            var processSvc = ServiceProvider.GetService<IProcessService>();

            try
            {
                await processSvc.Process(options);
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Failed to run process");

                throw;
            }
        }
    }
}
