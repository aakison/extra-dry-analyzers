using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpVerbsShouldHaveCrudNames>;

namespace ExtraDry.Analyzers.Test
{
    public class HttpVerbsShouldHaveCrudNamesTests {

        [Theory]
        [InlineData("HttpPatch", "Insert")]
        [InlineData("HttpPatch", "Patch")]
        [InlineData("HttpPut", "Update")]
        [InlineData("HttpPut", "Upsert")]
        [InlineData("HttpDelete", "Delete")]
        [InlineData("HttpPost", "Create")]
        [InlineData("HttpGet", "Retrieve")]
        [InlineData("HttpGet", "Read")]
        [InlineData("HttpGet", "List")]
        [InlineData("HttpPost", "ListHierarchy")]
        [InlineData("HttpPost", "Tree")]
        [InlineData("HttpPost", "ListTree")]
        [InlineData("HttpGet", "Tree")]
        [InlineData("AllowAnonymous", "NotTriggered")]
        public async Task AllGood_NoDiagnostic(string verb, string prefix)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    public void {prefix}Method(int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpPatch", "Bad")]
        [InlineData("HttpPatch", "Create")]
        [InlineData("HttpPut", "Bad")]
        [InlineData("HttpPut", "Create")]
        [InlineData("HttpDelete", "Bad")]
        [InlineData("HttpPost", "Bad")]
        [InlineData("HttpPost", "Update")]
        [InlineData("HttpGet", "Bad")]
        [InlineData("HttpGet", "Delete")]
        public async Task InvalidPrefix_Diagnostic(string verb, string prefix)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    public void [|{prefix}Method|](int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpPatch")]
        [InlineData("HttpPut")]
        [InlineData("HttpDelete")]
        [InlineData("HttpPost")]
        [InlineData("HttpGet")]
        public async Task NotApiController_NoDiagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleController : Controller {{
    [{verb}]
    public void Method(int id) {{}}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
