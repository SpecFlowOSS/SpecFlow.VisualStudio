# SpecFlow.VisualStudio

Visual Studio extension of SpecFlow (extracted from the main SpecFlow repo).

For documentation, please refer to the SpecFlow website: 
[http://www.specflow.org](http://www.specflow.org)

## Build prerequisites

### VS 2013 Extension
- Visual Studio 2013, any edition but Express
- Visual Studio 2013 SDK

Please use SpecFlow.VisualStudio.2013.sln as Solution file.

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
Currently there are only differences in the TechTalk.SpecFlow.VsIntegration.*.csproj. If you change one, please update the other two also.

## Debugging

To start the experimental instance of Visual Studio configure "Start external program" and Command line arguments on the "Debug"- Tab in the property window of the VSIntegration project.

**Start external program:**

- Visual Studio 2013: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
- Visual Studio 2015: C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe
- Visual Studio 2017: C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe


**Command line args for all Visual Studio Versions:**  
/RootSuffix Exp

