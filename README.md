# SpecFlow.VisualStudio

[![Visual Studio Marketplace Installs](https://img.shields.io/visual-studio-marketplace/i/TechTalkSpecFlowTeam.SpecFlowForVisualStudio?label=installs%20VS2019)](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowForVisualStudio)
[![Visual Studio Marketplace Installs](https://img.shields.io/visual-studio-marketplace/i/TechTalkSpecFlowTeam.SpecFlowForVisualStudio2017?label=installs%20VS2017)](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowForVisualStudio2017)
[![Visual Studio Marketplace Installs](https://img.shields.io/visual-studio-marketplace/i/TechTalkSpecFlowTeam.SpecFlowForVisualStudio2015?label=installs%20VS2015)](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowForVisualStudio2015)

SpecFlow extension for Visual Studio 2019 and earlier releases (extracted from the main SpecFlow repo).

For documentation, please refer to the SpecFlow website:
[https://www.specflow.org](https://www.specflow.org)

## Build status

Continuous Integration: [![Build Status](https://specflow.visualstudio.com/SpecFlow/_apis/build/status/SpecFlow.VisualStudio.CI)](https://specflow.visualstudio.com/SpecFlow/_build/latest?definitionId=3)

## Deployment status

Unit tests: ![Unit tests status](https://vsrm.dev.azure.com/specflow/_apis/public/Release/badge/4d755a95-f4b3-45f5-abb5-aeccc2b85d15/2/25)

Publish to MyGet: ![Publish to MyGet status](https://vsrm.dev.azure.com/specflow/_apis/public/Release/badge/4d755a95-f4b3-45f5-abb5-aeccc2b85d15/2/26)

MyGet Feed: <https://www.myget.org/F/specflow-vsix/vsix/>

## Supported platforms

### Visual Studio

* [Visual Studio 2019 (Community, Personal, Enterprise)](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowForVisualStudio)
* [Visual Studio 2017 (Community, Personal, Enterprise)](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowforVisualStudio2017)
* [Visual Studio 2015 (Community, Personal, Enterprise)](https://marketplace.visualstudio.com/items?itemName=TechTalkSpecFlowTeam.SpecFlowforVisualStudio2015)

## Build prerequisites

 - Visual Studio 2019
 - Visual Studio SDK

## Installation

The extension can be installed directly from Visual Studio using the extension manager. See detailed instructions at the [Installation documentation page](https://docs.specflow.org/projects/specflow/en/latest/visualstudio/visual-studio-installation.html).

Please also help other users by rating the extension at the Visual Studio Marketplace using the links under Supported Platforms.

## Developing

Use SpecFlow.VisualStudio.sln.

## Debugging

To start the experimental instance of Visual Studio configure "Start external program" and Command line arguments on the "Debug"- Tab in the property window of the VSIntegration project.

**Start external program:**

- Visual Studio 2015: C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe
- Visual Studio 2017: C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe
- Visual Studio 2019: C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe

**Command line args for all Visual Studio Versions:**  
/RootSuffix Exp

## Contributing

All contributors are welcome! For more information see the [Contribution guidelines](CONTRIBUTION.md)

## Find solutions, share ideas and engage in discussions

* Join our community forum: <https://support.specflow.org/>
* Join our Discord channel: <https://discord.com/invite/xQMrjDXx7a>
* Follow us on Twitter: <https://twitter.com/specflow>
* Follow us on LinkedIn: <https://www.linkedin.com/company/specflow/>
* Subscribe on YouTube: <https://www.youtube.com/c/SpecFlowBDD>
* Join our next webinar: <https://specflow.org/community/webinars/>

## License

SpecFlow for VisualStudio is licensed under the [MIT license](LICENSE).

Copyright (c) 2019-2021 Gaspar Nagy (Spec Solutions), Tricentis GmbH

The extension is built based on the [Deveroom for SpecFlow](https://github.com/specsolutions/deveroom-visualstudio) Visual Studio extension, created by Gaspar Nagy (Spec Solutions).
