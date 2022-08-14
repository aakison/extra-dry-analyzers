using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.MvcControllerPostMethodsShouldValidateAntiForgery>;

namespace ExtraDry.Analyzers.Test
{

    public class MvcControllerPostMethodsShouldValidateAntiForgeryTests {

        [Fact]
        public async Task CorrectUsageSeparate_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : Controller {
    [HttpPost, ValidateAntiForgeryToken]
    public void DoPost() {}
}
");
        }

        [Fact]
        public async Task NotPostNotNeeded_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : Controller {
    [HttpGet]
    public void DoGet() {}
}
");
        }

        [Fact]
        public async Task AntiForgeryNotNeededNotMvc_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController {
    [HttpPost]
    public void DoPost() {}
}
");
        }

        [Fact]
        public async Task PostMethodOnMvcControllersNeedAntiForgery_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : Controller {
    [HttpPost]
    public void [|DoPost|]() {}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
