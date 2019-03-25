using System.Collections.Generic;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public class ScenarioOutlineExamplesRow : Dictionary<string, string>
    {
        public int BlockRelativeLine { get; private set; }

        public ScenarioOutlineExamplesRow(TableRow tableRow, int blockRelativeLine) : base(tableRow)
        {
            BlockRelativeLine = blockRelativeLine;
        }
    }
}