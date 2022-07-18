using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.PocoAuditPropertyJsonIgnore>;

namespace ExtraDry.Analyzers.Test
{
    public class PocoAuditPropertyJsonIgnoreTest {

        [Theory]
        [InlineData("OkName")]
        [InlineData("Audit")]
        [InlineData("AuditRecord")]
        [InlineData("Version")]
        [InlineData("VersionInfo")]
        public async Task NoLeakagePrivate_NoDiagnostic(string name)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleEntity {{
    private string {name} {{ get; set; }}
}}
");
        }

        [Theory]
        [InlineData("", "OkName")]
        [InlineData("[JsonIgnore]", "Audit")]
        [InlineData("[JsonIgnore]", "AuditRecord")]
        [InlineData("[JsonIgnore]", "Version")]
        [InlineData("[JsonIgnore]", "VersionInfo")]
        public async Task NoLeakageIgnored_NoDiagnostic(string attribute, string name)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleEntity {{
    {attribute}
    public string {name} {{ get; set; }}
}}
");
        }

        [Theory]
        [InlineData("Audit")]
        [InlineData("AuditRecord")]
        [InlineData("Version")]
        [InlineData("VersionInfo")]
        public async Task PossibleLeakage_Diagnostic(string name)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleEntity {{
    public string [|{name}|] {{ get; set; }}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
