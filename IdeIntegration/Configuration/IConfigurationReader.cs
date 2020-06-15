using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration
{
    public interface IConfigurationReader
    {
        ISpecFlowConfigurationHolder ReadConfiguration();
    }
}
