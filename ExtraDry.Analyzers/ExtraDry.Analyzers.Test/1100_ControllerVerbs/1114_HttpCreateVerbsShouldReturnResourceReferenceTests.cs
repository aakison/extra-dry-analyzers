using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpCreateVerbsShouldReturnResourceReference>;

namespace ExtraDry.Analyzers.Test;


public class HttpCreateVerbsShouldReturnResourceReferenceTests {

    [Fact]
    public async Task AllGood_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [HttpPost]
    [Produces(""application/json"")]
    public ResourceReference Create(int id) {{
        throw new NotImplementedException();
    }}
}}
");
    }

    [Theory]
    [InlineData("ResourceReference")]
    [InlineData("Task<ResourceReference>")]
    [InlineData("ResourceReference<Company>")]
    [InlineData("Task<ResourceReference<Company>>")]
    public async Task AllGoodAsync_NoDiagnostic(string returnType)
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [HttpPost]
    [Produces(""application/json"")]
    public {returnType} Create(int id) {{
        throw new NotImplementedException();
    }}
}}
");
    }

    [Fact]
    public async Task NoProducesNotThisRule_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController : Controller {{
    [HttpPost]
    public ResourceReference Method(int id) {{
        throw new NotImplementedException();
    }}
}}
");
    }


    [Fact]
    public async Task NotApiController_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleController : Controller {{
    [HttpPost]
    public void Method(int id) {{}}
}}
");
    }

    [Theory]
    [InlineData("HttpPatch")]
    [InlineData("HttpPut")]
    [InlineData("HttpDelete")]
    public async Task NotACreateMethod_NoDiagnostic(string verb)
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    [Produces("""")]
    public void Method(int id) {{}}
}}
");
    }

    [Fact]
    public async Task InvalidReturnType_Diagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
[Produces("""")]
public class SampleController {{
    [HttpPost]
    public void [|Method|](int id) {{}}
}}
");
    }

    public string stubs = TestHelpers.Stubs;


}
