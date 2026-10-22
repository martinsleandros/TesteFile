using CsvExporter.Application.Abstractions;
using CsvExporter.Domain.ValueObjects;
namespace CsvExporter.Application.Orchestration;
public sealed class ExportOrchestrator(IExportDefinitionRepository definitions,IExecutionRepository executions,IExportPipeline pipeline) : IExportOrchestrator {
 public async Task<bool> ExecuteAsync(DateOnly date,int? exportId,CancellationToken ct) {
  IReadOnlyList<ExportDefinition> jobs = exportId is int id
   ? (await definitions.GetByIdAsync(id,ct) is { } one ? [one] : throw new ArgumentException($"Export {id} not found"))
   : await definitions.GetEnabledAsync(ct);
  var allOk=true;
  foreach(var job in jobs) {
   ct.ThrowIfCancellationRequested();
   var lease=await executions.TryAcquireAsync(job.Id,date,job.Version,ct);
   if(lease is null) continue;
   try { var result=await pipeline.RunAsync(job,date,ct); await executions.CompleteAsync(lease.ExecutionId,result,ct); }
   catch(Exception ex) { allOk=false; await executions.FailAsync(lease.ExecutionId,ex.GetType().Name,CancellationToken.None); Console.Error.WriteLine($"Export {job.Name} failed: {ex}"); }
  }
  return allOk;
 }
}
