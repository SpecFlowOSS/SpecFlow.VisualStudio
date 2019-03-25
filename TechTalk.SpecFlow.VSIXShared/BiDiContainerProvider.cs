using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoDi;

namespace TechTalk.SpecFlow.VsIntegration.Implementation
{
    [Export(typeof(IBiDiContainerProvider))]
    public class BiDiContainerProvider : IBiDiContainerProvider
    {
        public static void Init(IObjectContainer objectContainer)
        {
            CurrentContainer = objectContainer;
        }

        public static IObjectContainer CurrentContainer { get; private set; }

        public IObjectContainer ObjectContainer
        {
            get { return CurrentContainer; }
        }
    }
}
