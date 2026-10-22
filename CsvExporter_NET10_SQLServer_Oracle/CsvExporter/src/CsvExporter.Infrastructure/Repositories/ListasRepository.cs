using System.Data;
using CsvExporter.Application.Abstractions;
namespace CsvExporter.Infrastructure.Repositories;
public sealed class ListasRepository(IStoredProcedureExecutor executor) : IListasRepository {
 public Task<DataSet> GetAsync(DateOnly date,CancellationToken ct) => executor.ExecuteDataSetAsync(
  new StoredProcedureRequest(DatabaseProvider.SqlServer,"ReportingSqlServer","reporting.usp_ExportListas",
   [new ProcedureParameter("@ReferenceDate",date.ToDateTime(TimeOnly.MinValue),DbType.Date)],300),ct);
}
