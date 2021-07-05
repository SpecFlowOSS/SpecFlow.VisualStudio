using System.Collections.Generic;
using EnvDTE;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public interface IProjectScopeFactory
    {
        IProjectScope GetProjectScope(Project project);
        IEnumerable<IProjectScope> GetProjectScopesFromBindingProject(Project bindingProject);
    }
}