using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
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
            var dte = (DTE)_serviceProvider.GetService(typeof(DTE));
            var nonGenericProjects = dte.Solution.Projects;
            var projects = nonGenericProjects.Cast<Project>();

            var targetFrameworks = projects.Where(p => p != null)
                                           .Where(p => p.Properties != null)
                                           .Select(
                                               p =>
                                               {
                                                   string tfm;
                                                   bool success = TryGetTargetFrameworkMonikers(p.Properties, out tfm);
                                                   return new { success, tfm };
                                               })
                                           .Where(r => r.success)
                                           .SelectMany(r => r.tfm.Split(';'))
                                           .Distinct();
            return targetFrameworks;
        }

        public bool TryGetTargetFrameworkMonikers(Properties properties, out string tfm)
        {
            bool successSdkTfm = TryGetTargetFrameworksForSdkStyleProj(properties, out tfm);
            if (successSdkTfm)
            {
                return true;
            }

            bool successOldProjTfm = TryGetTargetFrameworkForOldProj(properties, out tfm);
            return successOldProjTfm;
        }

        public bool TryGetTargetFrameworksForSdkStyleProj(Properties properties, out string tfm)
        {
            return VsxHelper.TryGetProperty(properties, "TargetFrameworkMonikers", out tfm);
        }

        public bool TryGetTargetFrameworkForOldProj(Properties properties, out string tfm)
        {
            return VsxHelper.TryGetProperty(properties, "TargetFrameworkMoniker", out tfm);
        }
    }
}
