using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllerShouldHaveGroupName>;

namespace ExtraDry.Analyzers.Test
{

    public class ApiControllerShouldHaveGroupNameTests {

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
[ApiExplorerSettings(GroupName = ""Name"")]
public class SampleController {
    [HttpPost]
    public void DoPost() {}
}
");
        }

        [Fact]
        public async Task MissingApiExplorerSettings_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class [|SampleController|] {
    [HttpPost]
    public void DoPost() {}
}
");
        }

        [Fact]
        public async Task MissingGroupNameInApiExplorerSettings_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[ApiExplorerSettings()]
public class [|SampleController|] {
    [HttpPost]
    public void DoPost() {}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
