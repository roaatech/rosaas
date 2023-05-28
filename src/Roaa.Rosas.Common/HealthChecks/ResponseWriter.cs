/*
 * 
 *  I took this file from :
 *  https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.UI.Client/UIResponseWriter.cs
 *  
 *  All credits goes to the owner of this open source project
 *  I had to copy the file directly instead of using the nuget package becuase latest version 5.1 is not compatible with .net core 3.1
 *  and I don't want to use a previous version becuase CheckHealth UI in separated microservice is 5.1 
 * 
 * 
 */

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Roaa.Rosas.Common.HealthChecks.UI
{
    public static class ResponseWriter
    {
        const string DEFAULT_CONTENT_TYPE = "application/json";

        private static byte[] emptyResponse = new byte[] { (byte)'{', (byte)'}' };
        private static Lazy<JsonSerializerOptions> options = new Lazy<JsonSerializerOptions>(() => CreateJsonOptions());

        public static async Task WriteHealthCheckUIResponse(HttpContext httpContext, HealthReport report)
        {
            if (report != null)
            {
                httpContext.Response.ContentType = DEFAULT_CONTENT_TYPE;

                var uiReport = UIHealthReport
                    .CreateFrom(report);

                using var responseStream = new MemoryStream();

                await JsonSerializer.SerializeAsync(responseStream, uiReport, options.Value);
                await httpContext.Response.BodyWriter.WriteAsync(responseStream.ToArray());
            }
            else
            {
                await httpContext.Response.BodyWriter.WriteAsync(emptyResponse);
            }
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true,
            };

            options.Converters.Add(new JsonStringEnumConverter());

            //for compatibility with older UI versions ( <3.0 ) we arrange
            //timespan serialization as s
            options.Converters.Add(new TimeSpanConverter());

            return options;
        }
    }

    internal class TimeSpanConverter
        : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return default;
        }
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
