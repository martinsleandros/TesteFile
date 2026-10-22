using CsvExporter.Application.Abstractions;
using CsvExporter.Domain.ValueObjects;
namespace CsvExporter.Application.Validation;
public sealed class ValidationPipeline(IEnumerable<IExportValidator> validators) {
 public async Task ValidateAsync(ExportDefinition definition, CancellationToken ct) {
  foreach(var validator in validators) await validator.ValidateAsync(definition,ct);
 }
}
public sealed class ExportDefinitionValidator : IExportValidator {
 public Task ValidateAsync(ExportDefinition d, CancellationToken ct) {
  if (d.Id <= 0 || d.Version <= 0 || string.IsNullOrWhiteSpace(d.Name)) throw new ArgumentException("Invalid export identity");
  if (d.Layout.Columns.Count == 0 || d.Layout.Columns.Any(x=>string.IsNullOrWhiteSpace(x.SourceField) || string.IsNullOrWhiteSpace(x.OutputName)) || d.Layout.Columns.Select(x=>x.Position).Distinct().Count()!=d.Layout.Columns.Count) throw new ArgumentException("Invalid CSV columns");
  if (d.Layout.Delimiter is '\r' or '\n' or '"') throw new ArgumentException("Invalid CSV delimiter");
  if (d.Source.TimeoutSeconds <= 0 || string.IsNullOrWhiteSpace(d.Source.CommandText)) throw new ArgumentException("Invalid data source");
  if (string.IsNullOrWhiteSpace(d.Destination.PathTemplate)) throw new ArgumentException("Invalid destination");
  return Task.CompletedTask;
 }
}
