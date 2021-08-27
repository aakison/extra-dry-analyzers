using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Blazor.ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    Blazor.ExtraDry.Analyzers.ApiControllerMethodsShouldNotHaveRoute>;

namespace Blazor.ExtraDry.Analyzers.Test
{
    [TestClass]
    public class ApiControllerMethodsShouldNotHaveRouteTests {

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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
        
//        @"
//using System;
//using System.Collections.Generic;
//using System.Text;

//[AttributeUsage(AttributeTargets.Class)]
//public class ApiControllerAttribute : Attribute
//{
//}

//[AttributeUsage(AttributeTargets.Method)]
//public class RouteAttribute : Attribute
//{
//    public RouteAttribute() {}

//    public RouteAttribute(string route) {}
//}

//[AttributeUsage(AttributeTargets.Method)]
//public class HttpGetAttribute : Attribute
//{
//    public HttpGetAttribute() {}

//    public HttpGetAttribute(string route) {}
//}

//";

    }
}
