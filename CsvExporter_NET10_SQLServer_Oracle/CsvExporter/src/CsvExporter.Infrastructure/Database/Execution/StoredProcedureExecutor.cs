using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using CsvExporter.Application.Abstractions;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
namespace CsvExporter.Infrastructure.Database.Execution;
public sealed class StoredProcedureExecutor(IDbConnectionFactory factory) : IStoredProcedureExecutor {
 public async Task<DataSet> ExecuteDataSetAsync(StoredProcedureRequest request,CancellationToken cancellationToken) {
  if(request.TimeoutSeconds<=0) throw new ArgumentOutOfRangeException(nameof(request.TimeoutSeconds));
  if(!Regex.IsMatch(request.ProcedureName,@"^[A-Za-z_][A-Za-z0-9_]*(\.[A-Za-z_][A-Za-z0-9_]*){0,2}$"))
   throw new ArgumentException("Invalid procedure identifier; use an approved procedure name");
  await using var connection=factory.CreateConnection(request.Provider,request.ConnectionKey);
  await connection.OpenAsync(cancellationToken);
  await using var command=connection.CreateCommand();
  command.CommandText=request.ProcedureName;
  command.CommandType=CommandType.StoredProcedure;
  command.CommandTimeout=request.TimeoutSeconds;
  if(command is OracleCommand oracleCommand) oracleCommand.BindByName=true;
  foreach(var p in request.Parameters) {
   var dbp=command.CreateParameter();
   dbp.ParameterName=p.Name;
   dbp.Direction=p.Direction;
   dbp.DbType=p.Type;
   if(p.Size.HasValue) dbp.Size=p.Size.Value;
   dbp.Value=p.Value??DBNull.Value;
   command.Parameters.Add(dbp);
  }
  // Oracle procedures returning SYS_REFCURSOR need an explicit output parameter.
  // Set OracleDbType.RefCursor directly (DbType does not model REF CURSOR).
  if(request.Provider==DatabaseProvider.Oracle) {
   var cursor=new OracleParameter("p_result",OracleDbType.RefCursor,ParameterDirection.Output);
   command.Parameters.Add(cursor);
  }
  // ExecuteReaderAsync supports cancellation; DataTable.Load itself is synchronous.
  // The starter supports the first result set per procedure, sufficient for Listas/Clientes.
  await using var reader=await command.ExecuteReaderAsync(cancellationToken);
  var dataSet=new DataSet();
  if(reader.FieldCount>0) {
   var table=new DataTable("Result0");
   table.Load(reader);
   dataSet.Tables.Add(table);
  }
  return dataSet;
 }
}
