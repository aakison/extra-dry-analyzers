using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.BlazorComponentsCommonPropertiesArePublic>;

namespace ExtraDry.Analyzers.Test
{
    
    public class BlazorComponentsCommonPropertiesArePublic {

        [Theory]
        [InlineData("CssClass")]
        [InlineData("Placeholder")]
        public async Task AllGood_NoDiagnostic(string name)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleComponent : ComponentBase {{
    public string {name} {{ get; set; }}
}}
");
        }

        [Fact]
        public async Task DontApplyToNonComponentBase_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleComponent {
    private string CssClass { get; set; }
}
");
        }

        [Fact]
        public async Task DontApplyToAbstractClasses_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public abstract class SampleComponent : ComponentBase {
    protected string CssClass { get; set; }
}
");
        }

        [Theory]
        [InlineData("private")]
        [InlineData("protected")]
        [InlineData("internal")]
        [InlineData("internal protected")]
        public async Task NotPublicCssClassProperty_Diagnostic(string visibility)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleComponent : ComponentBase {{
    {visibility} string [|CssClass|] {{ get; set; }}
}}
");
        }

        [Theory]
        [InlineData("CssClass")]
        [InlineData("Placeholder")]
        public async Task NotPublicCssClassPropertyName_Diagnostic(string name)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleComponent : ComponentBase {{
    private string [|{name}|] {{ get; set; }}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
