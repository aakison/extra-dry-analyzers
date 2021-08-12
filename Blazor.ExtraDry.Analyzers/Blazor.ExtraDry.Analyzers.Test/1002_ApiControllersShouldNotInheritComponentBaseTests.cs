using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = Blazor.ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    Blazor.ExtraDry.Analyzers.ApiControllersShouldNotInheritComponentBase>;

namespace Blazor.ExtraDry.Analyzers.Test
{
    [TestClass]
    public class ApiControllersShouldNotInheritComponentBaseTests {

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
        public async Task DirectInheritence_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class [|SampleController|] : ControllerBase {
}
");
        }

        [TestMethod]
        public async Task IndirectInheritence_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class [|SampleController|] : DerivedControllerBase {
}
");
        }

        [TestMethod]
        public async Task IgnoreWhenControllerWarningWouldOveride_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController : Controller {
}
");
        }

        [TestMethod]
        public async Task IgnoreWhenInheritedControllerWarningWouldOveride_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController : DerivedController {
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

public class ControllerBase { }

public class DerivedControllerBase : ControllerBase {  }

public class Controller : ControllerBase { }

public class DerivedController : Controller {}

";

    }
}
