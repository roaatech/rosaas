/*
 * 
 *  I took this file from :
 *  https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.UI.Core/UIHealthReport.cs
 *  
 *  All credits goes to the owner of this open source project
 *  I had to copy the file directly instead of using the nuget package becuase latest version 5.1 is not compatible with .net core 3.1
 *  and I don't want to use a previous version becuase CheckHealth UI in separated microservice is 5.1 
 * 
 * 
 */

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Roaa.Rosas.Common.HealthChecks.UI
{
    /*
     * Models for UI Client. This models represent a indirection between HealthChecks API and 
     * UI Client in order to implement some features not present on HealthChecks of substitute 
     * some properties etc.
     */
    public class UIHealthReport
    {
        public HealthStatus Status { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public Dictionary<string, HealthReportEntry> Entries { get; }

        public UIHealthReport(Dictionary<string, HealthReportEntry> entries, TimeSpan totalDuration)
        {
            Entries = entries;
            TotalDuration = totalDuration;
        }
        public static UIHealthReport CreateFrom(HealthReport report)
        {
            var uiReport = new UIHealthReport(new Dictionary<string, HealthReportEntry>(), report.TotalDuration)
            {
                Status = (HealthStatus)report.Status,
            };

            foreach (var item in report.Entries)
            {
                var entry = new HealthReportEntry
                {
                    Data = item.Value.Data,
                    Description = item.Value.Description,
                    Duration = item.Value.Duration,
                    Status = (HealthStatus)item.Value.Status
                };

                if (item.Value.Exception != null)
                {
                    var message = item.Value.Exception?
                        .Message
                        .ToString();

                    entry.Exception = message;
                    entry.Description = item.Value.Description ?? message;
                }

                entry.Tags = item.Value.Tags;

                uiReport.Entries.Add(item.Key, entry);
            }

            return uiReport;
        }
        public static UIHealthReport CreateFrom(Exception exception, string entryName = "Endpoint")
        {
            var uiReport = new UIHealthReport(new Dictionary<string, HealthReportEntry>(), TimeSpan.FromSeconds(0))
            {
                Status = HealthStatus.Unhealthy,
            };

            uiReport.Entries.Add(entryName, new HealthReportEntry
            {
                Exception = exception.Message,
                Description = exception.Message,
                Duration = TimeSpan.FromSeconds(0),
                Status = HealthStatus.Unhealthy
            });

            return uiReport;
        }
    }
    public enum HealthStatus
    {
        Unhealthy = 0,
        Degraded = 1,
        Healthy = 2
    }
    public class HealthReportEntry
    {
        public IReadOnlyDictionary<string, object> Data { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public string Exception { get; set; }
        public HealthStatus Status { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}