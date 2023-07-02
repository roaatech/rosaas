using Audit.Core;
using Audit.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Audit.Net.Custom;
using Roaa.Rosas.Auditing.Configurations;

namespace Roaa.Rosas.Auditing;


public static class AuditConfigurations
{
    public static AuditingConfigurator Config = new AuditingConfigurator();

    /// <summary>
    ///     Configuration and Setup audit log to Store the events in a MySql database.
    /// </summary>
    public static void AddAudit(this IServiceCollection services, Action<IAuditingConfigurator> config)
    {
        AuditingConfigurator auditingConfigurator = new AuditingConfigurator();
        config(auditingConfigurator);
        Config = auditingConfigurator;
        Configuration.Setup().UseCustomMySql(_config => _config
                .ConnectionString(auditingConfigurator.ConnectionString)
                .TableName(auditingConfigurator.TableName)
                .IdColumnName(auditingConfigurator.IdColumnName)
                .JsonColumnName(auditingConfigurator.JsonDataColumnName)
                .CustomColumn(auditingConfigurator.ActionColumnName, ev => ev.EventType)
                .CustomColumn(auditingConfigurator.MethodColumnName, ev => ev.Environment.CallingMethodName)
                .CustomColumn(auditingConfigurator.CreatedDateColumnName, ev => ev.StartDate)
                .CustomColumn(auditingConfigurator.TimeStampColumnName, ev => ev.StartDate.Ticks)
                .CustomColumn(auditingConfigurator.DurationColumnName, ev => ev.Duration));
    }

    public static void AddAudit(this IServiceCollection services, string connectionString)
    {
        services.AddAudit(config => config.SetConnectionString(connectionString)
                                          .SetTableName("rosas_audits"));
    }

    /// <summary>
    ///     Enables audit log with a global Action Filter
    /// </summary>
    public static void AddAudit(this MvcOptions mvcOptions)
    {
        mvcOptions.AddAuditFilter(config => config
            .LogRequestIf(request => !request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase) &&
                                     (request.Headers.Remove("Authorization") ? true : true))
            .WithEventType("{verb}-{controller}/{action}")
            .IncludeHeaders()
            .IncludeRequestBody()
            .IncludeModelState()
            .IncludeResponseBody()
        );
    }


    /// <summary>
    ///     Configures what and how is logged or is not logged, (Configure audit output).
    /// </summary>
    public static void UseAudit(this IApplicationBuilder app)
    {
        app.UseCustomAudit();

    }

}