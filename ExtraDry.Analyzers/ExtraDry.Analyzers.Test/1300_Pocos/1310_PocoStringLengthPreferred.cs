using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.PocoStringLengthPreferred>;

namespace ExtraDry.Analyzers.Test
{
    public class PocoStringLengthPreferredTests {

        [Fact]
        public async Task NoAttributes_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    public string Name { get; set;}
}
");
        }

        [Fact]
        public async Task StringLengthUsed_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [StringLength(100)]
    public string Name { get; set;}
}
");
        }

        [Fact]
        public async Task MaxLengthUsed_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [MaxLength(100)]
    public string [|Name|] { get; set;}
}
");
        }

        [Fact]
        public async Task BothUsed_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [StringLength(100), MaxLength(200)]
    public string Name { get; set;}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }

}
