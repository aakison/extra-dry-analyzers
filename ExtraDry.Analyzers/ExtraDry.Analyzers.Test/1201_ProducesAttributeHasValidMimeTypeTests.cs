using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ProducesAttributeHasValidMimeType>;

namespace ExtraDry.Analyzers.Test
{
    public class ProducesAttributeHasValidMimeTypeTests {

        [Theory]
        [InlineData(@"""application/json""")]
        [InlineData(@"""application/octet-stream""")]
        [InlineData("MediaTypeNames.Application.Json")]
        [InlineData("MediaTypeNames.Application.Octet")]
        public async Task AllGood_NoDiagnostic(string mimeType)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [Produces({mimeType})]
    public void Method(int id) {{}}
}}
");
        }

        [Theory]
        [InlineData(@"""text/json""")]
        [InlineData(@"""application/pdf""")]
        [InlineData(@"""not-even-a-mime-type""")]
        [InlineData(@"""MediaTypeNames.Application.Pdf""")]
        [InlineData(@"""MediaTypeNames.Text.Plain""")]
        public async Task BadType_Diagnostic(string mimeType)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [[|Produces({mimeType})|]]
    public void Method(int id) {{}}
}}
");
        }

        [Fact]
        public async Task NoMimeTypeProvided_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [[|Produces(typeof(Int32))|]]
    public int Method(int id) {{ return 0; }}
}}
");
        }

        [Fact]
        public async Task MultipleMimeTypesProvided_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [[|Produces(""application/json"", ""bad/second"")|]]
    public void Method(int id) {{}}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
