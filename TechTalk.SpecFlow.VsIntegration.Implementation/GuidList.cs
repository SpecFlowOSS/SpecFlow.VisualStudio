using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.VsIntegration
{
    public static class GuidList
    {
        public const string vsContextGuidSilverlightProject = "{CB22EE0E-4072-4ae7-96E2-90FCCF879544}";

        public const string guidSpecFlowPkgString = "5d978b7f-8f91-41c1-b7ba-0b4c056118e8";
        public const string guidSpecFlowCmdSetString = "8c202d78-762d-4079-ac0e-282ee24b44b0";

        public const string ProductId = "30F08A29-D27E-42FF-92D3-50391313A1EF";

        public static readonly Guid guidSpecFlowCmdSet = new Guid(guidSpecFlowCmdSetString);
    };
}
