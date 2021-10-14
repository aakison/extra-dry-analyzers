using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.ConsumesAttributeHasValidMimeType>;

namespace ExtraDry.Analyzers.Test
{
    public class ConsumesAttributeHasValidMimeTypeTests {

        [Theory]
        [InlineData(@"""application/json""")]
        [InlineData(@"""multipart/form-data""")]
        [InlineData("MediaTypeNames.Application.Json")]
        public async Task AllGood_NoDiagnostic(string mimeType)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [Consumes({mimeType})]
    public void Method(int id) {{}}
}}
");
        }

        [Theory]
        [InlineData(@"""text/json""")]
        [InlineData(@"""application/pdf""")]
        [InlineData(@"""not-even-a-mime-type""")]
        [InlineData(@"""application/octet-stream""")]
        [InlineData("MediaTypeNames.Application.Octet")]
        public async Task BadType_Diagnostic(string mimeType)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [[|Consumes({mimeType})|]]
    public void Method(int id) {{}}
}}
");
        }

        [Fact]
        public async Task MultipleMimeTypesProvided_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [[|Consumes(""application/json"", ""bad/second"")|]]
    public void Method(int id) {{}}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
