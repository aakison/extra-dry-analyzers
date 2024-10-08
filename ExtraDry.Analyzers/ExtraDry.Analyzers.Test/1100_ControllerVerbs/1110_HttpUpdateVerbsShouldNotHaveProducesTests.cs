﻿using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.HttpUpdateVerbsShouldNotHaveProduces>;

namespace ExtraDry.Analyzers.Test
{
    public class HttpUpdateVerbsShouldNotHaveProducesTests {

        [Theory]
        [InlineData("HttpGet", "Produces")] // Set up alright
        [InlineData("HttpPut", "Route")] // No produces required 
        [InlineData("HttpPost", "Route")]
        [InlineData("HttpPatch", "Route")]
        [InlineData("HttpDelete", "Route")]
        public async Task AllGood_NoDiagnostic(string verb, string other)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    [{other}("""")]
    public void Method(int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpGet")]
        [InlineData("HttpPut")] 
        [InlineData("HttpPost")]
        [InlineData("HttpPatch")]
        [InlineData("HttpDelete")]
        public async Task MvcViewController_NoDiagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class SampleController : Controller {{
    [{verb}]
    [Consumes("""")] // Doesn't make sense, but not this rule.
    public void Method(int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpPatch")]
        [InlineData("HttpPut")]
        [InlineData("HttpDelete")]
        public async Task InvalidProducesOnAction_Diagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
public class SampleController {{
    [{verb}]
    [Produces("""")]
    public void [|Method|](int id) {{}}
}}
");
        }

        [Theory]
        [InlineData("HttpPatch")]
        [InlineData("HttpPut")]
        [InlineData("HttpDelete")]
        public async Task InvalidProducesOnController_Diagnostic(string verb)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
[Produces("""")]
public class SampleController {{
    [{verb}]
    public void [|Method|](int id) {{}}
}}
");
        }

        [Fact]
        public async Task IdReturnExceptionForUpdate_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
[ApiController]
[Produces(""application/json"")]
public class SampleController {{
    [HttpPut]
    public ResourceReference Method(int id) {{
        return new();
    }}
}}
");
        }

        [Fact]
        public async Task IdReturnGenericExceptionForUpdate_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + $@"
public class Foo
{{
    public Guid Uuid {{ get; set; }}
    public string Slug {{ get; set; }}
    public string Title {{ get; set; }}
}}

[ApiController]
[Produces(""application/json"")]
public class SampleController {{
    [HttpPut]
    public ResourceReference<Foo> Method(int id) {{
        return new();
    }}
}}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
