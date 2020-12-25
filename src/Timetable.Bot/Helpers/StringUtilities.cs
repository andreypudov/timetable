// <copyright file="StringUtilities.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.Bot.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;

    /// <summary>
    /// Provides a set of utility methods.
    /// </summary>
    public static class StringUtilities
    {
        /// <summary>
        /// Returns the string with replaced sequences.
        /// </summary>
        /// <param name="builder">The instance of the string builder.</param>
        /// <returns>The string with escaped sequences.</returns>
        public static string EscapeMarkdown(StringBuilder builder)
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

        /// <summary>
        /// Returns the value of the first occurance of the date.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>The date value of matched sequence.</returns>
        public static DateTime? GetDateFromString(string value)
        {
            var regex = new Regex(@"\b\d{2}\.\d{2}.\d{4}\b");

            foreach (Match? match in regex.Matches(value))
            {
                if ((match != null)
                    && DateTime.TryParseExact(match.Value, "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the value of the inner text.
        /// </summary>
        /// <param name="node">The HTML node to use.</param>
        /// <returns>Th value of the inner text.</returns>
        public static string GetInnerTextValue(HtmlNode node)
        {
            return WebUtility.HtmlDecode(node.InnerText.Trim());
        }

        /// <summary>
        /// Returns the emoji for the given name of the teacher.
        /// </summary>
        /// <param name="name">The name of the teacher.</param>
        /// <returns>The string vith the emoji.</returns>
        public static string GetTeacherEmoji(string name)
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
