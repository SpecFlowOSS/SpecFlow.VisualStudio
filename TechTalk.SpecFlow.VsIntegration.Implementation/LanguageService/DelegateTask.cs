using System;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public class DelegateTask : IGherkinProcessingTask
    {
        private readonly Action task;
        private readonly string name;

        public DelegateTask(Action task, string name)
        {
            this.task = task;
            this.name = name;
        }

        public void Apply()
        {
            task();
        }

        public IGherkinProcessingTask Merge(IGherkinProcessingTask other)
        {
            return null;
        }

        public override string ToString()
        {
            return name;
        }
    }
}