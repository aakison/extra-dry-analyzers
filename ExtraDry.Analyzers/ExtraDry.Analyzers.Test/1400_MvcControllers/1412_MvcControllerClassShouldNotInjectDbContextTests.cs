using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.MvcControllerClassShouldNotInjectDdContext>;

namespace ExtraDry.Analyzers.Test
{

    public class MvcControllerClassShouldNotInjectDbContextTests {

        [Fact]
        public async Task CorrectUsageNoDependencies_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleController : Controller {
    public SampleController() { }
}
");
        }

        [Fact]
        public async Task IncorrectButForApiRule_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    public SampleController(DbContext context) { }
}
");
        }

        [Theory]
        [InlineData("DbContext")]
        [InlineData("SampleContext")]
        public async Task DependencyOnFirstDbContext_Diagnostic(string className)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleController : Controller {{
    public [|SampleController|]({className} dbContext) {{ }}
}}
");
        }

        [Theory]
        [InlineData("DbContext")]
        [InlineData("SampleContext")]
        public async Task DependencyOnMiddleDbContext_Diagnostic(string className)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleController : Controller {{
    public [|SampleController|](string first, string second, {className} dbContext, string last) {{ }}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
