param (
 [string]$Configuration = "Debug",
 [string]$VisualStudioVersion = "",
 [string]$binaryLoggerSwitch = "/binaryLogger"
)

$msbuildPath = "msbuild"

$VisualStudioVersion =
  if (![System.String]::IsNullOrWhiteSpace($VisualStudioVersion)) { $VisualStudioVersion }
  else { $Env:VISUALSTUDIOVERSION };

Write-Host "Visual Studio version: $VisualStudioVersion";

if (![System.String]::IsNullOrEmpty($Env:MSBuild))
{
  $msbuildPath = join-path $Env:MSBuild 'msbuild.exe';
  Write-Host "Using msbuild from environment variable";
}
elseif ($VisualStudioVersion -eq "2017")
{
  $msbuildPath = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe";
  $binaryLoggerSwitch = "/binaryLogger:msbuild.$Configuration.binlog";
}
elseif ($VisualStudioVersion -eq "2015")
{
  $msbuildPath = "C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe";
  $binaryLoggerSwitch = "";
}
else
{
  throw [System.NotSupportedException]::new("The Visual Studio version $VisualStudioVersion is not supported.");
}

Write-Host "MSBuild path: $msbuildPath"


& nuget restore "./SpecFlow.VisualStudio.$VisualStudioVersion.sln"
& $msbuildPath "./SpecFlow.VisualStudio.$VisualStudioVersion.sln" $binaryLoggerSwitch /property:Configuration=$Configuration  /nodeReuse:false
