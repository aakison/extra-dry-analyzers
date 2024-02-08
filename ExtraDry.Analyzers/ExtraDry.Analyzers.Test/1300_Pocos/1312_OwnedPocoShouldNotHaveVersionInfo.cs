using System.Threading.Tasks;
using Xunit;
using VerifyCS = ExtraDry.Analyzers.Test.CSharpAnalyzerVerifier<
    ExtraDry.Analyzers.OwnedPocoShouldNotHaveVersionInfo>;

namespace ExtraDry.Analyzers.Test;

public class OwnedPocoShouldNotHaveVersionInfoTests {

    [Fact]
    public async Task NoAttributes_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class TableEntity {
    public VersionInfo AuditInfo { get; set; } = new();
}
");
    }

    [Fact]
    public async Task NotClass_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public interface ITableEntity {
    public VersionInfo AuditInfo { get; set; }
}
");
    }

    [Fact]
    public async Task NotVersionInfo_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + @"
public class NotVersionInfo {}
[Owned]
public class SubTableEntity {
    public NotVersionInfo AuditInfo { get; set; } = new();
}
");
    }

    [Fact]
    public async Task RequiredOnly_Diagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(stubs + @"
[Owned]
public class SubTableEntity {
    public VersionInfo [|AuditInfo|] { get; set; } = new();
}
");
    }

    public string stubs = TestHelpers.Stubs;

}
