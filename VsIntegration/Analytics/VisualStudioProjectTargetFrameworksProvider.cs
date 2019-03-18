using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.VsIntegration.Utils;

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

            var targetFrameworks = projects.Where(p => p != null)
                                           .Where(p => p.Properties != null)
                                           .Select(
                                               p =>
                                               {
                                                   string tfm;
                                                   bool success = VsxHelper.TryGetProperty(p.Properties, "TargetFrameworkMonikers", out tfm);
                                                   return new { success, tfm };
                                               })
                                           .Where(r => r.success)
                                           .Select(r => r.tfm);
            return targetFrameworks;
        }
    }
}
