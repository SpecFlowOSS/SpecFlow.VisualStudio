param (
	[parameter(mandatory)]
    [string] 
	$personalAccessToken,

	[parameter(mandatory)]
    [string] 
	$vsixPath,

	[parameter(mandatory)]
    [string] 
	$publishManifestPath
)

# Find the location of VsixPublisher
$vsInstallation = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -format json | ConvertFrom-Json
$vsInstallationPath = $vsInstallation.installationPath

Write-Host vsInstallationPath=$vsInstallationPath
$vsixPublisherPath = Join-Path -Path $vsInstallationPath -ChildPath "VSSDK\VisualStudioIntegration\Tools\Bin\VsixPublisher.exe" -Resolve

Write-Host vsixPublisherPath=$vsixPublisherPath

# Publish to VSIX to the marketplace
& $vsixPublisherPath publish -payload $vsixPath -publishManifest $publishManifestPath -personalAccessToken $personalAccessToken 
