using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpVerbsShouldNotTakeReservedWords>;

namespace ExtraDry.Analyzers.Test;

public class HttpVerbsShouldNotTakeReservedWordsTests {

    [Fact]
    public async Task AllGood_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [HttpGet]
    public void Method(PageQuery query) {{}}
}}
");
    }

    [Theory]
    [InlineData("filter")]
    [InlineData("Filter")]
    [InlineData("FILTER")]
    [InlineData("Sort")]
    [InlineData("Skip")]
    [InlineData("Take")]
    [InlineData("Token")]
    [InlineData("DefaultTake")]
    public async Task ReservedWord_Diagnostic(string name)
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [HttpGet]
    public void [|Method|](PageQuery {name}) {{}}
}}
");
    }

    [Theory]
    [InlineData("HttpPut")]
    [InlineData("HttpDelete")]
    [InlineData("HttpPost")]
    [InlineData("HttpPatch")]
    public async Task ReservedWordDoesntApplyNotGet_NoDiagnostic(string verb)
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    public void Method(PageQuery filter) {{}}
}}
");
    }

    public string stubs = TestHelpers.Stubs;

}
