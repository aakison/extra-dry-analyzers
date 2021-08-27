﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Blazor.ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    Blazor.ExtraDry.Analyzers.ApiControllerShouldNotHaveAllowAnonymous>;

namespace Blazor.ExtraDry.Analyzers.Test
{
    [TestClass]
    public class ApiControllerShouldNotHaveAllowAnonymousTests {

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
        public async Task AllowAnonymous_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|AllowAnonymous|]]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        [TestMethod]
        public async Task AllowAnonymousAttribute_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|AllowAnonymousAttribute|]]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        [TestMethod]
        public async Task AllowAnonymousComposite_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController, [|AllowAnonymous|]]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }


        public string stubs = TestHelpers.Stubs;

    }
}
