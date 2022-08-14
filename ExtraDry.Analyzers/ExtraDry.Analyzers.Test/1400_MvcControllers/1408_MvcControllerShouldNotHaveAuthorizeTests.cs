using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.MvcControllerShouldNotHaveAuthorize>;

namespace ExtraDry.Analyzers.Test
{

    public class MvcControllerShouldNotHaveAuthorizeTests {

        [Fact]
        public async Task AllGood_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : Controller {
    [Authorize(""policy-name"")]
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task NotMvcController_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[Authorize]
public class SampleController {
    [HttpGet(""abc"")]
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task Authorize_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[[|Authorize|]]
public class SampleController : Controller {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AuthorizeParameterized_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[[|Authorize(""policy-name"")|]]
public class SampleController : Controller {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AuthorizeAttribute_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[[|AuthorizeAttribute|]]
public class SampleController : Controller {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AuthorizeComposite_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiExceptionStatusCodes, [|Authorize|]]
public class SampleController : Controller {
    public void Retrieve(int id) {}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
