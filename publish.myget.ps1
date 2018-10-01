
$SystemArtifactsDirectory = $Env:SYSTEM_ARTIFACTSDIRECTORY;
$MyGetApiKey = $Env:MyGetApiKey;
$MyGetVsixFeed = $Env:MyGetVsixFeed;
$VisualStudioVersion = $Env:VisualStudioVersion;

$pathToExtension = "$SystemArtifactsDirectory\SpecFlow.VisualStudio.VS$VisualStudioVersion\s\VsIntegration\bin\Release\TechTalk.SpecFlow.VsIntegration.$VisualStudioVersion.vsix";

$extensionFileContent = [System.IO.File]::ReadAllBytes("$pathToExtension");

& Invoke-WebRequest -Uri "$MyGetVsixFeed/upload" -Method 'POST' -Body $extensionFileContent -Headers @{"X-NuGet-ApiKey"="$MyGetApiKey"}
