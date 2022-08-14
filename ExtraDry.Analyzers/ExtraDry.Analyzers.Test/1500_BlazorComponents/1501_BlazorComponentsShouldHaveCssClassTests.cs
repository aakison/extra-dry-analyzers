using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.BlazorComponentShouldHaveCssClass>;

namespace ExtraDry.Analyzers.Test
{
    
    public class BlazorComponentsShouldHaveCssClassTests {

        [Fact]
        public async Task AllGood_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleComponent : ComponentBase {
    public string CssClass { get; set; }
}
");
        }

        [Fact]
        public async Task DontApplyToNonComponentBase_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleComponent {
    public string NotCssClass { get; set; }
}
");
        }

        [Fact]
        public async Task DontApplyToAbstractClasses_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public abstract class SampleComponent : ComponentBase {
    public string NotCssClass { get; set; }
}
");
        }


        [Fact]
        public async Task MissingCssClassProperty_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class [|SampleComponent|] : ComponentBase {
    public string NotCssClass { get; set; }
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
