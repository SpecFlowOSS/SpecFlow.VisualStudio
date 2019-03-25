using TechTalk.SpecFlow.VsIntegration.Implementation.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Analytics
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
