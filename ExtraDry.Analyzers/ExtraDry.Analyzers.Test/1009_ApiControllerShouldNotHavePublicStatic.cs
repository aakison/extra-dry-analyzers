using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllerShouldNotHavePublicStatic>;

namespace ExtraDry.Analyzers.Test
{

    public class ApiControllerShouldNotHavePublicStaticTests {

        [Fact]
        public async Task StaticIsPrivate_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    private static void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task PublicIsInstance_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task NotApiController_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController {
    public static void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task PublicAndStatic_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    public static void [|Retrieve|](int id) {}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
