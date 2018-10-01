param (
 [string]$Configuration = "Debug"
)

$msbuildPath = "C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"


# if ([Environment]::OSVersion.Platform -eq "Win32NT"){
#   $vswherePath = [System.Environment]::ExpandEnvironmentVariables("%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe")
#   $vswhereParameters = @("-latest", "-products", "*", "-requires", "Microsoft.Component.MSBuild",  "-property", "installationPath")
  
#   $vsPath = & $vswherePath $vswhereParameters
  
#   Write-Host $path
  
#   if ($vsPath) {
#     $msbuildPath = join-path $vsPath 'MSBuild\15.0\Bin\MSBuild.exe'
#   }
  
#   Write-Host $msbuildPath
# }

& nuget restore ./SpecFlow.VisualStudio.2015.sln
& $msbuildPath ./SpecFlow.VisualStudio.2015.sln /property:Configuration=$Configuration /binaryLogger:msbuild.$Configuration.binlog /nodeReuse:false
