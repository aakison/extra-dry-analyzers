using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpVerbsShouldBeInApiController>;

namespace ExtraDry.Analyzers.Test
{
    public class HttpVerbsShouldBeInApiControllerTests {

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

        [Fact]
        public async Task IgnoreWhenControllerBase_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : ControllerBase {
    [HttpPatch(""/route"")]
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task IgnoreWhenController_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : Controller {
    [HttpPatch(""/route"")]
    public void Retrieve(int id) {}
}
");
        }

        [Theory]
        [InlineData("HttpPatch")]
        [InlineData("HttpDelete")]
        [InlineData("HttpPut")]
        public async Task RouteImpliesApiController_Diagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleController {{
    [{verb}]
    public void [|Retrieve|](int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpPost")]
        [InlineData("HttpGet")]
        public async Task RouteOnlySuggestsApiController_NoDiagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleController {{
    [{verb}]
    public void Retrieve(int id) {{}}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
