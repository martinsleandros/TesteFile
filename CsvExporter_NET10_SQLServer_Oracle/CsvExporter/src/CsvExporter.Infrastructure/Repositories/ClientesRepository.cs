using System.Data;
using CsvExporter.Application.Abstractions;
namespace CsvExporter.Infrastructure.Repositories;
public sealed class ClientesRepository(IStoredProcedureExecutor executor) : IClientesRepository {
 public Task<DataSet> GetAsync(DateOnly date,CancellationToken ct) => executor.ExecuteDataSetAsync(
  new StoredProcedureRequest(DatabaseProvider.Oracle,"ReportingOracle","PKG_EXPORT.EXPORT_CLIENTES",
   [new ProcedureParameter("p_reference_date",date.ToDateTime(TimeOnly.MinValue),DbType.Date)],300),ct);
}
