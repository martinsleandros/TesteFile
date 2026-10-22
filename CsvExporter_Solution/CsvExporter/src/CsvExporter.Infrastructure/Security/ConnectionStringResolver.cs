using CsvExporter.Infrastructure.DataSources.SqlServer;
using Microsoft.Extensions.Configuration;
namespace CsvExporter.Infrastructure.Security;
public sealed class ConnectionStringResolver(IConfiguration config) : IConnectionStringResolver {
 public string Resolve(string key) => config.GetConnectionString(key) ?? throw new InvalidOperationException($"Missing connection key: {key}");
}
