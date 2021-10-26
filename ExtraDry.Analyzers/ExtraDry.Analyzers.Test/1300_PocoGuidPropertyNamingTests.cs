using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.PocoGuidPropertyNaming>;

namespace ExtraDry.Analyzers.Test
{
    public class PocoGuidPropertyNaming {

        [Fact]
        public async Task NotPublic_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    private Guid Guid { get; set; }
}
");
        }

        [Fact]
        public async Task NotGuid_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    public int Guid { get; set; }
}
");
        }

        [Fact]
        public async Task PublicGuid_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    public Guid [|Guid|] { get; set; }
}
");
        }

        [Fact]
        public async Task PublicSystemGuid_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    public System.Guid [|Guid|] { get; set; }
}
");
        }

        [Fact]
        public async Task NamedProperly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    public Guid Uuid { get; set; }
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
