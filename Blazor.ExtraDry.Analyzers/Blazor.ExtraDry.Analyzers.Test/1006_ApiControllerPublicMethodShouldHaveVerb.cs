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
        public async Task DontApplyToStaticMethods_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    public static void Retrieve(int id) {}
}
");
        }

        [TestMethod]
        public async Task DontBombOutOnInterfaces_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public interface SampleController {
    void Retrieve(int id);
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


        public string stubs = TestHelpers.Stubs;

    }
}
