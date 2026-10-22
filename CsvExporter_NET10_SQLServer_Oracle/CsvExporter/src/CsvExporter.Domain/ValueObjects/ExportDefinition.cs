namespace CsvExporter.Domain.ValueObjects;
public sealed record DataSourceDefinition(string ProviderType, string ConnectionKey, string CommandText, int TimeoutSeconds = 300);
public sealed record CsvColumn(string SourceField, string OutputName, int Position, string? Format = null);
public sealed record CsvLayoutDefinition(char Delimiter, bool IncludeHeader, string EncodingName, string Culture, IReadOnlyList<CsvColumn> Columns);
public sealed record DestinationDefinition(string DestinationType, string PathTemplate);
public sealed record ExportDefinition(int Id, string Name, int Version, DataSourceDefinition Source, CsvLayoutDefinition Layout, DestinationDefinition Destination);
