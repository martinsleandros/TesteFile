using System.Security.Cryptography;
using CsvExporter.Application.Abstractions;
using CsvExporter.Application.DTOs;
using CsvExporter.Application.Validation;
using CsvExporter.Domain.ValueObjects;
namespace CsvExporter.Application.Pipelines;
public sealed class ExportPipeline(IDataSourceReaderFactory readers, IEnumerable<IFileSerializer> serializers,
 IEnumerable<IFilePublisher> publishers, ValidationPipeline validation) : IExportPipeline {
 public async Task<ExportResult> RunAsync(ExportDefinition d, DateOnly date, CancellationToken ct) {
  await validation.ValidateAsync(d,ct);
  var serializer = serializers.Single(x=>x.Format.Equals("CSV",StringComparison.OrdinalIgnoreCase));
  var publisher = publishers.Single(x=>x.DestinationType.Equals(d.Destination.DestinationType,StringComparison.OrdinalIgnoreCase));
  var tmp = Path.Combine(Path.GetTempPath(),$"csvexport-{Guid.NewGuid():N}.tmp");
  try {
   SerializationResult result;
   await using(var stream = new FileStream(tmp,FileMode.CreateNew,FileAccess.Write,FileShare.None,65536,useAsync:true)) {
    result = await serializer.SerializeAsync(readers.Create(d.Source.ProviderType).ReadAsync(d.Source,date,ct),d.Layout,stream,ct);
    await stream.FlushAsync(ct);
   }
   string hash;
   await using(var input = File.OpenRead(tmp)) hash = Convert.ToHexString(await SHA256.HashDataAsync(input,ct));
   var published = await publisher.PublishAsync(tmp,d.Destination,date,ct);
   return new ExportResult(d.Name,true,result.RecordCount,published.FinalPath,hash);
  } finally { if(File.Exists(tmp)) File.Delete(tmp); }
 }
}
