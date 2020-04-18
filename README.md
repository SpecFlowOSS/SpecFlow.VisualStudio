# SpecFlow.VisualStudio

Visual Studio extension of SpecFlow (extracted from the main SpecFlow repo).

For documentation, please refer to the SpecFlow website:
[http://www.specflow.org](http://www.specflow.org)

## Build status

Continuous Integration: [![Build Status](https://specflow.visualstudio.com/SpecFlow/_apis/build/status/SpecFlow.VisualStudio.CI)](https://specflow.visualstudio.com/SpecFlow/_build/latest?definitionId=3)

## Deployment status

Unit tests: ![Unit tests status](https://vsrm.dev.azure.com/specflow/_apis/public/Release/badge/4d755a95-f4b3-45f5-abb5-aeccc2b85d15/2/25)

Publish to MyGet: ![Publish to MyGet status](https://vsrm.dev.azure.com/specflow/_apis/public/Release/badge/4d755a95-f4b3-45f5-abb5-aeccc2b85d15/2/26)


## Build prerequisites

 - VS 2019
 - VS SDK


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
