using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllerShouldNotHaveAllowAnonymous>;

namespace ExtraDry.Analyzers.Test
{

    public class ApiControllerShouldNotHaveAllowAnonymousTests {

        [Fact]
        public async Task AllGood_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [HttpGet(""abc"")]
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task NotApiController_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[AllowAnonymous]
public class SampleController {
    [HttpGet(""abc"")]
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AllowAnonymous_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|AllowAnonymous|]]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AllowAnonymousAttribute_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|AllowAnonymousAttribute|]]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AllowAnonymousComposite_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController, [|AllowAnonymous|]]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
