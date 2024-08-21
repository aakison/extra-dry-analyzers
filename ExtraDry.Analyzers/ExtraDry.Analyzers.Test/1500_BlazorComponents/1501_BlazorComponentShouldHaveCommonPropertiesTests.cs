using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.BlazorComponentShouldHaveCommonProperties>;

namespace ExtraDry.Analyzers.Test
{
    
    public class BlazorComponentShouldHaveCommonPropertiesTests {

        [Fact]
        public async Task AllGood_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleComponent : ComponentBase, IExtraDryComponent {
    public string CssClass { get; set; }
    public Dictionary<string, object> UnmatchedAttributes { get; set; }
}
");
        }

        [Fact]
        public async Task DontApplyToNonComponentBase_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleComponent : IExtraDryComponent {
    public string NotCommon { get; set; }
}
");
        }

        [Fact]
        public async Task DontApplyToAbstractClasses_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public abstract class SampleComponent : ComponentBase, IExtraDryComponent {
    public string NotCommon { get; set; }
}
");
        }

        [Fact]
        public async Task InheritedFromBaseClass_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleBase : ComponentBase, IExtraDryComponent {
    public string CssClass { get; set; }
    public Dictionary<string, object> UnmatchedAttributes { get; set; }
}

public class SampleComponent : SampleBase, IExtraDryComponent {
}
");
        }


        [Fact]
        public async Task MissingCssClassProperty_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class [|SampleComponent|] : ComponentBase, IExtraDryComponent {
    public string NotCommon { get; set; }
    public Dictionary<string, object> UnmatchedAttributes { get; set; }
}
");
        }

        [Fact]
        public async Task MissingUnmatchedAttributesProperty_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class [|SampleComponent|] : ComponentBase, IExtraDryComponent {
    public string CssClass { get; set; }
    public Dictionary<string, object> NotUnmatchedAttributes { get; set; }
}
");
        }
        
        public string stubs = TestHelpers.Stubs;

    }
}
