using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.PocoDeleteRulePropertyName>;

namespace ExtraDry.Analyzers.Test
{
    public class PocoDeleteRulePropertyNameTests {

        [Fact]
        public async Task NotSoftDeleteRule_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    private string Name { get; set; }
}
");
        }

        [Fact]
        public async Task SoftDeleteRuleValid_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[DeleteRule(DeleteAction.Recycle, nameof(Name), null)]
public class SampleEntity {
    private string Name { get; set; }
}
");
        }

        [Fact]
        public async Task SoftDeleteRuleStringly_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[DeleteRule(DeleteAction.Recycle, [|""Name""|], null)]
public class SampleEntity {
    private string Name { get; set; }
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
