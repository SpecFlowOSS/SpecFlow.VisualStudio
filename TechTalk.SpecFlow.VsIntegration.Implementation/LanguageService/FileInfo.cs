using System;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public abstract class FileInfo
    {
        public bool IsAnalyzed { get; set; }
        public bool IsError { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string ProjectRelativePath { get; set; }

        public virtual void Rename(string newProjectRelativePath)
        {
            ProjectRelativePath = newProjectRelativePath;
        }

        public bool IsDirty(DateTime timeStamp)
        {
            return LastChangeDate > timeStamp.AddMilliseconds(0.5);
        }
    }
}