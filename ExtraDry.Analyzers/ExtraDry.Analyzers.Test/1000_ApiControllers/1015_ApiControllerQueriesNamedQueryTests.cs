using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllerQueriesNamedQuery>;

namespace ExtraDry.Analyzers.Test
{

    public class ApiControllerQueriesNamedQueryTests {

        [Fact]
        public async Task NoQuery_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [HttpGet(""/api/test-items"")]
    public List<object> TestMethod() {
        return new List<object>();
    }
}
");
        }

        [Fact]
        public async Task NotFilterQuery_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [HttpGet(""/api/test-items"")]
    public List<object> TestMethod(string filter) {
        return new List<object>();
    }
}
");
        }

        [Theory]
        [InlineData("FilterQuery")]
        [InlineData("PageQuery")]
        public async Task CorrectUsage_NoDiagnostic(string className)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @$"
[ApiController]
public class SampleController {{
    [HttpGet(""/api/test-items"")]
    public List<object> TestMethod({className} query) {{
        return new List<object>();
    }}
}}
");
        }

        [Theory]
        [InlineData("FilterQuery", "filter")]
        [InlineData("PageQuery", "pager")]
        public async Task NoFromQuery_Diagnostic(string className, string badName)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @$"
[ApiController]
public class SampleController {{
    [HttpGet(""/api/test-items"")]
    public List<object> TestMethod([|{className} {badName}|]) {{
        return new List<object>();
    }}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
