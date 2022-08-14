using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.EnumJsonConverterNotOnProperty>;

namespace ExtraDry.Analyzers.Test
{
    public class EnumJsonConverterNotOnPropertyTests {

        [Theory]
        [InlineData("internal")]
        [InlineData("")]
        public async Task NotPublicInNamespace_NoDiagnostic(string visibility)
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @$"
public enum SampleEnum {{
    ValueOne,
    ValueTwo,
}}

{visibility} class Container {{
    
    public SampleEnum SampleEnum {{ get; set; }}

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
public enum SampleEnum {{
    ValueOne,
    ValueTwo,
}}

public class SampleContainer {{
    {visibility} SampleEnum SampleEnum {{ get; set; }}
}}
");
        }


        [Fact]
        public async Task HasNoJsonConverter_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public enum SampleEnum {
    ValueOne,
    ValueTwo,
}

public class Container {
    public SampleEnum SampleEnum { get; set; }
}
");
        }

        [Fact]
        public async Task HasCustomJsonConverter_NoDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"

public class CustomEnumConverter : JsonConverter<SampleEnum> {
}

public enum SampleEnum {
    ValueOne,
    ValueTwo,
}

public class Container {

    [JsonConverter(typeof(CustomEnumConverter))]
    public SampleEnum SampleEnum { get; set; }

}
");
        }

        [Fact]
        public async Task PropertyHasJsonStringConverter_Diagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public enum SampleEnum {
    ValueOne,
    ValueTwo,
}

public class Container {

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SampleEnum [|SampleEnum|] { get; set; }

}
");
        }

        public string stubs = TestHelpers.Stubs;

    }
}
