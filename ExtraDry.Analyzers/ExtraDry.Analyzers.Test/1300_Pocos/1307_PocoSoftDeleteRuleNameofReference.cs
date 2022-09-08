using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.PocoSoftDeleteNameofReference>;

namespace ExtraDry.Analyzers.Test
{
    public class PocoSoftDeleteNameofReferenceTests {

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
        public async Task SoftDeleteRuleValidViaInheritence_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"

public class BaseClass {
    public string Name { get; set; }
}

[SoftDeleteRule(nameof(BaseClass.Name), null)]
public class SampleEntity : BaseClass {
}
");
        }

        [Fact]
        public async Task SoftDeleteRuleNameofReferencesClass_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[SoftDeleteRule([|nameof(SampleEntity)|], null)]
public class SampleEntity {
    private string Name { get; set; }
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }

}
