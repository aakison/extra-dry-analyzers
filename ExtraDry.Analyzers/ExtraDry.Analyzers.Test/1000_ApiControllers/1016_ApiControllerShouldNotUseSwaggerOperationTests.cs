using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllerShouldNotUseSwaggerOperation>;

namespace ExtraDry.Analyzers.Test
{

    public class ApiControllerShouldNotUseSwaggerOperationTests {

        [Fact]
        public async Task NotApiController_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : ControllerBase {
    [SwaggerOperation]
    public void DoPost() {}
}
");
        }

        [Fact]
        public async Task CorrectUsageSeparate_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    /// <summary>
    /// # Markdown supported comment
    /// </summary>
    [HttpGet]
    public void DoPost() {}
}
");
        }

        [Theory]
        [InlineData("")]
        [InlineData(@"(""summary"")")]
        [InlineData(@"(""summary"", ""description"")")]
        [InlineData(@"(OperationId = ""property"")")]
        public async Task PostMethodOnMvcControllersNeedAntiForgery_Diagnostic(string ctor)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @$"
[ApiController]
public class SampleController : Controller {{
    [SwaggerOperation{ctor}]
    public void [|DoPost|]() {{}}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
