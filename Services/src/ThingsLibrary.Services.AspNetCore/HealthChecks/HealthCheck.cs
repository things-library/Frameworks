// ================================================================================
// <copyright file="HealthCheck.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ThingsLibrary.Services.AspNetCore.HealthChecks
{
    /// <summary>
    /// Health Check utilities
    /// </summary>
    public static class HealthCheck
    {
        /// <summary>
        /// Get the health report as a json string
        /// </summary>
        /// <param name="healthReport">Health Report</param>
        /// <returns>Json version of the <see cref="HealthReport"/></returns>
        public static string GetHealthJson(HealthReport healthReport)
        {
            //App.Service.JsonWriterOpt
            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();
                    writer.WriteString("status", healthReport.Status.ToString());
                    writer.WriteStartObject("results");
                    foreach (var entry in healthReport.Entries)
                    {
                        writer.WriteStartObject(entry.Key);
                        writer.WriteString("status", entry.Value.Status.ToString());
                        writer.WriteString("description", entry.Value.Description);
                        writer.WriteStartObject("data");
                        foreach (var item in entry.Value.Data)
                        {
                            writer.WritePropertyName(item.Key);
                            JsonSerializer.Serialize(writer, item.Value, item.Value?.GetType() ?? typeof(object));
                        }
                        writer.WriteEndObject();
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                return System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Json writer for HealthReport objects
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="healthReport">Health Report</param>
        /// <returns>Writes health report to response buffer</returns>
        public static Task Writer(HttpContext context, HealthReport healthReport)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var json = HealthCheck.GetHealthJson(healthReport);

            return context.Response.WriteAsync(json);
        }
    }
}