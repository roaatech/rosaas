using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data.SqlClient;

namespace Roaa.Rosas.Common.HealthChecks.Checkers
{
    public class SqlDbHealthChecker : IHealthCheck
    {
        private string _connectionString { get; set; }

        public SqlDbHealthChecker(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            await using var connection = new SqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync(cancellationToken);
                var command = connection.CreateCommand();
                command.CommandText = "select 1";

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            
            catch (Exception exc)
            {
                return HealthCheckResult.Unhealthy(exc.Message, exc);
            }

            return HealthCheckResult.Healthy();
        }
    }
}
