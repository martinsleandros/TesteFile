using System.Data.Common;
using CsvExporter.Application.Abstractions;
using CsvExporter.Infrastructure.DataSources.SqlServer;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
namespace CsvExporter.Infrastructure.Database.Connections;
public sealed class DbConnectionFactory(IConnectionStringResolver connectionStrings) : IDbConnectionFactory {
 public DbConnection CreateConnection(DatabaseProvider provider, string connectionKey) {
  var cs=connectionStrings.Resolve(connectionKey);
  return provider switch {
   DatabaseProvider.SqlServer => new SqlConnection(cs),
   DatabaseProvider.Oracle => new OracleConnection(cs),
   _ => throw new NotSupportedException($"Unsupported database provider: {provider}")
  };
 }
}
