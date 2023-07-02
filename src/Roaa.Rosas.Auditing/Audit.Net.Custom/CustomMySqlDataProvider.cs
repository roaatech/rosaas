using Audit.Core;
using Audit.MySql.Configuration;
using Audit.MySql.Providers;
using MySqlConnector;
using Roaa.Rosas.Auditing.Helpers;

namespace Roaa.Rosas.Audit.Net.Custom;
public class CustomMySqlDataProvider : MySqlDataProvider
{

    private bool _addUser = false;
    private bool _addUserType = false;
    private bool _addClient = false;
    private bool _addExternalSystem = false;


    private string _userIdColumnName = "UserId";
    private string _userTypeColumnName = "UserType";
    private string _clientColumnName = "Client";
    private string _externalSystemColumnName = "ExternalSystem";

    /// <summary>
    /// The MySQL connection string
    /// </summary>
    public string UserColumnName
    {
        get { return _userIdColumnName; }
        set { _userIdColumnName = value; }
    }

    /// <summary>
    /// The MySQL events Table Name
    /// </summary>
    public string UserTypeColumnName
    {
        get { return _userTypeColumnName; }
        set { _userTypeColumnName = value; }
    }

    /// <summary>
    /// The Column Name that stores the JSON
    /// </summary>
    public string ClientColumnName
    {
        get { return _clientColumnName; }
        set { _clientColumnName = value; }
    }

    /// <summary>
    /// The Column Name that is the primary ley
    /// </summary>
    public string ExternalSystemColumnName
    {
        get { return _externalSystemColumnName; }
        set { _externalSystemColumnName = value; }
    }


    #region Ctors     
    public CustomMySqlDataProvider() : base()
    {
    }

    public CustomMySqlDataProvider(Action<IMySqlServerProviderConfigurator> config) : base(config)
    {
        var mysqlConfig = new CustomMySqlServerProviderConfigurator();
        if (config != null)
        {
            config.Invoke(mysqlConfig);

            _userIdColumnName = mysqlConfig._userIdColumnName;
            _userTypeColumnName = mysqlConfig._userTypeColumnName;
            _clientColumnName = mysqlConfig._clientColumnName;
            _externalSystemColumnName = mysqlConfig._externalSystemColumnName;
        }
    }
    #endregion



    public override object InsertEvent(AuditEvent auditEvent)
    {
        using (var cnn = new MySqlConnection(ConnectionString))
        {
            var cmd = GetInsertCommand(cnn, auditEvent);
            object id = cmd.ExecuteScalar();
            return id;
        }
    }

    public override async Task<object> InsertEventAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        using (var cnn = new MySqlConnection(ConnectionString))
        {
            var cmd = GetInsertCommand(cnn, auditEvent);
            object id = await cmd.ExecuteScalarAsync(cancellationToken);
            return id;
        }
    }

    private MySqlCommand GetInsertCommand(MySqlConnection cnn, AuditEvent auditEvent)
    {
        var cmdText = string.Format("INSERT INTO `{0}` ({1}) VALUES({2}); SELECT LAST_INSERT_ID();",
            TableName,
            GetColumnsForInsert(),
            GetParameterNamesForInsert());
        cnn.Open();
        var cmd = cnn.CreateCommand();
        cmd.CommandText = cmdText;
        cmd.Parameters.AddRange(GetParameterValues(auditEvent).ToArray());
        return cmd;
    }


    private string GetParameterNamesForInsert()
    {
        var names = new List<string>();
        if (JsonColumnName != null)
        {
            names.Add("@value");
        }
        if (CustomColumns != null)
        {
            for (int i = 0; i < CustomColumns.Count; i++)
            {
                names.Add($"@c{i}");
            }
        }

        if (_addUser)
        {
            names.Add($"@c_{_userIdColumnName}");
        }

        if (_addUserType)
        {
            names.Add($"@c_{_userTypeColumnName}");
        }

        if (_addClient)
        {
            names.Add($"@c_{_clientColumnName}");
        }

        if (_addExternalSystem)
        {
            names.Add($"@c_{_externalSystemColumnName}");
        }

        return string.Join(", ", names);
    }

    private List<MySqlParameter> GetParameterValues(AuditEvent auditEvent)
    {
        var parameters = new List<MySqlParameter>();
        if (JsonColumnName != null)
        {
            parameters.Add(new MySqlParameter("@value", auditEvent.ToJson()));
        }
        if (CustomColumns != null)
        {
            for (int i = 0; i < CustomColumns.Count; i++)
            {
                parameters.Add(new MySqlParameter($"@c{i}", CustomColumns[i].Value?.Invoke(auditEvent)));
            }
        }

        if (_addUser)
        {
            parameters.Add(new MySqlParameter($"@c_{_userIdColumnName}", TryGetCustomFieldValue(auditEvent, Consts.UserIdCustomFieldKey)));
        }

        if (_addUserType)
        {
            parameters.Add(new MySqlParameter($"@c_{_userTypeColumnName}", TryGetCustomFieldValue(auditEvent, Consts.UserTypeCustomFieldKey)));
        }

        if (_addClient)
        {
            parameters.Add(new MySqlParameter($"@c_{_clientColumnName}", TryGetCustomFieldValue(auditEvent, Consts.ClientCustomFieldKey)));
        }

        if (_addExternalSystem)
        {
            parameters.Add(new MySqlParameter($"@c_{_externalSystemColumnName}", TryGetCustomFieldValue(auditEvent, Consts.ExternalSystemCustomFieldKey)));
        }

        return parameters;
    }


    private string GetColumnsForInsert()
    {
        var columns = new List<string>();
        if (JsonColumnName != null)
        {
            columns.Add(JsonColumnName);
        }
        if (CustomColumns != null)
        {
            foreach (var column in CustomColumns)
            {
                columns.Add(column.Name);
            }
        }

        if (!columns.Any(x => x.Equals(_userIdColumnName, StringComparison.OrdinalIgnoreCase)))
        {
            _addUser = true;
            columns.Add(_userIdColumnName);
        }

        if (!columns.Any(x => x.Equals(_userTypeColumnName, StringComparison.OrdinalIgnoreCase)))
        {
            _addUserType = true;
            columns.Add(_userTypeColumnName);
        }

        if (!columns.Any(x => x.Equals(_clientColumnName, StringComparison.OrdinalIgnoreCase)))
        {
            _addClient = true;
            columns.Add(_clientColumnName);
        }

        if (!columns.Any(x => x.Equals(_externalSystemColumnName, StringComparison.OrdinalIgnoreCase)))
        {
            _addExternalSystem = true;
            columns.Add(_externalSystemColumnName);
        }

        return string.Join(", ", columns.Select(c => $"`{c}`"));
    }


    private string? TryGetCustomFieldValue(AuditEvent auditEvent, string customFieldKey)
    {
        if (auditEvent.CustomFields is not null && auditEvent.CustomFields.TryGetValue(customFieldKey, out object value))
        {
            return value?.ToString();
        }
        return null;
    }

}
