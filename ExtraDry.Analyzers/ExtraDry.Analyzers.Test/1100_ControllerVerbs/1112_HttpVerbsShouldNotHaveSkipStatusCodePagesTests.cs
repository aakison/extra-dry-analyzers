using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpVerbsShouldNotHaveSkipStatusCodePages>;

namespace ExtraDry.Analyzers.Test
{
    public class HttpVerbsShouldNotHaveSkipStatusCodePagesTests {

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
[SkipStatusCodePages]
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
        public async Task HasSkipAtMethod_Diagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController : Controller {{
    [{verb}]
    [[|SkipStatusCodePages|]]
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
        public async Task HasSkipAtMethodCombined_Diagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController : Controller {{
    [{verb}, [|SkipStatusCodePages|]]
    public void Retrieve(int id) {{}}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
