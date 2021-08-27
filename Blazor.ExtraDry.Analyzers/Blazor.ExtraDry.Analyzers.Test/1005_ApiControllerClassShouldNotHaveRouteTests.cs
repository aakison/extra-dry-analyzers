﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Blazor.ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    Blazor.ExtraDry.Analyzers.ApiControllerClassShouldNotHaveRoute>;

namespace Blazor.ExtraDry.Analyzers.Test
{
    [TestClass]
    public class ApiControllerClassShouldNotHaveRouteTests {

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
