using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpVerbsShouldNotTakeInt>;

namespace ExtraDry.Analyzers.Test
{
    public class HttpVerbsShouldNotTakeIntTests {

        [Theory]
        [InlineData("HttpPatch", "Guid")]
        [InlineData("HttpPatch", "string")]
        [InlineData("HttpPut", "Guid")]
        [InlineData("HttpPut", "string")]
        [InlineData("HttpDelete", "Guid")]
        [InlineData("HttpDelete", "string")]
        [InlineData("HttpPost", "Guid")]
        [InlineData("HttpPost", "string")]
        [InlineData("HttpGet", "Guid")]
        [InlineData("HttpGet", "string")]
        [InlineData("Route", "int")]
        public async Task AllGood_NoDiagnostic(string verb, string type)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    public void Method({type} id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpPatch", "int")]
        [InlineData("HttpPut", "int")]
        [InlineData("HttpDelete", "int")]
        [InlineData("HttpPost", "int")]
        [InlineData("HttpGet", "int")]
        [InlineData("HttpPatch", "Int32")]
        [InlineData("HttpPut", "long")]
        [InlineData("HttpDelete", "Int64")]
        [InlineData("HttpPost", "short")]
        [InlineData("HttpGet", "Int16")]
        public async Task IntegerTypeOnHttpVerb_Diagnostic(string verb, string type)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    public void [|Method|]({type} id) {{}}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
