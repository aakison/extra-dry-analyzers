using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.PocoRulesAttributeGetSet>;

namespace ExtraDry.Analyzers.Test
{
    public class PocoRulesAttributeGetSetTests {

        [Fact]
        public async Task NoRuleAttribute_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    public string Name { get; }
}
");
        }

        [Fact]
        public async Task GetAndSetDefined_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [Rules(RuleAction.Ignore)]
    public string Name { get; set; }
}
");
        }

        [Fact]
        public async Task GetWithoutSet_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [Rules(RuleAction.Ignore)]
    public string [|Name|] { get; } = string.Empty;
}
");
        }

        [Fact]
        public async Task SetWithoutGet_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [Rules(RuleAction.Ignore)]
    public string [|Name|] { set { name = value; } }
    private string name;
}
");
        }

        [Fact]
        public async Task PrivateGet_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [Rules(RuleAction.Ignore)]
    public string [|Name|] { private get; set; }
}
");
        }

        [Fact]
        public async Task PrivateSet_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [Rules(RuleAction.Ignore)]
    public string [|Name|] { get; private set; }
}
");
        }

        [Fact]
        public async Task ImplicitGetOnly_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [Rules(RuleAction.Ignore)]
    public string [|Name|] => string.Empty;
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }

}
