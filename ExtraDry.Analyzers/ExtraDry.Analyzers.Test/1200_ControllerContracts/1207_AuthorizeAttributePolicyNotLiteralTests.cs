using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.AuthorizeAttributePolicyNotLiteral>;

namespace ExtraDry.Analyzers.Test
{
    public class AuthorizeAttributePolicyNotLiteralTests {

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
        public async Task PolicyNotSpecified_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Authorize]
    public void Method() {}
}
");
        }

        [Fact]
        public async Task AllowAnonymous_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [AllowAnonymous]
    public void Method() {}
}
");
        }

        [Fact]
        public async Task UsesConst_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Authorize(PolicyName)]
    public void Method() {}
    const string PolicyName = ""policy-name"";
}
");
        }

        [Fact]
        public async Task UsesPositionalConst_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Authorize(Policy = PolicyName)]
    public void Method() {}
    const string PolicyName = ""policy-name"";
}
");
        }

        [Fact]
        public async Task UsesNameof_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Authorize(nameof(SampleController))]
    public void Method() {}
}
");
        }

        [Fact]
        public async Task UsesPositionalNameof_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Authorize(Policy = nameof(SampleController))]
    public void Method() {}
}
");
        }

        [Fact]
        public async Task UsesRoles_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [Authorize(Roles = ""role-name"")]
    public void Method(int id) {}
}
");
        }

        [Fact]
        public async Task UsesPositionalPolicy_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[ApiController]
public class SampleController {
    [[|Authorize(""policy-name"")|]]
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
