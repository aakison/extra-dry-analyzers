using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.EnumShouldHaveJsonConverter>;

namespace ExtraDry.Analyzers.Test
{
    public class EnumShouldHaveJsonConverterTests {

        [Theory]
        [InlineData("internal")]
        [InlineData("")]
        public async Task NotPublicInNamespace_NoDiagnostic(string visibility)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @$"
{visibility} enum SampleEnum {{
    ValueOne,
    ValueTwo,
}}
");
        }

        [Theory]
        [InlineData("internal")]
        [InlineData("")]
        [InlineData("private")]
        [InlineData("protected")]
        [InlineData("protected internal")]
        public async Task NotPublicInClass_NoDiagnostic(string visibility)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @$"
public class SampleContainer {{
    {visibility} enum SampleEnum {{
        ValueOne,
        ValueTwo,
    }}
}}
");
        }


        [Fact]
        public async Task HasRecommendedJsonConverter_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SampleEnum {
    ValueOne,
    ValueTwo,
}
");
        }

        [Fact]
        public async Task HasCustomJsonConverter_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"

public class CustomEnumConverter : JsonConverter<SampleEnum> {
}

[JsonConverter(typeof(CustomEnumConverter))]
public enum SampleEnum {
    ValueOne,
    ValueTwo,
}
");
        }

        [Fact]
        public async Task EnumHasNoConverter_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public enum [|SampleEnum|] {
    ValueOne,
    ValueTwo,
}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
