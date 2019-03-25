using EnvDTE;
using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;
using VSLangProj;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public class BindingAssemblyInfo
    {
        public string AssemblyName { get; private set; }
        public Project Project { get; private set; }
        public Reference Reference { get; private set; }

        public bool IsProject
        {
            get { return Project != null; }
        }

        public bool IsAssemblyReference
        {
            get { return Reference != null; }
        }

        public string AssemblyShortName
        {
            get { return AssemblyName.Split(new[] { ',' }, 2)[0].Trim(); }
        }

        public BindingAssemblyInfo(Project project)
        {
            AssemblyName = project.GetAssemblyName();
            Project = project;
        }

        public BindingAssemblyInfo(string assemblyName, Project mainProject)
        {
            AssemblyName = assemblyName;
            Project = VsxHelper.FindProjectByAssemblyName(mainProject.DTE, AssemblyShortName);

            if (Project != null && VsProjectScope.GetTargetLanguage(Project) == ProgrammingLanguage.FSharp) //HACK: we force the f# libs to be used as assembly reference
                Project = null;

            if (Project == null)  
                Reference = VsxHelper.GetReference(mainProject, assemblyName);
        }
    }
}