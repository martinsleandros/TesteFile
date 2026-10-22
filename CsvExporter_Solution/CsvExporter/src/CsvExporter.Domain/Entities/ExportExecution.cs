using CsvExporter.Domain.Enums;
namespace CsvExporter.Domain.Entities;
public sealed record ExportExecution(Guid Id, int ExportId, DateOnly ReferenceDate, int DefinitionVersion, ExportStatus Status, DateTimeOffset StartedAt);
