using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Analytics
{
    public class VisualStudioIdeInformationStore : IIdeInformationStore
    {
        private const string IdeName = "Microsoft Visual Studio";

        public string GetName()
        {
            return IdeName;
        }

        public string GetVersion()
        {
            return VSVersion.FullVersion.ToString();
        }
    }
}
