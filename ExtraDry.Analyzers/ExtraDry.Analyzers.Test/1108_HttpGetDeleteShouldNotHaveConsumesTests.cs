using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpGetDeleteShouldNotHaveConsumes>;

namespace ExtraDry.Analyzers.Test
{
    public class HttpGetDeleteShouldNotHaveConsumesTests {

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
        [InlineData("HttpGet")]
        [InlineData("HttpDelete")]
        public async Task MvcViewController_NoDiagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleController : Controller {{
    [{verb}]
    [Consumes("""")] // Doesn't make sense, but not this rule.
    public void Method(int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpGet")]
        [InlineData("HttpDelete")]
        public async Task InvalidConsumesOnAction_Diagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    [Consumes("""")]
    public void [|Method|](int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpGet")]
        [InlineData("HttpDelete")]
        public async Task InvalidConsumesOnController_Diagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
[Consumes("""")]
public class SampleController {{
    [{verb}]
    public void [|Method|](int id) {{}}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
