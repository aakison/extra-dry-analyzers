using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.MvcControllerClassShouldHaveIgnoreApi>;

namespace ExtraDry.Analyzers.Test
{

    public class MvcControllerClassShouldHaveIgnoreApiTests {

        [Fact]
        public async Task CorrectUsageSeparate_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiExplorerSettings(IgnoreApi = true)]
public class SampleController : Controller {
}
");
        }

        [Fact]
        public async Task NotMvcController_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : ControllerBase {
}
");
        }

        [Fact]
        public async Task BothMvcAndApiControllerDeferToOtherRules_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController : Controller {
}
");
        }

        [Fact]
        public async Task NoApiExplorerSettingsAttribute_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class [|SampleController|] : Controller {
}
");
        }

        [Fact]
        public async Task ApiExplorerAttributeNoIgnoreProperty_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiExplorerSettings]
public class [|SampleController|] : Controller {
}
");
        }

        [Fact]
        public async Task ApiExplorerAttributeIgnoreApiIsFalse_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiExplorerSettings(IgnoreApi = false)]
public class [|SampleController|] : Controller {
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
