using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Analytics;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public class VisualStudioProjectTargetFrameworksProvider : IProjectTargetFrameworksProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public VisualStudioProjectTargetFrameworksProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<string> GetProjectTargetFrameworks()
        {
            var dte = _serviceProvider.GetService<DTE>();
            var nonGenericProjects = dte.Solution.Projects;
            var projects = nonGenericProjects.Cast<Project>();
            var targetFrameworks = projects.Select(p => p.Properties.Item("TargetFramework"))
                                           .Select(p => p.Value)
                                           .Cast<string>();
            return targetFrameworks;
        }
    }
}
