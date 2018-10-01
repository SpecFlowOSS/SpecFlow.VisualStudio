
$SystemArtifactsDirectory = $Env:SYSTEM_ARTIFACTSDIRECTORY;
$MyGetApiKey = $Env:MyGetApiKey;
$MyGetVsixFeed = $Env:MyGetVsixFeed;

& curl -X POST --verbose --data-binary \@"$SystemArtifactsDirectory\VsIntegration\bin\Release\TechTalk.SpecFlow.VsIntegration.2015.vsix" -H "X-NuGet-ApiKey: $MyGetApiKey" "$MyGetVsixFeed/upload"
