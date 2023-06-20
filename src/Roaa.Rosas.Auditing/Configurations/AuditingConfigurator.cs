namespace Roaa.Rosas.Auditing.Configurations;

public class AuditingConfigurator : IAuditingConfigurator
{
    internal string ConnectionString { get; set; } = "Server=localhost; Database=events; Uid=admin; Pwd=admin;";
    internal string TableName { get; set; } = "Audits";
    internal string IdColumnName { get; set; } = "Id";
    internal string CreatedDateColumnName { get; set; } = "CreatedDate";
    internal string DurationColumnName { get; set; } = "Duration";
    internal string TimeStampColumnName { get; set; } = "TimeStamp";
    internal string MethodColumnName { get; set; } = "Method";
    internal string ActionColumnName { get; set; } = "Action";
    internal string UserIdColumnName { get; set; } = "UserId";
    internal string UserTypeColumnName { get; set; } = "UserType";
    internal string JsonDataColumnName { get; set; } = "JsonData";


    public IAuditingConfigurator SetConnectionString(string connectionString)
    {
        ConnectionString = connectionString;
        return this;
    }

    public IAuditingConfigurator SetTableName(string tableName)
    {
        TableName = tableName;
        return this;
    }

    public IAuditingConfigurator SetIdColumnName(string idColumnName)
    {
        IdColumnName = idColumnName;
        return this;
    }

    public IAuditingConfigurator SetCreatedDateColumnName(string createdDateColumnName)
    {
        CreatedDateColumnName = createdDateColumnName;
        return this;
    }

    public IAuditingConfigurator SetTimeStampColumnName(string timeStampColumnName)
    {
        TimeStampColumnName = timeStampColumnName;
        return this;
    }

    public IAuditingConfigurator SetMethodColumnName(string methodColumnName)
    {
        MethodColumnName = methodColumnName;
        return this;
    }

    public IAuditingConfigurator SetActionColumnName(string actionColumnName)
    {
        ActionColumnName = actionColumnName;
        return this;
    }

    public IAuditingConfigurator SetUserIdColumnName(string userIdColumnName)
    {
        UserIdColumnName = userIdColumnName;
        return this;
    }

    public IAuditingConfigurator SetUserTypeColumnName(string userTypeColumnName)
    {
        UserTypeColumnName = userTypeColumnName;
        return this;
    }

    public IAuditingConfigurator SetJsonDataColumnName(string jsonDataColumnName)
    {
        ConnectionString = jsonDataColumnName;
        return this;
    }
}