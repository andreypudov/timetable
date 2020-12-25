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
        private const string GeneralUrl = "http://nngasu.ru/cdb/schedule/student.php?login=yes";
        private const string SesssionUrl = "http://nngasu.ru/cdb/schedule/session-student.php?login=yes";

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

            var formVariables = new List<KeyValuePair<string?, string?>>
            {
                new KeyValuePair<string?, string?>("AUTH_FORM", "Y"),
                new KeyValuePair<string?, string?>("TYPE", "AUTH"),
                new KeyValuePair<string?, string?>("backurl", "/cdb/schedule/student.php"),
                new KeyValuePair<string?, string?>("USER_LOGIN", login),
                new KeyValuePair<string?, string?>("USER_PASSWORD", password),
                new KeyValuePair<string?, string?>("Login", "%C2%EE%E9%F2%E8"),
            };

            var cookieContainer = new CookieContainer();
            var timetableUrl = string.Empty;
            var formContent = new FormUrlEncodedContent(formVariables);

            string timetable = string.Empty;

            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using var client = new HttpClient(handler, false);
                {
                    using var message = new HttpRequestMessage { Method = HttpMethod.Post, RequestUri = new Uri(baseUrl), Content = formContent };
                    using var response = await client.SendAsync(message);
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
            }

            return timetable;
        }
    }
}
