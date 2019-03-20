
$SystemArtifactsDirectory = $Env:SYSTEM_ARTIFACTSDIRECTORY;
$MyGetApiKey = $Env:MyGetApiKey;
$MyGetVsixFeed = $Env:MyGetVsixFeed;

Get-ChildItem -Path $SystemArtifactsDirectory -Filter *.vsix | 
ForEach-Object {
    $filename = Get-Content $_.FullName

    $extensionFileContent = [System.IO.File]::ReadAllBytes("$filename");

    & Invoke-WebRequest -Uri "$MyGetVsixFeed/upload" -Method 'POST' -Body $extensionFileContent -Headers @{"X-NuGet-ApiKey"="$MyGetApiKey"}
}

