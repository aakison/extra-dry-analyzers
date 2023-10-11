using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.PocoDeleteRuleUniqueUndeleteProperty>;

namespace ExtraDry.Analyzers.Test
{
    public class PocoDeleteRuleUniqueUndeletePropertyTests {

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
        public async Task SoftDeleteRuleNoUndelete_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[DeleteRule(DeleteAction.Recycle, nameof(Name), null)]
public class SampleEntity {
    private string Name { get; set; }
}
");
        }

        [Fact]
        public async Task SoftDeleteRuleUniqueStrings_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[DeleteRule(DeleteAction.Recycle, nameof(Name), ""delete"", ""undelete"")]
public class SampleEntity {
    private string Name { get; set; }
}
");
        }

        [Fact]
        public async Task SoftDeleteRuleUniqueEnums_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public enum SampleState { Active, Inactive, Deleted }

[DeleteRule(DeleteAction.Recycle, nameof(State), SampleState.Deleted, SampleState.Inactive)]
public class SampleEntity {
    private SampleState State { get; set; } = SampleState.Active;
}
");
        }

        [Fact]
        public async Task SoftDeleteRuleBothNull_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[DeleteRule(DeleteAction.Recycle, nameof(Name), null, [|null|])]
public class SampleEntity {
    private string Name { get; set; }
}
");
        }

        [Fact]
        public async Task SoftDeleteRuleBothSameString_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[DeleteRule(DeleteAction.Recycle, nameof(Name), ""delete"", [|""delete""|])]
public class SampleEntity {
    private string Name { get; set; }
}
");
        }

        [Fact]
        public async Task SoftDeleteRuleBothSameEnums_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public enum SampleState { Active, Inactive, Deleted }

[DeleteRule(DeleteAction.Recycle, nameof(State), SampleState.Deleted, [|SampleState.Deleted|])]
public class SampleEntity {
    private SampleState State { get; set; } = SampleState.Active;
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
