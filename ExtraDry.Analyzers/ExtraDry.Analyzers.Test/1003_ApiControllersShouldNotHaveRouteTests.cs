using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllersShouldNotHaveRoute>;

namespace ExtraDry.Analyzers.Test
{
    public class ApiControllersShouldNotHaveRouteTests {

        [Fact]
        public async Task NotApplicable_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController {
}
");
        }

        [Fact]
        public async Task ApiControllerOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
}
");
        }

        [Fact]
        public async Task ApiControllerAttributeOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiControllerAttribute]
public class SampleController {
}
");
        }

        [Fact]
        public async Task RouteOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[Route]
public class SampleController {
}
");
        }

        [Fact]
        public async Task RouteAttributeOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[RouteAttribute]
public class SampleController {
}
");
        }

        [Fact]
        public async Task ApiControllerAndRoute_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|Route|]]
public class SampleController {
}
");
        }

        [Fact]
        public async Task ApiControllerAndRouteString_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|Route(""route"")|]]
public class SampleController {
}
");
        }

        [Fact]
        public async Task ApiControllerAndRouteAttribute_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|RouteAttribute|]]
public class SampleController {
}
");
        }

        [Fact]
        public async Task ApiControllerAndRouteSingleLine_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController, [|Route|]]
public class SampleController {
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
