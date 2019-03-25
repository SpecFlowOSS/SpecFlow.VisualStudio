using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public class ScenarioOutlineExamplesTable : Table, ITableWithRowPositions
    {
        private readonly Dictionary<int, int> blockRelativeLines = new Dictionary<int, int>();

        public ScenarioOutlineExamplesTable(string[] cells) : base(cells)
        {
        }

        public ScenarioOutlineExamplesRow GetExamplesRow(int rowIndex)
        {
            int blockRelativeLine;
            if (!blockRelativeLines.TryGetValue(rowIndex, out blockRelativeLine))
                blockRelativeLine = -1;
            return new ScenarioOutlineExamplesRow(Rows[rowIndex], blockRelativeLine);
        }

        public void SetBlockRelativePosition(int rowIndex, int blockRelativeLine)
        {
            blockRelativeLines[rowIndex] = blockRelativeLine;
        }

        public ScenarioOutlineExamplesRow FindByBlockRelativeLine(int blockRelativeLine)
        {
            var selectedLines = blockRelativeLines.Where(r2l => r2l.Value == blockRelativeLine).Select(r2l => r2l.Key).ToArray();
            if (selectedLines.Length == 0)
                return null;
            return GetExamplesRow(selectedLines[0]);
        }
    }
}