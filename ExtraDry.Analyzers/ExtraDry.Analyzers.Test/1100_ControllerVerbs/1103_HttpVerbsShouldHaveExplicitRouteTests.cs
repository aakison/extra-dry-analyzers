using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpVerbsShouldHaveExplicitRoute>;

namespace ExtraDry.Analyzers.Test
{
    public class HttpVerbsShouldHaveExplicitRouteTests {

        [Theory]
        [InlineData("HttpPatch")]
        [InlineData("HttpPut")]
        [InlineData("HttpDelete")]
        [InlineData("HttpPost")]
        [InlineData("HttpGet")]
        public async Task AllGood_NoDiagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}(""/route"")]
    public void Retrieve(int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpPatch")]
        [InlineData("HttpPut")]
        [InlineData("HttpDelete")]
        [InlineData("HttpPost")]
        [InlineData("HttpGet")]
        public async Task NoArgumentsAtAll_Diagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController : Controller {{
    [[|{verb}|]]
    public void Retrieve(int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpPatch")]
        [InlineData("HttpPut")]
        [InlineData("HttpDelete")]
        [InlineData("HttpPost")]
        [InlineData("HttpGet")]
        public async Task NamedArgumentNotRouteTemplate_Diagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController : Controller {{
    [[|{verb}(Name = ""name"")|]]
    public void Retrieve(int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpPatch")]
        [InlineData("HttpPut")]
        [InlineData("HttpDelete")]
        [InlineData("HttpPost")]
        [InlineData("HttpGet")]
        public async Task NamedArgumentIsRouteTemplate_NoDiagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController : Controller {{
    [{verb}(Template = ""name"")]
    public void Retrieve(int id) {{}}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
