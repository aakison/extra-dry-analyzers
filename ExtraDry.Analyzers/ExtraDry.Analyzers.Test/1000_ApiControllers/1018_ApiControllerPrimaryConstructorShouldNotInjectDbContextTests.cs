using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllerPrimaryConstructorShouldNotInjectDdContext>;

namespace ExtraDry.Analyzers.Test {

    public class ApiControllerPrimaryConstructorShouldNotInjectDbContextTests {

        [Fact]
        public async Task CorrectUsageNoDependencies_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class Dependency { }

[ApiController]
public class SampleController(Dependency dependency) {
}
");
        }


        [Theory]
        [InlineData("DbContext")]
        [InlineData("SampleContext")]
        public async Task NotApiController_NoDiagnostic(string className)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleController({className} dbContext) {{
    
}}
");
        }

        [Theory]
        [InlineData("DbContext")]
        [InlineData("SampleContext")]
        public async Task DependencyOnPrimaryConstructorDbContext_Diagnostic(string className)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class [|SampleController|]({className} dbContext) {{
    
}}
");
        }

        [Theory]
        [InlineData("DbContext")]
        [InlineData("SampleContext")]
        public async Task DependencyOnPrimaryConstructorMiddleDbContext_Diagnostic(string className)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class Dependency {{ }}

[ApiController]
public class [|SampleController|](Dependency d1, {className} dbContext, Dependency d2) {{
    
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
