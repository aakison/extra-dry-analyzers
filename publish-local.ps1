# Publishes a version of the NuGet package to a local directory for testing. 

function HeadIsBranch($branch) {
    if($Env:BUILD_SOURCEBRANCHNAME -eq $null) {
        $headHash = git rev-parse HEAD
        $branchHash = git rev-parse $branch
        $headHash -eq $branchHash
    }
    else {
        $Env:BUILD_SOURCEBRANCHNAME -eq $branch
    }
}

function Coallesce($value1, $value2) {
    if($value1 -eq $null) { $value2 } else { $value1 }
}

$describe = git describe --tags
Write-Host "Git Version is $describe"
$bits = $describe.TrimStart("v").Split(".-")
$majorVersion = Coallesce $bits[0] 1
$minorVersion = Coallesce $bits[1] 0
$patchVersion = Coallesce $bits[2] 0
$prereleaseVersion = "head" # no semantic meaning
if(HeadIsBranch("develop")) {
    $prereleaseVersion = "alpha"
}
if(HeadIsBranch("release")) {
    $prereleaseVersion = "beta"
}
if(HeadIsBranch("main") -OR HeadIsBranch("master")) {
    $prereleaseVersion = ""
}
$semanticVersion = "$majorVersion.$minorVersion.$patchVersion-$prereleaseVersion".TrimEnd("-")

Write-Host "Semenatic version is $semanticVersion"

dotnet pack .\ExtraDry.Analyzers\ExtraDry.Analyzers.Package\ExtraDry.Analyzers.Package.csproj -p:PackageVersion=$semanticVersion

nuget add .\ExtraDry.Analyzers\ExtraDry.Analyzers.Package\bin\Debug\ExtraDry.Analyzers.$semanticVersion.nupkg -source $env:USERPROFILE\Repos\Nuget\
