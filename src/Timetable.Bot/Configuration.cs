// <copyright file="Configuration.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.Bot
{
    /// <summary>
    /// Represents application configuration.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets or sets authentication token.
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Gets or sets user name to access timetable information.
        /// </summary>
        public string? Login { get; set; }

        /// <summary>
        /// Gets or sets password to access timetable information.
        /// </summary>
        public string? Password { get; set; }
    }
}
