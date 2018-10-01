param (
 [string]$Configuration = "Debug"
)

$VisualStudioVersion = $Env:VisualStudioVersion;
$msbuildPath = "msbuild"

if ([Environment]::OSVersion.Platform -eq "Win32NT"){
  $vswherePath = [System.Environment]::ExpandEnvironmentVariables("%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe")
  $vswhereParameters = @("-latest", "-products", "*", "-requires", "Microsoft.Component.MSBuild",  "-property", "installationPath")

  $vsPath = & $vswherePath $vswhereParameters
  
  Write-Host $path
  
  if ($vsPath) {
    $msbuildPath = join-path $vsPath 'MSBuild\14.0\Bin\MSBuild.exe'
  }
  
  Write-Host $msbuildPath
}

& nuget restore ./SpecFlow.VisualStudio.2017.sln
& $msbuildPath /Restore ./SpecFlow.VisualStudio.2017.sln /property:Configuration=$Configuration /binaryLogger:msbuild.$Configuration.binlog /nodeReuse:false
