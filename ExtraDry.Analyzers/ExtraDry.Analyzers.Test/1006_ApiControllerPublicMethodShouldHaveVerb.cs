using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllerPublicMethodShouldHaveVerb>;

namespace ExtraDry.Analyzers.Test
{
    
    public class ApiControllerPublicMethodShouldHaveVerbTests {

        [Fact]
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

        [Fact]
        public async Task DontApplyToStaticMethods_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    public static void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task DontBombOutOnInterfaces_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public interface SampleController {
    void Retrieve(int id);
}
");
        }

        [Fact]
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

        [Fact]
        public async Task AllGoodNotAController_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController {
    public void Retrieve(int id) {}
}
");
        }

        [Fact]
        public async Task AllGoodNotPublicMethod_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    private void Retrieve(int id) {}
}
");
        }

        [Fact]
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
