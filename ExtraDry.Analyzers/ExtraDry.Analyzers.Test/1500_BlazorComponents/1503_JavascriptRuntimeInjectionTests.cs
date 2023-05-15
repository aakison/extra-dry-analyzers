using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.JavascriptRuntimeInjection>;

namespace ExtraDry.Analyzers.Test
{
    
    public class JavascriptRuntimeInjectionTests {

        [Fact]
        public async Task AllGood_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleComponent : ComponentBase {{
    [Inject]
    private ExtraDryJavascriptModule Module {{ get; set; }} = null!;
}}
");
        }

        [Fact]
        public async Task DontApplyToNonComponentBase_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleComponent {
    [Inject]
    private IJSRuntime Module { get; set; } = null!;
}
");
        }

        [Fact]
        public async Task DontApplyIfNotInjected_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleComponent : ComponentBase {
    private IJSRuntime Module { get; set; } = null!;
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
