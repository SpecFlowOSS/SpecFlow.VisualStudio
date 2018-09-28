using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechTalk.SpecFlow.IdeIntegration.Configuration
{
    public interface IConfigurationReader
    {
        ConfigurationHolder ReadConfiguration();
    }
}
