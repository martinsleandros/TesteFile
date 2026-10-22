using CsvExporter.Application.Abstractions;
using CsvExporter.Application.DTOs;
using CsvExporter.Domain.ValueObjects;
namespace CsvExporter.Infrastructure.Publishing;
public sealed class LocalFilePublisher : IFilePublisher {
 public string DestinationType=>"Local";
 public async Task<PublicationResult> PublishAsync(string temporaryFile,DestinationDefinition destination,DateOnly date,CancellationToken ct) {
  var path=destination.PathTemplate.Replace("{date:yyyyMMdd}",date.ToString("yyyyMMdd"),StringComparison.OrdinalIgnoreCase);
  var full=Path.GetFullPath(path);
  Directory.CreateDirectory(Path.GetDirectoryName(full)!);
  var staging=full+"."+Guid.NewGuid().ToString("N")+".partial";
  try {
   await using(var input=File.OpenRead(temporaryFile))
   await using(var output=new FileStream(staging,FileMode.CreateNew,FileAccess.Write,FileShare.None,65536,true))
    await input.CopyToAsync(output,ct);
   // Refuse overwrite: reprocessing requires an explicit publication policy.
   File.Move(staging,full,overwrite:false);
   return new PublicationResult(full);
  } finally { if(File.Exists(staging)) File.Delete(staging); }
 }
}
