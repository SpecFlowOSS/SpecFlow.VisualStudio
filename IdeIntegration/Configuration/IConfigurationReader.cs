using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration
{
    public interface IConfigurationReader
    {
        SpecFlowConfigurationHolder ReadConfiguration();
    }
}
