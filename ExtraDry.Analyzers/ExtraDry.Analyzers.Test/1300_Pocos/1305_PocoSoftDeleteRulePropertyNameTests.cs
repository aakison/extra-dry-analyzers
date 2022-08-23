using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.PocoSoftDeleteRulePropertyName>;

namespace ExtraDry.Analyzers.Test
{
    public class PocoSoftDeleteRulePropertyNameTests {

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
[SoftDeleteRule(nameof(Name), null)]
public class SampleEntity {
    private string Name { get; set; }
}
");
        }

        [Fact]
        public async Task SoftDeleteRuleStringly_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[SoftDeleteRule([|""Name""|], null)]
public class SampleEntity {
    private string Name { get; set; }
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
