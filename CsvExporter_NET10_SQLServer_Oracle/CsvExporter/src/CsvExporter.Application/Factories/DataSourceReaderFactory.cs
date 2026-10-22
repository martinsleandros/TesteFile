using CsvExporter.Application.Abstractions;
namespace CsvExporter.Application.Factories;
public sealed class DataSourceReaderFactory(IEnumerable<IDataSourceReader> readers) : IDataSourceReaderFactory {
 private readonly IReadOnlyDictionary<string,IDataSourceReader> _readers = readers.ToDictionary(x=>x.ProviderType,StringComparer.OrdinalIgnoreCase);
 public IDataSourceReader Create(string providerType) => _readers.TryGetValue(providerType,out var reader) ? reader : throw new NotSupportedException($"Unsupported provider: {providerType}");
}
