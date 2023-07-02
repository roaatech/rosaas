namespace Roaa.Rosas.Auditing.Configurations;
public interface IAuditingConfigurator
{
    IAuditingConfigurator SetConnectionString(string connectionString);
    IAuditingConfigurator SetTableName(string tableName);
    IAuditingConfigurator SetIdColumnName(string idColumnName);
    IAuditingConfigurator SetCreatedDateColumnName(string createdDateColumnName);
    IAuditingConfigurator SetTimeStampColumnName(string timeStampColumnName);
    IAuditingConfigurator SetMethodColumnName(string methodColumnName);
    IAuditingConfigurator SetActionColumnName(string actionColumnName);
    IAuditingConfigurator SetJsonDataColumnName(string jsonDataColumnName);

}
