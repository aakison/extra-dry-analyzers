using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.AuthorizeAttributeUsesPolicy>;

namespace ExtraDry.Analyzers.Test
{
    public class AuthorizeAttributeUsesPolicyTests {

        [Fact]
        public async Task NoAttribute_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    public void Method() {}
}
");
        }

        [Fact]
        public async Task UsesConstructor_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Authorize(""policy-name"")]
    public void Method() {}
}
");
        }

        [Fact]
        public async Task UsesProperty_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Authorize(Policy = ""role-name"")]
    public void Method(int id) {}
}
");
        }

        [Fact]
        public async Task NoConstructorNoProperties_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [[|Authorize|]]
    public void Method(int id) {}
}
");
        }

        [Fact]
        public async Task UsesRoles_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [[|Authorize(Roles = ""role-name"")|]]
    public void Method(int id) {}
}
");
        }

        [Fact]
        public async Task UsesPositionalAndRoles_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [[|Authorize(""policy-name"", Roles = ""role-name"")|]]
    public void Method(int id) {}
}
");
        }

        [Fact]
        public async Task UsesBothProperties_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [[|Authorize(Policy = ""policy-name"", Roles = ""role-name"")|]]
    public void Method(int id) {}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
