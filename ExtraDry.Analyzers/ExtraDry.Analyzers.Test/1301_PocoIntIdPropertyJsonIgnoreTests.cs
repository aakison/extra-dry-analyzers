using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.PocoIntIdPropertyJsonIgnore>;

namespace ExtraDry.Analyzers.Test
{
    public class PocoIntIdPropertyJsonIgnoreTest {

        [Theory]
        [InlineData("", "Id")]
        [InlineData("", "SampleEntityId")]
        [InlineData("[Key]", "RandomName")]
        public async Task NotPublic_NoDiagnostic(string attribute, string name)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleEntity {{
    {attribute}
    private int {name} {{ get; set; }}
}}
");
        }

        [Fact]
        public async Task NotInt_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    public string Id { get; set; }
}
");
        }

        [Theory]
        [InlineData("", "Id")]
        [InlineData("", "SampleEntityId")]
        [InlineData("[Key]", "RandomName")]
        public async Task PublicId_Diagnostic(string attribute, string name)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleEntity {{
    {attribute}
    public int [|{name}|] {{ get; set; }}
}}
");
        }

        [Fact]
        public async Task PublicSystemInt32_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    public System.Int32 [|Id|] { get; set; }
}
");
        }

        [Theory]
        [InlineData("", "Id")]
        [InlineData("", "SampleEntityId")]
        [InlineData("[Key]", "RandomName")]
        public async Task JsonIgnoredProperly_NoDiagnostic(string attribute, string name)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleEntity {{
    {attribute}
    [JsonIgnore] 
    public int {name} {{ get; set; }}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
