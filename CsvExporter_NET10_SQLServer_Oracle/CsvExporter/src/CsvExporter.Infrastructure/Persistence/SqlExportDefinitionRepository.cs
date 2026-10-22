using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using CsvExporter.Application.Abstractions;
using CsvExporter.Domain.ValueObjects;
using CsvExporter.Infrastructure.DataSources.SqlServer;
namespace CsvExporter.Infrastructure.Persistence;
public sealed class SqlExportDefinitionRepository(IConnectionStringResolver connections) : IExportDefinitionRepository {
 public async Task<IReadOnlyList<ExportDefinition>> GetEnabledAsync(CancellationToken ct) {
  var list=new List<ExportDefinition>();
  await using var conn=new SqlConnection(connections.Resolve("Metadata")); await conn.OpenAsync(ct);
  await using var cmd=new SqlCommand("SELECT DefinitionJson FROM dbo.ExportDefinition WHERE Enabled=1 ORDER BY ExportId",conn);
  await using var reader=await cmd.ExecuteReaderAsync(ct);
  while(await reader.ReadAsync(ct)) list.Add(Parse(reader.GetString(0)));
  return list;
 }
 public async Task<ExportDefinition?> GetByIdAsync(int id,CancellationToken ct) {
  await using var conn=new SqlConnection(connections.Resolve("Metadata")); await conn.OpenAsync(ct);
  await using var cmd=new SqlCommand("SELECT DefinitionJson FROM dbo.ExportDefinition WHERE ExportId=@id AND Enabled=1",conn);
  cmd.Parameters.Add("@id",SqlDbType.Int).Value=id;
  var json=await cmd.ExecuteScalarAsync(ct) as string;
  return json is null?null:Parse(json);
 }
 private static ExportDefinition Parse(string json)=>JsonSerializer.Deserialize<ExportDefinition>(json,new JsonSerializerOptions{PropertyNameCaseInsensitive=true}) ?? throw new InvalidDataException("Invalid definition JSON");
}
