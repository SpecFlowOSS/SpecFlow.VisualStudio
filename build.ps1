param (
 [string]$Configuration = "Debug",
 [string]$ExtensionVisualStudioVersion = "2017"
)

$VisualStudioVersion = $Env:VisualStudioVersion;
$msbuildPath = "msbuild"

if ([System.String]::IsNullOrEmpty($Env:MSBuild))
{
  $msbuildPath = join-path $Env:MSBuild 'msbuild.exe';
  Write-Host "Using msbuild from environment variable";
}
elseif ([Environment]::OSVersion.Platform -eq "Win32NT")
{
  $vswherePath = [System.Environment]::ExpandEnvironmentVariables("%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe")
  $vswhereParameters = @("-latest", "-products", "*", "-requires", "Microsoft.Component.MSBuild",  "-property", "installationPath")

  $vsPath = & $vswherePath $vswhereParameters
  
  Write-Host $path
  
  if ($vsPath) {
    $msbuildPath = join-path $vsPath 'MSBuild\14.0\Bin\MSBuild.exe'
  }
}

Write-Host $msbuildPath

& nuget restore "./SpecFlow.VisualStudio.$ExtensionVisualStudioVersion.sln"
& $msbuildPath "./SpecFlow.VisualStudio.$ExtensionVisualStudioVersion.sln" /property:Configuration=$Configuration /binaryLogger:msbuild.$Configuration.binlog /nodeReuse:false
