// <copyright file="TimetableFormatter.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.Bot.Helpers
{
    using System;
    using System.Text;
    using HtmlAgilityPack;
    using Timetable.Bot.Types;

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

                    builder.Append($"*{StringUtilities.GetInnerTextValue(columns[(int)GeneralTimetableColumns.Time]).ToUpperInvariant()}*\n\n");
                    titleAdded = true;
                }
                else if (columns?.Count == 7)
                {
                    builder
                        .Append($"*{StringUtilities.GetInnerTextValue(columns[(int)GeneralTimetableColumns.Class])}*\n")
                        .Append($"```{StringUtilities.GetInnerTextValue(columns[(int)GeneralTimetableColumns.Time])}```\n")
                        .Append($"{StringUtilities.GetInnerTextValue(columns[(int)GeneralTimetableColumns.Teacher])} {StringUtilities.GetTeacherEmoji(StringUtilities.GetInnerTextValue(columns[(int)GeneralTimetableColumns.Teacher]))}\n");

                    if (string.IsNullOrEmpty(columns[5].InnerText) == false)
                    {
                        var link = columns[(int)GeneralTimetableColumns.Link].SelectNodes("a");
                        if (link?.Count == 1)
                        {
                            builder.Append($"[]{link[0].Attributes["href"].Value}\n");
                        }
                    }

                    if (string.IsNullOrEmpty(columns[(int)GeneralTimetableColumns.Description].InnerText) == false)
                    {
                        builder.Append($"\\[{StringUtilities.GetInnerTextValue(columns[(int)GeneralTimetableColumns.Description])}\\]\n");
                    }

                    builder.Append('\n');
                }
            }

            return StringUtilities.EscapeMarkdown(builder);
        }

        private static string FormatSessionTimetable(string timetable)
        {
            var builder = new StringBuilder();
            var document = new HtmlDocument();
            document.LoadHtml(timetable);

            var rows = document.DocumentNode.SelectNodes("(//div[contains(@id, 'schedule-student-container')]//table//tr)");
            var dayToPass = false;

            foreach (var row in rows)
            {
                var columns = row.SelectNodes("td");
                if (columns?.Count == 1)
                {
                    var title = StringUtilities.GetInnerTextValue(columns[(int)SessionTimetableColumns.Time])
                        .ToUpperInvariant();
                    var date = StringUtilities.GetDateFromString(title);
                    if (date < DateTime.Today)
                    {
                        dayToPass = true;
                        continue;
                    }

                    builder.Append($"*{title}*\n\n");
                    dayToPass = false;
                }
                else if (columns?.Count == 5 && dayToPass == false)
                {
                    builder
                        .Append($"*{StringUtilities.GetInnerTextValue(columns[(int)SessionTimetableColumns.Class])}*\n")
                        .Append($"```{StringUtilities.GetInnerTextValue(columns[(int)SessionTimetableColumns.Time])}```\n")
                        .Append($"{StringUtilities.GetInnerTextValue(columns[(int)SessionTimetableColumns.Teacher])} {StringUtilities.GetTeacherEmoji(StringUtilities.GetInnerTextValue(columns[(int)SessionTimetableColumns.Teacher]))}\n");

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
                        builder.Append($"\\[{StringUtilities.GetInnerTextValue(columns[(int)SessionTimetableColumns.Description])}\\]\n");
                    }

                    builder.Append('\n');
                }
            }

            return StringUtilities.EscapeMarkdown(builder);
        }
    }
}
