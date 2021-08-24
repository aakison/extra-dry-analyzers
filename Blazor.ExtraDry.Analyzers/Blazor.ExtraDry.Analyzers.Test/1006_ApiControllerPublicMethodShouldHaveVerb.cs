using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Blazor.ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    Blazor.ExtraDry.Analyzers.ApiControllerPublicMethodShouldHaveVerb>;

namespace Blazor.ExtraDry.Analyzers.Test
{
    [TestClass]
    public class ApiControllerPublicMethodShouldHaveVerbTests {

        [TestMethod]
        public async Task AllGood_NoDiagnostic()
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
        public async Task AllGoodNonStandardVerb_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [HttpPatch(""abc"")]
    public void Retrieve(int id) {}
}
");
        }

        [TestMethod]
        public async Task AllGoodNotAController_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        [TestMethod]
        public async Task AllGoodNotPublicMethod_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    private void Retrieve(int id) {}
}
");
        }


        [TestMethod]
        public async Task MissingVerbMethod_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    public void [|Retrieve|](int id) {}
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

[AttributeUsage(AttributeTargets.Method)]
public class HttpGetAttribute : Attribute
{
    public HttpGetAttribute() {}

    public HttpGetAttribute(string route) {}
}

[AttributeUsage(AttributeTargets.Method)]
public class HttpPatchAttribute : Attribute
{
    public HttpPatchAttribute() {}

    public HttpPatchAttribute(string route) {}
}

";

    }
}
