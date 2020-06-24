using System;
using EnvDTE;
using Gherkin.Ast;
using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public class FeatureFileInfo : FileInfo
    {
        public Version GeneratorVersion { get; set; }
        public Feature ParsedFeature { get; set; }

        public FeatureFileInfo(ProjectItem projectItem, ProjectItem codeBehindItem)
        {
            ProjectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
            var codeBehindItemChangeDate = VsxHelper.GetLastChangeDate(codeBehindItem) ?? DateTime.MinValue;
            var featureFileLastChangeDate = VsxHelper.GetLastChangeDate(projectItem) ?? DateTime.MinValue;
            LastChangeDate = featureFileLastChangeDate > codeBehindItemChangeDate ? featureFileLastChangeDate : codeBehindItemChangeDate;
        }
    }
}