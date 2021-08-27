# blazor-extra-dry-analyzers
Opinionated rules for building RESTful web services and Blazor SPA apps using the Blazor Extra Dry framework.

## Setup

To get the full benefits of this solution:
  * Install the "Visual Studio Extension Development" worload in Visual Studio Installer (`visualstudio2019-workload-visualstudioextension`)
  * Reda a tutorial to understand capabilities, e.g. https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/tutorials/how-to-write-csharp-analyzer-code-fix

## Testing

To test locally, use either the VSIX project to launch and debug or publish the Nuget package locally.

To work with the NuGet package:
  * One time setup:
    * Install the NuGet command line (e.g. `choco install nuget.commandline`)
    * Add the directory for local package to the Visual Studio Package Options (`$env:USERPROFILE\Repos\Nuget\`)
  * After building the project, publish using `publish-local.ps1` script
