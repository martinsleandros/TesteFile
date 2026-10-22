using System.Data;
namespace CsvExporter.Application.Abstractions;
public interface IListasRepository { Task<DataSet> GetAsync(DateOnly referenceDate,CancellationToken ct); }
public interface IClientesRepository { Task<DataSet> GetAsync(DateOnly referenceDate,CancellationToken ct); }
