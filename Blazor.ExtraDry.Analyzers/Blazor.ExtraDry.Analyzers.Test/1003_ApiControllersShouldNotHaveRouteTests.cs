using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Blazor.ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    Blazor.ExtraDry.Analyzers.ApiControllersShouldNotHaveRoute>;

namespace Blazor.ExtraDry.Analyzers.Test
{
    [TestClass]
    public class ApiControllersShouldNotHaveRouteTests {

        [TestMethod]
        public async Task NotApplicable_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController {
}
");
        }

        [TestMethod]
        public async Task ApiControllerOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
}
");
        }

        [TestMethod]
        public async Task ApiControllerAttributeOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiControllerAttribute]
public class SampleController {
}
");
        }

        [TestMethod]
        public async Task RouteOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[Route]
public class SampleController {
}
");
        }

        [TestMethod]
        public async Task RouteAttributeOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[RouteAttribute]
public class SampleController {
}
");
        }

        [TestMethod]
        public async Task ApiControllerAndRoute_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|Route|]]
public class SampleController {
}
");
        }

        [TestMethod]
        public async Task ApiControllerAndRouteString_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|Route(""route"")|]]
public class SampleController {
}
");
        }

        [TestMethod]
        public async Task ApiControllerAndRouteAttribute_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|RouteAttribute|]]
public class SampleController {
}
");
        }

        [TestMethod]
        public async Task ApiControllerAndRouteSingleLine_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController, [|Route|]]
public class SampleController {
}
");
        }

        public string stubs = @"
using System;
using System.Collections.Generic;
using System.Text;

[AttributeUsage(AttributeTargets.Class)]
public class ApiControllerAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public class RouteAttribute : Attribute
{
    public RouteAttribute() {}

    public RouteAttribute(string route) {}
}

";

    }
}
