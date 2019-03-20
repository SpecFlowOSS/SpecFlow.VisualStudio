param (
 [string]$Configuration = "Debug"
)

$msbuildPath = "msbuild"

Write-Host $IsWindows

if ($IsWindows){
  $vswherePath = [System.Environment]::ExpandEnvironmentVariables("%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe")
  $vswhereParameters = @("-latest", "-products", "*", "-requires", "Microsoft.Component.MSBuild",  "-property", "installationPath")
  
  $vsPath = & $vswherePath $vswhereParameters
  
  Write-Host $vsPath
  
  if ($vsPath) {
    $msbuildPath = join-path $vsPath 'MSBuild\Current\Bin\MSBuild.exe'
  }
}

Write-Host $msbuildPath

& $msbuildPath -restore ./SpecFlow.VisualStudio.sln -property:Configuration=$Configuration -binaryLogger:msbuild.$Configuration.binlog -nodeReuse:false