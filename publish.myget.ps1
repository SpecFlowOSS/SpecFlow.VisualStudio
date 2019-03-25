param (
    [string] $folderToSearch
)


$MyGetApiKey = $Env:MyGetApiKey;
$MyGetVsixFeed = $Env:MyGetVsixFeed;

Get-ChildItem -Path $folderToSearch -Filter *.vsix -Recurse | 
ForEach-Object {
    $filename = $_.FullName
    $filecontent = Get-Content $_.FullName

	Write-Host "Uploading file $filename"

    
    & Invoke-WebRequest -Uri "$MyGetVsixFeed/upload" -Method 'POST' -Body $filecontent -Headers @{"X-NuGet-ApiKey"="$MyGetApiKey"}

    Write-Host "File was uploaded"
}

