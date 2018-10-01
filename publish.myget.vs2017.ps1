
$SystemArtifactsDirectory = $Env:SYSTEM_ARTIFACTSDIRECTORY;
$MyGetApiKey = $Env:MyGetApiKey;
$MyGetVsixFeed = $Env:MyGetVsixFeed;

$pathToExtension = "$SystemArtifactsDirectory\SpecFlow.VisualStudio.VS2017\s\VsIntegration\bin\Release\TechTalk.SpecFlow.VsIntegration.2017.vsix";

$extensionFileContent = [System.IO.File]::ReadAllBytes("$pathToExtension");

& Invoke-WebRequest -Uri "$MyGetVsixFeed/upload" -Method 'POST' -Body $extensionFileContent -Headers @{"X-NuGet-ApiKey"="$MyGetApiKey"}
