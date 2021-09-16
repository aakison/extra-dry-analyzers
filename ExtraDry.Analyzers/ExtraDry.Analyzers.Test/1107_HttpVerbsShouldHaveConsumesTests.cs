using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpUpdateVerbsShouldHaveConsumes>;

namespace ExtraDry.Analyzers.Test
{
    public class HttpVerbsShouldHaveConsumesTests {

        [Theory]
        [InlineData("HttpPatch", "Consumes")] // Set up alright
        [InlineData("HttpPut", "Consumes")]
        [InlineData("HttpPost", "Consumes")]
        [InlineData("HttpGet", "Route")] // No consumes required 
        [InlineData("HttpDelete", "Route")]
        public async Task AllGood_NoDiagnostic(string verb, string other)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    [{other}("""")]
    public void Method(int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpPatch")]
        [InlineData("HttpPut")]
        [InlineData("HttpPost")]
        public async Task MissingConsumes_Diagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    public void [|Method|](int id) {{}}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
