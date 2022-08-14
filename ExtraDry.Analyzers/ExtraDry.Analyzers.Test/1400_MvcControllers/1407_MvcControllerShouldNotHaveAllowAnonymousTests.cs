using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.MvcControllerShouldNotHaveAllowAnonymous>;

namespace ExtraDry.Analyzers.Test
{

    public class MvcControllerShouldNotHaveAllowAnonymousTests {

        [Fact]
        public async Task AllGood_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : Controller {
    [HttpGet(""abc"")]
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task NotMvcController_NoDiagnostic()
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
[[|AllowAnonymous|]]
public class SampleController : Controller {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AllowAnonymousAttribute_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[[|AllowAnonymousAttribute|]]
public class SampleController : Controller {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AllowAnonymousComposite_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiExceptionStatusCodes, [|AllowAnonymous|]]
public class SampleController : Controller {
    public void Retrieve(int id) {}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
