param (
 [string]$Configuration = "Debug",
 [string]$ExtensionVisualStudioVersion = "2017",
 [string]$binaryLoggerSwitch = "/binaryLogger"
)

$msbuildPath = "msbuild"

if (![System.String]::IsNullOrEmpty($Env:MSBuild))
{
  $msbuildPath = join-path $Env:MSBuild 'msbuild.exe';
  Write-Host "Using msbuild from environment variable";
}
elseif ($ExtensionVisualStudioVersion -eq "2017")
{
  $msbuildPath = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe";
  $binaryLoggerSwitch = "/binaryLogger:msbuild.$Configuration.binlog";
}
elseif ($ExtensionVisualStudioVersion -eq "2015")
{
  $msbuildPath = "C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe";
  $binaryLoggerSwitch = "";
}
else
{
  throw [System.NotSupportedException]::new("The Visual Studio version $ExtensionVisualStudioVersion is not supported.");
}

Write-Host "MSBuild path: $msbuildPath"


& nuget restore "./SpecFlow.VisualStudio.$ExtensionVisualStudioVersion.sln"
& $msbuildPath "./SpecFlow.VisualStudio.$ExtensionVisualStudioVersion.sln" $binaryLoggerSwitch /property:Configuration=$Configuration  /nodeReuse:false
