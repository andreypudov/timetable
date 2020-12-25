// <copyright file="TimetableType.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.Bot.Types
{
    /// <summary>
    /// Represents available types of the timetable.
    /// </summary>
    public enum TimetableType
    {
        /// <summary>
        /// Timetable for weekly classes.
        /// </summary>
        General,

        /// <summary>
        /// Timetable for for examination session.
        /// </summary>
        Session,
    }
}
