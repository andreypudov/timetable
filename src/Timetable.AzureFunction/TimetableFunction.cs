// <copyright file="TimetableFunction.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.AzureFunction
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Timetable.Bot;

    /// <summary>
    /// Represents the Azure function processes requests from a Telegram client.
    /// </summary>
    public static class TimetableFunction
    {
        /// <summary>
        /// Azure HTTP trigger function.
        /// </summary>
        /// <param name="request">The HTTP request from the client.</param>
        /// <param name="logger">The instance of the logger.</param>
        /// <param name="context">The applicaiton context.</param>
        /// <returns>The status of the request processing.</returns>
        [FunctionName(nameof(TimetableFunction))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest request,
            ILogger logger,
            ExecutionContext context)
        {
            logger.LogInformation($"{nameof(TimetableFunction)} HTTP trigger function processed a request.");

            var body = await request.ReadAsStringAsync().ConfigureAwait(false);
            if (string.IsNullOrEmpty(body))
            {
                return new BadRequestResult();
            }

            var configuration = GetConfiguration(context);
            if (configuration == null)
            {
                logger.LogError($"{nameof(TimetableFunction)} Could not read configuration.");
                return new BadRequestResult();
            }

            var client = new WebhookClient(configuration, logger);
            await client.Update(body).ConfigureAwait(false);

            return new OkResult();
        }

        private static Configuration? GetConfiguration(ExecutionContext context)
        {
            var configuration = new Configuration();
            var builder = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Bind(configuration);

            return configuration;
        }
    }
}