param (
 [string]$Configuration = "Debug",
 [string]$SpecFlowVisualStudioVersion = "",
 [string]$binaryLoggerSwitch = "/binaryLogger"
)

$msbuildPath = "msbuild"

Write-Host "Visual Studio version: $SpecFlowVisualStudioVersion";

if (![System.String]::IsNullOrEmpty($Env:MSBuild))
{
  $msbuildPath = join-path $Env:MSBuild 'msbuild.exe';
  Write-Host "Using msbuild from environment variable";
}
elseif ($SpecFlowVisualStudioVersion -eq "2019")
{
  $msbuildPath = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Preview\MSBuild\Current\Bin\MSBuild.exe";
  $binaryLoggerSwitch = "/binaryLogger:msbuild.$Configuration.binlog";
}
else
{
  throw [System.NotSupportedException]::new("The Visual Studio version $SpecFlowVisualStudioVersion is not supported.");
}

Write-Host "MSBuild path: $msbuildPath"


& nuget restore "./SpecFlow.VisualStudio.$SpecFlowVisualStudioVersion.sln"
& $msbuildPath "./SpecFlow.VisualStudio.$SpecFlowVisualStudioVersion.sln" $binaryLoggerSwitch /property:Configuration=$Configuration  /nodeReuse:false
