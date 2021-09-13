using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpVerbsShouldHaveExplicitSecurity>;

namespace ExtraDry.Analyzers.Test
{
    public class HttpVerbsShouldHaveExplicitSecurity {

        [Theory]
        [InlineData("HttpPatch", "AllowAnonymous")] // Set up alright
        [InlineData("HttpPatch", "Authorize")]
        [InlineData("HttpPut", "AllowAnonymous")]
        [InlineData("HttpPut", "Authorize")]
        [InlineData("HttpDelete", "AllowAnonymous")]
        [InlineData("HttpDelete", "Authorize")]
        [InlineData("HttpPost", "AllowAnonymous")]
        [InlineData("HttpPost", "Authorize")]
        [InlineData("HttpGet", "AllowAnonymous")]
        [InlineData("HttpGet", "Authorize")]
        [InlineData("AllowAnonymous", "Route")] // No verb
        [InlineData("Authorize", "Route")]
        public async Task AllGood_NoDiagnostic(string verb, string security)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    [{security}]
    public void Method(int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpPatch")]
        [InlineData("HttpPut")]
        [InlineData("HttpDelete")]
        [InlineData("HttpPost")]
        [InlineData("HttpGet")]
        public async Task MissingSecurity_Diagnostic(string verb)
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
