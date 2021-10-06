using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ConsumesAttributeNotOnClass>;

namespace ExtraDry.Analyzers.Test
{
    public class ConsumesAttributeNotOnClassTests {

        [Fact]
        public async Task AllGood_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Consumes(""application/json"")]
    public void Method(int id) {}
}
");
        }

        [Fact]
        public async Task NotOnApiController_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
[[|Consumes(""application/json"")|]]
public class SampleController {
    public void Method(int id) {}
}
");
        }

        [Fact]
        public async Task NotOnAnyClass_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[[|Consumes(""application/json"")|]]
public class SampleController {
    public void Method(int id) {}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
