using CsvExporter.Infrastructure.Database.Connections;
using CsvExporter.Infrastructure.Database.Execution;
using CsvExporter.Infrastructure.Repositories;
using CsvExporter.Application.Abstractions;
using CsvExporter.Application.Factories;
using CsvExporter.Application.Orchestration;
using CsvExporter.Application.Pipelines;
using CsvExporter.Application.Validation;
using CsvExporter.Infrastructure.DataSources.SqlServer;
using CsvExporter.Infrastructure.Persistence;
using CsvExporter.Infrastructure.Publishing;
using CsvExporter.Infrastructure.Security;
using CsvExporter.Infrastructure.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
var builder=Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IConnectionStringResolver,ConnectionStringResolver>();
builder.Services.AddScoped<IDbConnectionFactory,DbConnectionFactory>();
builder.Services.AddScoped<IStoredProcedureExecutor,StoredProcedureExecutor>();
builder.Services.AddScoped<IListasRepository,ListasRepository>();
builder.Services.AddScoped<IClientesRepository,ClientesRepository>();
builder.Services.AddScoped<IExportDefinitionRepository,SqlExportDefinitionRepository>();
builder.Services.AddScoped<IExecutionRepository,SqlExecutionRepository>();
builder.Services.AddScoped<IDataSourceReader,SqlServerDataSourceReader>();
builder.Services.AddScoped<IDataSourceReaderFactory,DataSourceReaderFactory>();
builder.Services.AddScoped<IFileSerializer,CsvFileSerializer>();
builder.Services.AddScoped<IFilePublisher,LocalFilePublisher>();
builder.Services.AddScoped<IExportValidator,ExportDefinitionValidator>();
builder.Services.AddScoped<ValidationPipeline>();
builder.Services.AddScoped<IExportPipeline,ExportPipeline>();
builder.Services.AddScoped<IExportOrchestrator,ExportOrchestrator>();
using var host=builder.Build();
var logger=host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("CsvExporter");
try {
 DateOnly date=DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
 int? id=null;
 for(var i=0;i<args.Length;i++) {
  if(args[i]=="--date" && i+1<args.Length) date=DateOnly.ParseExact(args[++i],"yyyy-MM-dd");
  else if(args[i]=="--export" && i+1<args.Length) id=int.Parse(args[++i]);
  else throw new ArgumentException($"Invalid argument: {args[i]}");
 }
 using var scope=host.Services.CreateScope();
 var ok=await scope.ServiceProvider.GetRequiredService<IExportOrchestrator>().ExecuteAsync(date,id,CancellationToken.None);
 return ok?0:1;
} catch(Exception ex) {logger.LogError(ex,"Batch failed");return 1;}
