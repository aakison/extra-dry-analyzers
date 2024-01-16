using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.PocoRequiredJsonIgnoreMismatch>;

namespace ExtraDry.Analyzers.Test {
    public class PocoRequiredJsonIgnoreMismatchTests {

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
        public async Task RequiredOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [Required]
    public string Name { get; set;}
}
");
        }

        [Fact]
        public async Task JsonIgnoreOnly_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [JsonIgnore]
    public string Name { get; set;}
}
");
        }

        [Fact]
        public async Task BothUsedOneLine_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [JsonIgnore, Required]
    public string [|Name|] { get; set;}
}
");
        }

        [Fact]
        public async Task BothUsedTwoLines_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class SampleEntity {
    [JsonIgnore]
    [Required]
    public string [|Name|] { get; set;}
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }

}
