using System;
using EnvDTE;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
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