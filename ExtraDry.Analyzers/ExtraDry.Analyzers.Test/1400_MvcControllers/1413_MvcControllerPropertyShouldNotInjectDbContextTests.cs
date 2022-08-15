using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.MvcControllerPropertyShouldNotInjectDdContext>;

namespace ExtraDry.Analyzers.Test
{

    public class MvcControllerPropertyShouldNotInjectDbContextTests {

        [Fact]
        public async Task CorrectUsageNoInject()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : Controller {
    public string JustAProperty { get; set; }
}
");
        }

        [Fact]
        public async Task CorrectUsageInject()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : Controller {
    [Inject]
    public string JustAProperty { get; set; }
}
");
        }

        [Fact]
        public async Task IncorrectButForMvcRule_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Inject]
    public DbContext Database { get; set; }
}
");
        }

        [Theory]
        [InlineData("DbContext")]
        [InlineData("SampleContext")]
        public async Task DependencyOnDbContext_Diagnostic(string className)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleController : Controller {{
    [Inject]
    public {className} [|SampleProperty|] {{ get; set; }}
}}
");
        }

        [Fact]
        public async Task RealWorldException_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
internal interface IComments {
    string Placeholder { get; set; }
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
