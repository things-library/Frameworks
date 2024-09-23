// ================================================================================
// <copyright file="Schedule.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType
{
    [DebuggerDisplay("Cron = {Cron}, TTL = {TTL}")]
    public class Schedule
    {
        /// <summary>
        /// Time To Live (TTL)
        /// </summary>
        public TimeSpan? TTL { get; set; }

        /// <summary>
        /// CRON Schedule string
        /// </summary>
        public string Cron { get; set; } = string.Empty;


        public override string ToString()
        {
            if(this.TTL != null)
            {
                return this.TTL.ToString()!;
            }
            else
            {
                return this.Cron;
            }
        }
    }
}
