using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Blazor.ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    Blazor.ExtraDry.Analyzers.HttpVerbsShouldBeInApiController>;

namespace Blazor.ExtraDry.Analyzers.Test
{
    [TestClass]
    public class HttpVerbsShouldBeInApiControllerTests {

        [TestMethod]
        public async Task AllGood_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [HttpPatch(""/route"")]
    public void Retrieve(int id) {}
}
");
        }

        [TestMethod]
        public async Task IgnoreWhenControllerBase_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : ControllerBase {
    [HttpPatch(""/route"")]
    public void Retrieve(int id) {}
}
");
        }

        [TestMethod]
        public async Task IgnoreWhenController_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : Controller {
    [HttpPatch(""/route"")]
    public void Retrieve(int id) {}
}
");
        }




        //        [TestMethod]
        //        public async Task AuthorizeParameterized_Diagnostic()
        //        {
        //            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
        //[ApiController]
        //[[|Authorize(""policy-name"")|]]
        //public class SampleController {
        //    public void Retrieve(int id) {}
        //}
        //");
        //        }

        //        [TestMethod]
        //        public async Task AuthorizeAttribute_Diagnostic()
        //        {
        //            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
        //[ApiController]
        //[[|AuthorizeAttribute|]]
        //public class SampleController {
        //    public void Retrieve(int id) {}
        //}
        //");
        //        }

        //        [TestMethod]
        //        public async Task AuthorizeComposite_Diagnostic()
        //        {
        //            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
        //[ApiController, [|Authorize|]]
        //public class SampleController {
        //    public void Retrieve(int id) {}
        //}
        //");
        //        }


        public string stubs = TestHelpers.Stubs;

    }
}
