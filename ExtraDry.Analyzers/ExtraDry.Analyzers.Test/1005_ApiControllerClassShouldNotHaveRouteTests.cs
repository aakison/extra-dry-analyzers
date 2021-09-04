using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllerClassShouldNotHaveRoute>;

namespace ExtraDry.Analyzers.Test
{

    public class ApiControllerClassShouldNotHaveRouteTests {

        [Fact]
        public async Task NotApplicable_NoDiagnostic()
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
        public async Task RouteOnClass_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|Route|]]
public class SampleController {
    [HttpGet(""abc"")]
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task ApiControllerRouteAttributeOnly_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|RouteAttribute(""abc"")|]]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task RouteMixedWithApiController_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController, [|Route(""asdf"")|]]
public class SampleController {
    [HttpGet]
    public void Retrieve(int id) {}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
