using Audit.Core;
using Audit.MySql.Configuration;
using Audit.NET.MySql;

namespace Roaa.Rosas.Audit.Net.Custom;

public partial class CustomMySqlServerProviderConfigurator : IMySqlServerProviderConfigurator
{
    internal string _connectionString = "Server=localhost; Database=events; Uid=admin; Pwd=admin;";

    internal string _tableName = "Event";

    internal string _idColumnName = "Id";

    internal string _jsonColumnName = "Data";

    internal string _userIdColumnName = "UserId";

    internal string _userTypeColumnName = "UserType";

    internal string _clientColumnName = "Client";

    internal string _externalSystemColumnName = "ExternalSystem";

    internal List<CustomColumn> _customColumns = new List<CustomColumn>();

    public IMySqlServerProviderConfigurator ConnectionString(string connectionString)
    {
        _connectionString = connectionString;
        return this;
    }

    public IMySqlServerProviderConfigurator TableName(string tableName)
    {
        _tableName = tableName;
        return this;
    }

    public IMySqlServerProviderConfigurator IdColumnName(string idColumnName)
    {
        _idColumnName = idColumnName;
        return this;
    }

    public IMySqlServerProviderConfigurator JsonColumnName(string jsonColumnName)
    {
        _jsonColumnName = jsonColumnName;
        return this;
    }

    public IMySqlServerProviderConfigurator CustomColumn(string columnName, Func<AuditEvent, object> value)
    {
        _customColumns.Add(new CustomColumn(columnName, value));
        return this;
    }


}