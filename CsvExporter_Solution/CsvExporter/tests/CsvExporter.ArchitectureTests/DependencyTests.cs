using Xunit;
namespace CsvExporter.ArchitectureTests;
public class DependencyTests {
 [Fact] public void Domain_has_no_project_dependencies() {
  var references=typeof(CsvExporter.Domain.ValueObjects.ExportDefinition).Assembly.GetReferencedAssemblies();
  Assert.DoesNotContain(references,x=>x.Name is not null && x.Name.StartsWith("CsvExporter."));
 }
}
