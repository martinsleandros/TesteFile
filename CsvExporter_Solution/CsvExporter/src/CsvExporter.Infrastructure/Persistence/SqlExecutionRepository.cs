using System.Data;
using Microsoft.Data.SqlClient;
using CsvExporter.Application.Abstractions;
using CsvExporter.Application.DTOs;
using CsvExporter.Infrastructure.DataSources.SqlServer;
namespace CsvExporter.Infrastructure.Persistence;
public sealed class SqlExecutionRepository(IConnectionStringResolver connections) : IExecutionRepository {
 public async Task<ExecutionLease?> TryAcquireAsync(int exportId,DateOnly date,int version,CancellationToken ct) {
  var id=Guid.NewGuid();
  try {
   await using var conn=new SqlConnection(connections.Resolve("Metadata")); await conn.OpenAsync(ct);
   await using var cmd=new SqlCommand("INSERT dbo.ExportExecution (ExecutionId,ExportId,ReferenceDate,DefinitionVersion,Status,StartedAt) VALUES (@id,@export,@date,@version,'Running',SYSUTCDATETIME())",conn);
   cmd.Parameters.Add("@id",SqlDbType.UniqueIdentifier).Value=id;
   cmd.Parameters.Add("@export",SqlDbType.Int).Value=exportId;
   cmd.Parameters.Add("@date",SqlDbType.Date).Value=date.ToDateTime(TimeOnly.MinValue);
   cmd.Parameters.Add("@version",SqlDbType.Int).Value=version;
   await cmd.ExecuteNonQueryAsync(ct);
   return new ExecutionLease(id);
  } catch(SqlException ex) when(ex.Number is 2601 or 2627) {return null;}
 }
 public Task CompleteAsync(Guid id,ExportResult result,CancellationToken ct)=>UpdateAsync(id,"Succeeded",result.RecordCount,result.FilePath,result.Sha256,null,ct);
 public Task FailAsync(Guid id,string errorCode,CancellationToken ct)=>UpdateAsync(id,"Failed",null,null,null,errorCode,ct);
 private async Task UpdateAsync(Guid id,string status,long? count,string? path,string? hash,string? error,CancellationToken ct) {
  await using var conn=new SqlConnection(connections.Resolve("Metadata")); await conn.OpenAsync(ct);
  await using var cmd=new SqlCommand("UPDATE dbo.ExportExecution SET Status=@status,EndedAt=SYSUTCDATETIME(),RecordCount=@count,FilePath=@path,Sha256=@hash,ErrorCode=@error WHERE ExecutionId=@id",conn);
  cmd.Parameters.AddWithValue("@id",id);
  cmd.Parameters.AddWithValue("@status",status);
  cmd.Parameters.AddWithValue("@count",(object?)count??DBNull.Value);
  cmd.Parameters.AddWithValue("@path",(object?)path??DBNull.Value);
  cmd.Parameters.AddWithValue("@hash",(object?)hash??DBNull.Value);
  cmd.Parameters.AddWithValue("@error",(object?)error??DBNull.Value);
  if(await cmd.ExecuteNonQueryAsync(ct)!=1) throw new InvalidOperationException("Execution row not found");
 }
}
