# SpecFlow.VisualStudio

Visual Studio extension of SpecFlow (extracted from the main SpecFlow repo).

For documentation, please refer to the SpecFlow website:
[http://www.specflow.org](http://www.specflow.org)

## Build status

Continuous Integration: [![Build Status](https://specflow.visualstudio.com/SpecFlow/_apis/build/status/SpecFlow.VisualStudio.CI)](https://specflow.visualstudio.com/SpecFlow/_build/latest?definitionId=3)

## Deployment status

### Visual Studio 2015 extension

Unit tests: ![Unit tests status](https://specflow.vsrm.visualstudio.com/_apis/public/Release/badge/4d755a95-f4b3-45f5-abb5-aeccc2b85d15/2/8)

Publish to MyGet: ![Publish to MyGet status](https://specflow.vsrm.visualstudio.com/_apis/public/Release/badge/4d755a95-f4b3-45f5-abb5-aeccc2b85d15/2/9)

### Visual Studio 2017 extension

Unit tests: ![Unit tests status](https://specflow.vsrm.visualstudio.com/_apis/public/Release/badge/4d755a95-f4b3-45f5-abb5-aeccc2b85d15/2/6)

Publish to MyGet: ![Publish to MyGet status](https://specflow.vsrm.visualstudio.com/_apis/public/Release/badge/4d755a95-f4b3-45f5-abb5-aeccc2b85d15/2/7)

## Build prerequisites

### VS 2015 Extension

- Visual Studio 2015
- Visual Studio 2015 SDK

Please use SpecFlow.VisualStudio.2015.sln as Solution file.

### VS 2017 Extension

- Visual Studio 2017
- Visual Studio 2017 SDK

Please use SpecFlow.VisualStudio.2017.sln as Solution file.

## Developing

For general fixes & features you can choose the Visual Studio version of your choice.
Currently there are only differences in the TechTalk.SpecFlow.VsIntegration.*.csproj. If you change one, please update the other two as well.

## Debugging

To start the experimental instance of Visual Studio configure "Start external program" and Command line arguments on the "Debug"- Tab in the property window of the VSIntegration project.

**Start external program:**

- Visual Studio 2015: C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe
- Visual Studio 2017: C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe

**Command line args for all Visual Studio Versions:**  
/RootSuffix Exp
