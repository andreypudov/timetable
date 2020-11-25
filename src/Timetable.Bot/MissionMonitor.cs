// <copyright file="MissionMonitor.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.Bot
{
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the mission monitor.
    /// </summary>
    public class MissionMonitor
    {
        private static readonly string BotApiKey = "1207628089:AAF-ytVyTJsmQ-5y_XTL-ZUUrtIU4LGfnzo";
        private static readonly string ChannelName = "-1001232512421";

        /// <summary>
        /// Publishes provided message to the Message monitor channel.
        /// </summary>
        /// <param name="message">The value of the message to publish.</param>
        /// <returns>The task handling the HTTP publishing request.</returns>
        public static async Task<string> Publish(string message)
        {
            var url = $"https://api.telegram.org/bot{BotApiKey}/sendMessage?chat_id={ChannelName}&text={WebUtility.HtmlEncode(message)}";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}