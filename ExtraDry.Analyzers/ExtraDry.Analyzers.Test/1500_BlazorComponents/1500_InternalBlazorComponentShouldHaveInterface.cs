using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.InternalBlazorComponentShouldHaveInterface>;

namespace ExtraDry.Analyzers.Test
{
    
    public class InternalBlazorComponentShouldHaveInterfaceTests {

        [Fact]
        public async Task AllGood_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
namespace ExtraDry.Blazor {
    public class SampleComponent : ComponentBase, IExtraDryComponent {
    }
}
");
        }

        [Fact]
        public async Task DontApplyToNonComponentBase_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
namespace ExtraDry.Blazor {
    public class SampleComponent {
    }
}
");
        }

        [Fact]
        public async Task DontApplyToAbstractClasses_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
namespace ExtraDry.Blazor {
    public abstract class SampleComponent : ComponentBase {
    }
}
");
        }

        [Fact]
        public async Task DontApplyToOtherNamespace_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
namespace NotTheRightNamespace {
    public class SampleComponent : ComponentBase {
    }
}
");
        }

        [Fact]
        public async Task MissingInterface_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
namespace ExtraDry.Blazor {
    public class [|SampleComponent|] : ComponentBase {
    }
}
");
        }

        [Fact]
        public async Task MissingInterfaceDeepNamespace_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
namespace Tyring.To.Hide.ExtraDry.Blazor.Namespace {
    public class [|SampleComponent|] : ComponentBase {
    }
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
