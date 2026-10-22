using Xunit;
using CsvExporter.Application.Validation;
using CsvExporter.Domain.ValueObjects;
namespace CsvExporter.UnitTests;
public class ExportDefinitionValidatorTests {
 [Fact] public async Task Rejects_empty_columns() {
  var d=new ExportDefinition(1,"test",1,new("SqlServer","Reporting","dbo.Export",30),new(';',true,"utf-8","en-US",[]),new("Local","C:\\Temp\\a.csv"));
  await Assert.ThrowsAsync<ArgumentException>(()=>new ExportDefinitionValidator().ValidateAsync(d,CancellationToken.None));
 }
}
