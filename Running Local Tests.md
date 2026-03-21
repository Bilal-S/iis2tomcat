# Running Tests and Builds


With these following assumptions you can run unit tests and should be able to build the files:

Prerequisites:
- You will need `MSBuild.exe` which normally is installed with Visual Studio including community editions. You can put in the `PATH` but that is not a requirment
- You will need the NuGet Packages for `xUnit` 2.9x as testrunners installed for the project

To run build successfully for the BonCode libraries you will need to direct DEBUG output to existing directories. We are using `C:\Sites\AJP13\BIN` as the target output for our binaries. You should adjust this based on your system. The release output is sent to standard sub-directories.

Using Powershell and inside the main Solution directory:

## MSBuild.exe location
C:\Dev\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin

## Run build in PowerShell
& "C:\Dev\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "Connector.Tests\Connector.Tests.csproj" /t:Build /p:Configuration=Debug

## Build tests
& "C:\Dev\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "Connector.Tests\Connector.Tests.csproj" /t:Build /p:Configuration=Debug

## Run tests after building
& "C:\Dev\Microsoft Visual Studio\2022\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" "Connector.Tests\bin\Debug\Connector.Tests.dll"

## Alternative Test Run
You can also use the standard dotnet test command if you like if the tests are already built.

`dotnet test Connector.Tests --no-restore --verbosity normal 2>&1`

or with a filter for a test
`dotnet test Connector.Tests --filter "FullyQualifiedName~LoggerTests" --no-restore --verbosity normal 2>&1`