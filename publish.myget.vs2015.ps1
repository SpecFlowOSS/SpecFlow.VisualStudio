
$SystemArtifactsDirectory = $Env:SYSTEM_ARTIFACTSDIRECTORY;
$MyGetApiKey = $Env:MyGetApiKey;
$MyGetVsixFeed = $Env:MyGetVsixFeed;

$pathToExtension = "$SystemArtifactsDirectory\VsIntegration\bin\Release\TechTalk.SpecFlow.VsIntegration.2015.vsix";

$extensionFileContent = [System.IO.File]::ReadAllBytes($pathToExtension);

& Invoke-WebRequest -Uri "$MyGetVsixFeed/upload" -Method 'POST' -Body $extensionFileContent -Headers @{"X-NuGet-ApiKey"="$MyGetApiKey"} 
