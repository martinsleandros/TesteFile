using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using CsvExporter.Application.Abstractions;
using CsvExporter.Application.DTOs;
using CsvExporter.Domain.ValueObjects;
namespace CsvExporter.Infrastructure.Serialization;
public sealed class CsvFileSerializer : IFileSerializer {
 public string Format=>"CSV";
 public async Task<SerializationResult> SerializeAsync(IAsyncEnumerable<ExportRow> rows,CsvLayoutDefinition layout,Stream destination,CancellationToken ct) {
  var encoding=Encoding.GetEncoding(layout.EncodingName);
  using var writer=new StreamWriter(destination,encoding,65536,leaveOpen:true);
  var cols=layout.Columns.OrderBy(x=>x.Position).ToArray();
  var culture=CultureInfo.GetCultureInfo(layout.Culture);
  if(layout.IncludeHeader) await writer.WriteLineAsync(string.Join(layout.Delimiter,cols.Select(c=>Escape(c.OutputName,layout.Delimiter))));
  long count=0;
  await foreach(var row in rows.WithCancellation(ct)) {
   var values=cols.Select(c=>{
    if(!row.Fields.TryGetValue(c.SourceField,out var value)) throw new InvalidDataException($"Missing source field {c.SourceField}");
    var s=value is IFormattable f ? f.ToString(c.Format,culture) ?? "" : value?.ToString() ?? "";
    return Escape(s,layout.Delimiter);
   });
   await writer.WriteLineAsync(string.Join(layout.Delimiter,values));
   count++;
  }
  await writer.FlushAsync(ct);
  return new SerializationResult(count);
 }
 private static string Escape(string value,char delimiter) {
  // Spreadsheet formula injection: prefix dangerous cell values with apostrophe.
  var trimmed=value.TrimStart(' ','\t','\r','\n');
  if(trimmed.Length>0 && "=+-@".Contains(trimmed[0])) value="'"+value;
  return value.IndexOfAny([delimiter,'"','\r','\n'])>=0 ? "\""+value.Replace("\"","\"\"")+"\"" : value;
 }
}
