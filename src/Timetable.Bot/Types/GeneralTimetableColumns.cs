// <copyright file="GeneralTimetableColumns.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.Bot.Types
{
    /// <summary>
    /// Represents the index values for the columns of the session timetable.
    /// </summary>
    internal enum GeneralTimetableColumns
    {
        /// <summary>
        /// The time of the class.
        /// </summary>
        Time = 0,

        /// <summary>
        /// The name of the class.
        /// </summary>
        Class = 1,

        /// <summary>
        /// The name of the teacher.
        /// </summary>
        Teacher = 3,

        /// <summary>
        /// The link to the class.
        /// </summary>
        Link = 5,

        /// <summary>
        /// The value of the description.
        /// </summary>
        Description = 6,
    }
}
