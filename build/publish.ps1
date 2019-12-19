param (
 [string]$pat
)

Get-ChildItem

Push-Location $AGENT_RELEASEDIRECTORY
Get-ChildItem

Push-Location "TechTalk.SpecFlow.VisualStudioIntegration\s"
Get-ChildItem
Pop-Location

Pop-Location

$VsixPath = "$AGENT_RELEASEDIRECTORY\TechTalk.SpecFlow.VisualStudioIntegration\s\VsIntegration2015\bin\Release\TechTalk.SpecFlow.VsIntegration.2015.vsix"
$manifestPath = "$AGENT_RELEASEDIRECTORY\TechTalk.SpecFlow.VisualStudioIntegration\s\VsIntegration2015\PackageInformation\extensionmanifest.json"

# Find the location of VsixPublisher
$Installation = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -format json | ConvertFrom-Json
$Path = $Installation.installationPath

Write-Host $Path
$VsixPublisher = Join-Path -Path $Path -ChildPath "VSSDK\VisualStudioIntegration\Tools\Bin\VsixPublisher.exe" -Resolve

Write-Host $VsixPublisher

# -ignoreWarnings "VSIXValidatorWarning01,VSIXValidatorWarning02,VSIXValidatorWarning08"
& $VsixPublisher publish -payload $VsixPath -publishManifest $manifestPath -personalAccessToken $pat