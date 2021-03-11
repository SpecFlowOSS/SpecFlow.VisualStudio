using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;
using TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions;
using VSLangProj;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public class BindingFileInfo : FileInfo
    {
        public IEnumerable<IStepDefinitionBinding> StepBindings { get; set; }
        public IEnumerable<StepArgumentType> StepArgumentTypes { get; set; }

        public bool IsAssembly
        {
            get
            {
                var extension = Path.GetExtension(ProjectRelativePath);
                return (".dll".Equals(extension, StringComparison.InvariantCultureIgnoreCase) ||
                        ".exe".Equals(extension, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public BindingFileInfo(ProjectItem projectItem)
        {
            ProjectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
            LastChangeDate = VsxHelper.GetLastChangeDate(projectItem) ?? DateTime.MinValue;
        }

        public BindingFileInfo(Reference reference)
        {
            ProjectRelativePath = VsxHelper.GetProjectRelativePath(reference);
            LastChangeDate = VsxHelper.GetLastChangeDate(reference) ?? DateTime.MinValue;
        }
    }
}