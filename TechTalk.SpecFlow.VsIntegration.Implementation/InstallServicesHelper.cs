using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.VsIntegration.TestRunner;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration
{
    public class InstallServicesHelper
    {
        private readonly DTE dte;
        private readonly InstallServices installServices;

        public InstallServicesHelper(InstallServices installServices, DTE dte)
        {
            this.installServices = installServices;
            this.dte = dte;
        }

        public void OnPackageUsed()
        {
            bool isSpecRunUsed = IsSpecRunUsed();
            installServices.OnPackageUsed(isSpecRunUsed);
        }

        private bool IsSpecRunUsed()
        {
            return VsxHelper.FindProject(dte, IsSpecRunProjectSafe) != null;
        }

        private static bool IsSpecRunProjectSafe(Project project)
        {
            try
            {
                return IsSpecRunProject(project);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsSpecRunProject(Project project)
        {
            return VsxHelper.GetReference(project, "TechTalk.SpecRun") != null ||
                   VsxHelper.GetReference(project, "SpecFlow.Plus.Runner") != null;
        }
    }
}
