
@echo off
set Version=
for /F "tokens=1,2" %%a in (ReleaseNotes.md) do ( 
    if NOT defined Version set Version=%%b 
)
echo ==================
echo  Version: %Version%
echo ==================
echo on

dotnet build CodeSizeAnalyzer\CodeSizeAnalyzer.csproj -c Release -p:AssemblyVersion=%Version% -p:Version=%Version%
"C:\Program files\nuget.exe" pack .\CodeSizeAnalyzer.nuspec -OutputDirectory .\nuget -Verbosity detailed -Version %Version%
