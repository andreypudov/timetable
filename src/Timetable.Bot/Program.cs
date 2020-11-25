// <copyright file="Program.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.Bot
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NLog;
    using NLog.Extensions.Logging;

    /// <summary>
    /// An entry point of the application.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Configures the application's services.
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        /// <param name="configuration">The instance of a applicaiton configuration.</param>
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var botConfiguration = new Configuration();
            configuration.Bind(botConfiguration);

            services
                .AddTransient<LongPollingClient>()
                .AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    builder.AddNLog(configuration);
                })
                .AddSingleton(botConfiguration);
        }

        /// <summary>
        /// An entry point method of the application.
        /// </summary>
        /* <param name="args">An array of command-line arguments.</param> */
        private static void Main(/* string[] args */)
        {
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection, configuration);

                var serviceProvider = serviceCollection.BuildServiceProvider();
                using (serviceProvider)
                {
                    var bot = serviceProvider.GetService<LongPollingClient>();
                    bot?.Run();
                }
            }
            catch (Exception e)
            {
                // NLog: catch any exception and log it.
                logger.Error(e, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }
    }
}