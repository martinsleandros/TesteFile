using System.Runtime.CompilerServices;
using System.Data;
using Microsoft.Data.SqlClient;
using CsvExporter.Application.Abstractions;
using CsvExporter.Application.DTOs;
using CsvExporter.Domain.ValueObjects;
namespace CsvExporter.Infrastructure.DataSources.SqlServer;
public sealed class SqlServerDataSourceReader(IConnectionStringResolver connections) : IDataSourceReader {
 public string ProviderType=>"SqlServer";
 public async IAsyncEnumerable<ExportRow> ReadAsync(DataSourceDefinition source,DateOnly date,[EnumeratorCancellation] CancellationToken ct) {
  // CommandText is an approved stored procedure name, never raw SQL.
  if(!System.Text.RegularExpressions.Regex.IsMatch(source.CommandText,@"^[A-Za-z_][A-Za-z0-9_]*\.[A-Za-z_][A-Za-z0-9_]*$"))
   throw new ArgumentException("Stored procedure must be schema-qualified");
  await using var conn=new SqlConnection(connections.Resolve(source.ConnectionKey));
  await conn.OpenAsync(ct);
  await using var cmd=new SqlCommand(source.CommandText,conn) {CommandType=CommandType.StoredProcedure,CommandTimeout=source.TimeoutSeconds};
  cmd.Parameters.Add("@ReferenceDate",SqlDbType.Date).Value=date.ToDateTime(TimeOnly.MinValue);
  await using var reader=await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess,ct);
  var names=Enumerable.Range(0,reader.FieldCount).Select(reader.GetName).ToArray();
  while(await reader.ReadAsync(ct)) {
   var values=new Dictionary<string,object?>(StringComparer.OrdinalIgnoreCase);
   for(int i=0;i<names.Length;i++) values[names[i]]=await reader.IsDBNullAsync(i,ct)?null:reader.GetValue(i);
   yield return new ExportRow(values);
  }
 }
}
public interface IConnectionStringResolver { string Resolve(string key); }
