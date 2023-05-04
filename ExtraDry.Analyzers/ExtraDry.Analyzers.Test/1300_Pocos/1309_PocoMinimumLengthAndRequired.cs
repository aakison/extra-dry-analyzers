using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.PocoMinimumLengthAndRequired>;

namespace ExtraDry.Analyzers.Test
{
    public class PocoMinimumLengthAndRequiredTests {

        [Fact]
        public async Task NoStringLengthAttribute_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    public string Name { get; set;}
}
");
        }

        [Fact]
        public async Task RequiredButNoStringLength_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [Required]
    public string Name { get; set;}
}
");
        }

        [Fact]
        public async Task NoMinimumLengthNoRequired_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [StringLength(100)]
    public string Name { get; set;}
}
");
        }

        [Fact]
        public async Task MinimumLengthNoRequired_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [StringLength(100, MinimumLength = 5)]
    public string [|Name|] { get; set;}
}
");
        }

        [Fact]
        public async Task MinimumLengthAndRequired_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [Required, StringLength(100, MinimumLength = 5)]
    public string Name { get; set;}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }

}
