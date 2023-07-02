using Audit.Core;
using Audit.Core.ConfigurationApi;
using Audit.MySql.Configuration;
using Audit.NET.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Auditing.Configurations;
using Roaa.Rosas.Auditing.Helpers;

namespace Roaa.Rosas.Audit.Net.Custom;

public static class CustomMySqlServerConfiguratorExtensions
{
    /// <summary>
    /// Store the events in a MySQL database.
    /// </summary>
    /// <param name="configurator">The configurator object.</param>
    /// <param name="connectionString">The MySQL connection string.</param>
    /// <param name="tableName">The MySQL table name to store the events.</param>
    /// <param name="idColumnName">The primary key column name.</param>
    /// <param name="jsonColumnName">The column name where to store the json data.</param>
    /// <param name="customColumns">The extra custom columns.</param>
    public static ICreationPolicyConfigurator UseCustomMySql(this IConfigurator configurator, string connectionString,
        string tableName, string idColumnName,
        string jsonColumnName, List<CustomColumn> customColumns = null)
    {
        Configuration.DataProvider = new CustomMySqlDataProvider()
        {
            ConnectionString = connectionString,
            TableName = tableName,
            IdColumnName = idColumnName,
            JsonColumnName = jsonColumnName,
            CustomColumns = customColumns
        };
        return new CreationPolicyConfigurator();
    }





    /// <summary>
    /// Store the events in a MySQL database.
    /// </summary>
    /// <param name="configurator">The configurator object.</param>
    /// <param name="config">The MySQL provider configuration.</param>
    public static ICreationPolicyConfigurator UseCustomMySql(this IConfigurator configurator, Action<IMySqlServerProviderConfigurator> config)
    {
        var dbConfig = new CustomMySqlServerProviderConfigurator();
        config.Invoke(dbConfig);
        return configurator.UseCustomMySql(dbConfig._connectionString,
            dbConfig._tableName, dbConfig._idColumnName,
            dbConfig._jsonColumnName, dbConfig._customColumns);
    }



    /// <summary>
    /// Store the events in a MySQL database.
    /// </summary>
    /// <param name="configurator">The configurator object.</param>
    /// <param name="config">The MySQL provider configuration.</param>
    public static void UseCustomAudit(this IApplicationBuilder app)
    {
        Configuration.AddCustomAction(ActionType.OnEventSaving, async scope =>
        {
            var httpContextAccessor = app.ApplicationServices.GetService<IHttpContextAccessor>();
            if (httpContextAccessor.HttpContext.CheckIsAuthenticated())
            {
                scope.SetCustomField(Consts.UserIdCustomFieldKey, httpContextAccessor.HttpContext.GetClaim("sub"));
                scope.SetCustomField(Consts.UserTypeCustomFieldKey, httpContextAccessor.HttpContext.GetClaim("type"));
                scope.SetCustomField(Consts.ClientCustomFieldKey, httpContextAccessor.HttpContext.GetClaim("client_id"));
                scope.SetCustomField(Consts.ExternalSystemCustomFieldKey, httpContextAccessor.HttpContext.GetClaim("pid"));
            }

        });
        Configuration.IncludeTypeNamespaces = true;
    }
}
