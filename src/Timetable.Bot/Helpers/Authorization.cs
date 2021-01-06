// <copyright file="Authorization.cs" company="Andrey Pudov">
//     Copyright (c) Andrey Pudov. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Timetable.Bot.Helpers
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides helper methods for authorization.
    /// </summary>
    public static class Authorization
    {
        /// <summary>
        /// Determines the access for provided user name.
        /// </summary>
        /// <param name="username">The vaue of the user name.</param>
        /// <returns><c>true</c> if provied user name is able to get access, and <c>false</c> otherwise.</returns>
        public static bool IsAuthorized(string username)
        {
            var denied = new List<string> { "ежик в кедах" };

            return denied.Contains(username) == false;
        }
    }
}
