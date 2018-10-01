
& curl -X POST --verbose --data-binary \@"$(SYSTEM_ARTIFACTSDIRECTORY)\s\VsIntegration\bin\Release\TechTalk.SpecFlow.VsIntegration.2017.vsix" -H "X-NuGet-ApiKey: $(MyGetApiKey)" "$(MyGetVsixFeed)/upload"
