param(
     [parameter(Mandatory=$true)][string] $apikey
)

$SLN = "..\PdfRasterizer\PdfRasterizer.sln"
$NUSPEC = ".\PdfRasterizer.Plugin.nuspec"
$MAIN_DLL = "..\PdfRasterizer\PdfRasterizer\Plugin.PdfRasterizer\bin\Release\Plugin.PdfRasterizer.dll"
$MSBUILD_EXE = "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
$TOOLS_DIR = Join-Path $PSScriptRoot "tools"
$NUGET_EXE = Join-Path $TOOLS_DIR "nuget.exe"

# Try download NuGet.exe if do not exist.
if (!(Test-Path -PathType Container $TOOLS_DIR)) {
    New-Item -ItemType Directory -Force -Path $TOOLS_DIR
}

if (!(Test-Path $NUGET_EXE)) {
    Invoke-WebRequest -Uri http://nuget.org/nuget.exe -OutFile $NUGET_EXE
}

# Make sure NuGet exists where we expect it.
if (!(Test-Path $NUGET_EXE)) {
    Throw "Could not find NuGet.exe"
}

# Building project
& $MSBUILD_EXE $SLN /t:Rebuild /p:Configuration=Release

if($LASTEXITCODE -ne 0) {
    Throw "Build failed"
}

# NuGet packaging

$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($MAIN_DLL).FileVersion
& $NUGET_EXE pack $NUSPEC -Version $version

if($LASTEXITCODE -ne 0) {
    Throw "Packaging failed"
}

# NuGet publishing

$nupkg = "Xam.Plugin.PdfRasterizer.$version.nupkg"
& $NUGET_EXE push $nupkg $apikey

