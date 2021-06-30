using TechTalk.SpecFlow.IdeIntegration.Utils;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics
{
    public class VisualStudioIdeInformationStore : IIdeInformationStore
    {
        public string GetName()
        {
            return "Microsoft Visual Studio";
        } 

        public string GetVersion()
        {
            return VSVersion.FullVersion.ToString();
        }
    }
}
