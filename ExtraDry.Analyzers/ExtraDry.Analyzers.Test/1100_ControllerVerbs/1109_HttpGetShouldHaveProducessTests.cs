using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpGetShouldHaveProduces>;

namespace ExtraDry.Analyzers.Test
{
    public class HttpGetShouldHaveProducesTests {

        [Theory]
        [InlineData("HttpGet", "Produces")] // Set up alright
        [InlineData("HttpPatch", "Route")] // No consumes required 
        [InlineData("HttpPut", "Route")]
        [InlineData("HttpPost", "Route")]
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
        [InlineData("HttpDelete")]
        [InlineData("HttpGet")]
        public async Task MvcViewController_NoDiagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleController : Controller {{
    [{verb}]
    public void Method(int id) {{}}
}}
");
        }

        [Fact]
        public async Task MissingProduces_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [HttpGet]
    public void [|Method|](int id) {{}}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
