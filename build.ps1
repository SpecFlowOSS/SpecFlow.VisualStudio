param (
 [string]$Configuration = "Debug",
 [string]$appInsightsInstrumentationKey = ""
)

$msbuildPath = "msbuild"

Write-Host $IsWindows
Write-Host "Visual Studio version: $SpecFlowVisualStudioVersion";
Write-Host ($appInsightsInstrumentationKey -eq "")

if ($IsWindows){
  $vswherePath = [System.Environment]::ExpandEnvironmentVariables("%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe")
  $vswhereParameters = @("-latest", "-products", "*", "-requires", "Microsoft.Component.MSBuild",  "-property", "installationPath", "-prerelease")
  
  Write-Host $vswherePath
  Write-Host $vswhereParameters

  $vsPath = & $vswherePath $vswhereParameters
  
  Write-Host $vsPath
  
  if ($vsPath) {
    $msbuildPath = join-path $vsPath 'MSBuild\Current\Bin\MSBuild.exe'
  }
}

Write-Host $msbuildPath

& nuget restore "./SpecFlow.VisualStudio.sln"
& $msbuildPath ./SpecFlow.VisualStudio.sln -property:Configuration=$Configuration -binaryLogger:msbuild.$Configuration.binlog -nodeReuse:false "-property:AppInsightsInstrumentationKey=$appInsightsInstrumentationKey"
