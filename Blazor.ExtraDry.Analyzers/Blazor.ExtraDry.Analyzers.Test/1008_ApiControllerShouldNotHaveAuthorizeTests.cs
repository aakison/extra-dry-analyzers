using System.Threading.Tasks;
using Xunit;
using VerifyCS = Blazor.ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    Blazor.ExtraDry.Analyzers.ApiControllerShouldNotHaveAuthorize>;

namespace Blazor.ExtraDry.Analyzers.Test
{

    public class ApiControllerShouldNotHaveAuthorizeTests {

        [Fact]
        public async Task AllGood_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Authorize(""policy-name"")]
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task NotApiController_NoDiagnostic()
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
[ApiController]
[[|Authorize|]]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AuthorizeParameterized_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|Authorize(""policy-name"")|]]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AuthorizeAttribute_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|AuthorizeAttribute|]]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AuthorizeComposite_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController, [|Authorize|]]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
