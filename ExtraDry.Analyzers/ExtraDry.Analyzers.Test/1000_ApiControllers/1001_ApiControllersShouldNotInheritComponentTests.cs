using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllersShouldNotInheritController>;

namespace ExtraDry.Analyzers.Test
{
    public class ApiControllersShouldNotInheritControllerTests {

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
        public async Task DirectInheritence_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class [|SampleController|] : Controller {
}
");
        }

        [Fact]
        public async Task IndirectInheritence_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class [|SampleController|] : DerivedController {
}
");
        }

        [Fact]
        public async Task NotControllerBase_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController : ControllerBase {
}
");
        }

        public string stubs = TestHelpers.Stubs + @"

public class DerivedControllerBase : ControllerBase {  }

public class DerivedController : Controller {}
";

    }
}
