using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.AuthorizeAttributePolicyPositional>;

namespace ExtraDry.Analyzers.Test
{
    public class AuthorizeAttributePolicyPositional {

        [Fact]
        public async Task UsesConstructor_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Authorize(""policy-name"")]
    public void Method() {}
}
");
        }

        [Fact]
        public async Task UsesProperty_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [[|Authorize(Policy = ""role-name"")|]]
    public void Method(int id) {}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
