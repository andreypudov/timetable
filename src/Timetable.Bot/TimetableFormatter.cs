// <copyright file="TimetableFormatter.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.Bot
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using HtmlAgilityPack;

    /// <summary>
    /// Timetable formatter that returns a markdown-formatted representation.
    /// </summary>
    public static class TimetableFormatter
    {
        /// <summary>
        /// Formats a given timetable to markdown representation.
        /// </summary>
        /// <param name="timetable">The HTML representation of the timetable.</param>
        /// <param name="timetableType">The type of the timetable to return.</param>
        /// <returns>The markdown representation of the given timetable.</returns>
        public static string Format(string timetable, TimetableType timetableType)
        {
            switch (timetableType)
            {
                case TimetableType.Session:
                    return FormatSessionTimetable(timetable);
                default:
                    return FormatGeneralTimetable(timetable);
            }
        }

        private static string FormatGeneralTimetable(string timetable)
        {
            var builder = new StringBuilder();
            var document = new HtmlDocument();
            document.LoadHtml(timetable);

            var rows = document.DocumentNode.SelectNodes("(//div[contains(@id, 'schedule-student-container')]//table//tr)");
            var titleAdded = false;

            foreach (var row in rows)
            {
                var columns = row.SelectNodes("td");
                if (columns?.Count == 1)
                {
                    if (titleAdded)
                    {
                        break;
                    }

                    builder.Append($"*{GetInnerTextValue(columns[0]).ToUpperInvariant()}*\n\n");
                    titleAdded = true;
                }
                else if (columns?.Count == 7)
                {
                    builder
                        .Append($"*{GetInnerTextValue(columns[1])}*\n")
                        .Append($"```{GetInnerTextValue(columns[0])}```\n")
                        .Append($"{GetInnerTextValue(columns[3])} {GetTeacherEmoji(GetInnerTextValue(columns[3]))}\n");

                    if (string.IsNullOrEmpty(columns[5].InnerText) == false)
                    {
                        var link = columns[5].SelectNodes("a");
                        if (link?.Count == 1)
                        {
                            builder.Append($"[]{link[0].Attributes["href"].Value}\n");
                        }
                    }

                    if (string.IsNullOrEmpty(columns[6].InnerText) == false)
                    {
                        builder.Append($"\\[{GetInnerTextValue(columns[6])}\\]\n");
                    }

                    builder.Append('\n');
                }
            }

            return EscapeMarkdown(builder);
        }

        private static string FormatSessionTimetable(string timetable)
        {
            var builder = new StringBuilder();
            var document = new HtmlDocument();
            document.LoadHtml(timetable);

            var rows = document.DocumentNode.SelectNodes("(//div[contains(@id, 'schedule-student-container')]//table//tr)");

            foreach (var row in rows)
            {
                var columns = row.SelectNodes("td");
                if (columns?.Count == 1)
                {
                    builder.Append($"*{GetInnerTextValue(columns[(int)SessionTimetableColumns.Time]).ToUpperInvariant()}*\n\n");
                }
                else if (columns?.Count == 5)
                {
                    builder
                        .Append($"*{GetInnerTextValue(columns[(int)SessionTimetableColumns.Class])}*\n")
                        .Append($"```{GetInnerTextValue(columns[(int)SessionTimetableColumns.Time])}```\n")
                        .Append($"{GetInnerTextValue(columns[(int)SessionTimetableColumns.Teacher])} {GetTeacherEmoji(GetInnerTextValue(columns[(int)SessionTimetableColumns.Teacher]))}\n");

                    if (string.IsNullOrEmpty(columns[(int)SessionTimetableColumns.Link].InnerText) == false)
                    {
                        var link = columns[(int)SessionTimetableColumns.Link].SelectNodes("a");
                        if (link?.Count == 1)
                        {
                            builder.Append($"[]{link[0].Attributes["href"].Value}\n");
                        }
                    }

                    if (string.IsNullOrEmpty(columns[(int)SessionTimetableColumns.Description].InnerText) == false)
                    {
                        builder.Append($"\\[{GetInnerTextValue(columns[(int)SessionTimetableColumns.Description])}\\]\n");
                    }

                    builder.Append('\n');
                }
            }

            return EscapeMarkdown(builder);
        }

        private static string EscapeMarkdown(StringBuilder builder)
        {
            return builder
                .ToString()
                .Replace("_", "\\_", StringComparison.InvariantCulture)
                .Replace("<", "\\<", StringComparison.InvariantCulture)
                .Replace(">", "\\>", StringComparison.InvariantCulture)
                .Replace("+", "\\+", StringComparison.InvariantCulture)
                .Replace("-", "\\-", StringComparison.InvariantCulture)
                .Replace(".", "\\.", StringComparison.InvariantCulture)
                .Replace("|", "\\|", StringComparison.InvariantCulture)
                .Replace("(", "\\(", StringComparison.InvariantCulture)
                .Replace(")", "\\)", StringComparison.InvariantCulture)
                .Replace("=", "\\=", StringComparison.InvariantCulture);
        }

        private static string GetInnerTextValue(HtmlNode node)
        {
            return WebUtility.HtmlDecode(node.InnerText.Trim());
        }

        private static string GetTeacherEmoji(string name)
        {
            var map = new Dictionary<string, string>
            {
                { "Дуцев Михаил Викторович", "👨‍🎨" },
                { "Гребенников Александр Валентинович", "🕺" },
                { "Орлова Людмила Николаевна", "👩‍⚖️" },
            };

            return map.GetValueOrDefault(name) ?? string.Empty;
        }
    }
}
