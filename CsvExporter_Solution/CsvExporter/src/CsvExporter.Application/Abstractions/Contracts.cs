using CsvExporter.Application.DTOs;
using CsvExporter.Domain.ValueObjects;
namespace CsvExporter.Application.Abstractions;
public interface IExportDefinitionRepository {
 Task<IReadOnlyList<ExportDefinition>> GetEnabledAsync(CancellationToken ct);
 Task<ExportDefinition?> GetByIdAsync(int id, CancellationToken ct);
}
public interface IExecutionRepository {
 Task<ExecutionLease?> TryAcquireAsync(int exportId, DateOnly date, int version, CancellationToken ct);
 Task CompleteAsync(Guid executionId, ExportResult result, CancellationToken ct);
 Task FailAsync(Guid executionId, string errorCode, CancellationToken ct);
}
public interface IDataSourceReader {
 string ProviderType { get; }
 IAsyncEnumerable<ExportRow> ReadAsync(DataSourceDefinition source, DateOnly date, CancellationToken ct);
}
public interface IDataSourceReaderFactory { IDataSourceReader Create(string providerType); }
public interface IFileSerializer {
 string Format { get; }
 Task<SerializationResult> SerializeAsync(IAsyncEnumerable<ExportRow> rows, CsvLayoutDefinition layout, Stream destination, CancellationToken ct);
}
public interface IFilePublisher {
 string DestinationType { get; }
 Task<PublicationResult> PublishAsync(string temporaryFile, DestinationDefinition destination, DateOnly date, CancellationToken ct);
}
public interface IExportValidator { Task ValidateAsync(ExportDefinition definition, CancellationToken ct); }
public interface IExportPipeline { Task<ExportResult> RunAsync(ExportDefinition definition, DateOnly date, CancellationToken ct); }
public interface IExportOrchestrator { Task<bool> ExecuteAsync(DateOnly date, int? exportId, CancellationToken ct); }
