// <copyright file="TimetableParser.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.Bot.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    using Timetable.Bot.Types;

    /// <summary>
    /// Represents the web parser for the source data.
    /// </summary>
    public static class TimetableParser
    {
        private const string GeneralUrl = "https://nngasu.ru/cdb/schedule/student.php?login=yes";
        private const string SesssionUrl = "https://nngasu.ru/cdb/schedule/session-student.php?login=yes";

        /// <summary>
        /// Parsers the web page and returns the string representation of the timetable for the first available day.
        /// </summary>
        /// <param name="login">The login to access the timetable data.</param>
        /// <param name="password">The password to access the timetable data.</param>
        /// <param name="timetableType">The type of the timetable to return.</param>
        /// <returns>The string representation of the timetable for the first available day.</returns>
        public static async Task<string> GetAsync(string login, string password, TimetableType timetableType)
        {
            var baseUrl = (timetableType == TimetableType.General) ? GeneralUrl : SesssionUrl;

            var timetableUrl = string.Empty;
            string timetable = string.Empty;

            using var client = new HttpClient();
            {
                var authHeader = new UTF8Encoding().GetBytes($"{login}:{password}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authHeader));

                using var response = await client.GetAsync(new Uri(baseUrl));
                if (response.IsSuccessStatusCode)
                {
                    var buffer = await response.Content.ReadAsByteArrayAsync();
                    var encoding = Encoding.GetEncoding("windows-1251");
                    var responseString = encoding.GetString(buffer, 0, buffer.Length);

                    var document = new HtmlDocument();
                    document.LoadHtml(responseString);

                    var timetableIFrame = document.DocumentNode.SelectNodes("(//iframe[@id='diploma-iframe'])");
                    if (timetableIFrame.Count > 0)
                    {
                        timetableUrl = timetableIFrame[0].Attributes["src"].Value;
                    }
                }

                using var message2 = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(timetableUrl),
                };
                using var response2 = await client.SendAsync(message2);
                if (response2.IsSuccessStatusCode)
                {
                    timetable = await response2.Content.ReadAsStringAsync();
                }
            }

            return timetable;
        }
    }
}
