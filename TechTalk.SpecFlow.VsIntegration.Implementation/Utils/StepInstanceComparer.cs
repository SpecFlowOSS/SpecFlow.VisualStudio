using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.VsIntegration.Implementation.StepSuggestions;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Utils
{
    internal class StepInstanceComparer : IEqualityComparer<StepInstance>, IComparer<StepInstance>
    {
        public static readonly StepInstanceComparer Instance = new StepInstanceComparer();

        public bool Equals(StepInstance si1, StepInstance si2)
        {
            var sp1 = (ISourceFilePosition)si1;
            var sp2 = (ISourceFilePosition)si2;
            return sp1.SourceFile.Equals(sp2.SourceFile, StringComparison.InvariantCultureIgnoreCase) && sp1.Location.Line == sp2.Location.Line;
        }

        public int GetHashCode(StepInstance obj)
        {
            return ((ISourceFilePosition)obj).SourceFile.GetHashCode();
        }

        public int Compare(StepInstance si1, StepInstance si2)
        {
            var sp1 = (ISourceFilePosition)si1;
            var sp2 = (ISourceFilePosition)si2;

            int result = StringComparer.InvariantCultureIgnoreCase.Compare(sp1.SourceFile, sp2.SourceFile);
            if (result == 0)
                result = sp1.Location.Line.CompareTo(sp2.Location.Line);
            return result;
        }
    }
}
