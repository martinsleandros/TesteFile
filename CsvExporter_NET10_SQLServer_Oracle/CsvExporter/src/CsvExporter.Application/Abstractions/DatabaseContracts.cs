using System.Data;
using System.Data.Common;
namespace CsvExporter.Application.Abstractions;
public enum DatabaseProvider { SqlServer, Oracle }
public sealed record ProcedureParameter(string Name, object? Value, DbType Type, ParameterDirection Direction = ParameterDirection.Input, int? Size = null);
public sealed record StoredProcedureRequest(DatabaseProvider Provider, string ConnectionKey, string ProcedureName, IReadOnlyList<ProcedureParameter> Parameters, int TimeoutSeconds = 300);
public interface IDbConnectionFactory {
    DbConnection CreateConnection(DatabaseProvider provider, string connectionKey);
}
public interface IStoredProcedureExecutor {
    Task<DataSet> ExecuteDataSetAsync(StoredProcedureRequest request, CancellationToken cancellationToken);
}
