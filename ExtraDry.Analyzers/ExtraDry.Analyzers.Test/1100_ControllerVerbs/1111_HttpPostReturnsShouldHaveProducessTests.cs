using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpPostReturnShouldHaveProduces>;

namespace ExtraDry.Analyzers.Test
{
    public class HttpPostReturnShouldHaveProducesTests {

        [Theory]
        [InlineData("HttpPost", "Produces")] // Set up alright
        [InlineData("HttpPatch", "Route")] // No produces required for this rule
        [InlineData("HttpPut", "Route")]
        [InlineData("HttpDelete", "Route")]
        [InlineData("HttpGet", "Route")]
        public async Task AllGood_NoDiagnostic(string verb, string other)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    [{other}("""")]
    public object Method(int id) {{ return new object(); }}
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
    public object Method(int id) {{ return new object(); }}
}}
");
        }

        [Fact]
        public async Task NoReturnNoProduces_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [HttpPost]
    public void Method(int id) {{ }}
}}
");
        }

        [Fact]
        public async Task ReturnOnlyTaskNoProduces_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [HttpPost]
    public async Task Method(int id) {{ await Task.Delay(10); }}
}}
");
        }

        [Fact]
        public async Task MissingProduces_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [HttpPost]
    public object [|Method|](int id) {{ return new object(); }}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
