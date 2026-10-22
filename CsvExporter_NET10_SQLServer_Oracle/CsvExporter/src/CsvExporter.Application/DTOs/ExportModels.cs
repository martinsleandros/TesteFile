namespace CsvExporter.Application.DTOs;
public sealed record ExportRow(IReadOnlyDictionary<string, object?> Fields);
public sealed record ExportResult(string JobName, bool Success, long RecordCount, string? FilePath, string? Sha256);
public sealed record SerializationResult(long RecordCount);
public sealed record PublicationResult(string FinalPath);
public sealed record ExecutionLease(Guid ExecutionId);
