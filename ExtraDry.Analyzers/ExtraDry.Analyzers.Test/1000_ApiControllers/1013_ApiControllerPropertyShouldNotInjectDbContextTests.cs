using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllerPropertyShouldNotInjectDdContext>;

namespace ExtraDry.Analyzers.Test
{

    public class ApiControllerPropertyShouldNotInjectDbContextTests {

        [Fact]
        public async Task CorrectUsageNoInject()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    public string JustAProperty { get; set; }
}
");
        }

        [Fact]
        public async Task CorrectUsageInject()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Inject]
    public string JustAProperty { get; set; }
}
");
        }

        [Fact]
        public async Task IncorrectButForMvcRule_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : Controller {
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
[ApiController]
public class SampleController {{
    [Inject]
    public {className} [|SampleProperty|] {{ get; set; }}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
