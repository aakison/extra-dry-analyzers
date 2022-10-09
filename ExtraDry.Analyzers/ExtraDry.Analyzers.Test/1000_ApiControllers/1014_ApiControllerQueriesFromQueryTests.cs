using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ApiControllerQueriesFromQuery>;

namespace ExtraDry.Analyzers.Test
{

    public class ApiControllerQueriesFromQueryTests {

        [Fact]
        public async Task NoFilter_NoDiagnostic()
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

        [Theory]
        [InlineData("FilterQuery")]
        [InlineData("PageQuery")]
        public async Task CorrectUsage_NoDiagnostic(string className)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @$"
[ApiController]
public class SampleController {{
    [HttpGet(""/api/test-items"")]
    public List<object> TestMethod([FromQuery] {className} query) {{
        return new List<object>();
    }}
}}
");
        }

        [Theory]
        [InlineData("FilterQuery")]
        [InlineData("PageQuery")]
        public async Task NoFromQuery_Diagnostic(string className)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @$"
[ApiController]
public class SampleController {{
    [HttpGet(""/api/test-items"")]
    public List<object> TestMethod([|{className} query|]) {{
        return new List<object>();
    }}
}}
");
        }

        [Fact]
        public async Task NoFromQueryButOtherAttribute_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class RandomParameterAttribute : Attribute {
}

[ApiController]
public class SampleController {
    [HttpGet(""/api/test-items"")]
    public List<object> TestMethod([|[RandomParameter] FilterQuery query|]) {
        return new List<object>();
    }
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
