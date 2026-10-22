using Xunit;
namespace CsvExporter.IntegrationTests;
public class SmokeTests { [Fact] public void Infrastructure_assembly_is_available() {
 Assert.NotNull(typeof(CsvExporter.Infrastructure.Serialization.CsvFileSerializer));
}}
