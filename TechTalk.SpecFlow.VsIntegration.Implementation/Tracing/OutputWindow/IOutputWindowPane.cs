using System;

namespace TechTalk.SpecFlow.VsIntegration.Tracing.OutputWindow
{
    public interface IOutputWindowPane : IDisposable
    {
        string Name
        {
            get;
            set;
        }

        void Activate();
        void Hide();
        void Clear();
        void Write(string text);
        void WriteLine(string text);
    }
}