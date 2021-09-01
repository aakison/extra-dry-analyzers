using System.Threading.Tasks;
using Xunit;
using VerifyCS = Blazor.ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    Blazor.ExtraDry.Analyzers.ApiControllerMethodsShouldNotHaveRoute>;

namespace Blazor.ExtraDry.Analyzers.Test
{

    public class ApiControllerMethodsShouldNotHaveRouteTests {

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
        public async Task RouteOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [[|Route(""abc"")|]]
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task ApiControllerAttributeOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [[|RouteAttribute(""abc"")|]]
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task RouteParameterlessOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [[|Route|]]
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task RouteMixedWithVerb_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [HttpGet, [|Route|]]
    public void Retrieve(int id) {}
}
");
        }

        public string stubs = TestHelpers.Stubs;
        
    }
}
