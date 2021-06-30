using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    internal class InvalidFileBlock : GherkinFileBlock, IInvalidFileBlock
    {
        public InvalidFileBlock(int startLine, int endLine, params ErrorInfo[] errorInfos)
            : this(startLine, endLine - startLine, endLine - startLine, new ClassificationSpan[0], new ITagSpan<IOutliningRegionTag>[0], errorInfos ?? new ErrorInfo[0])
        {
            
        }

        public InvalidFileBlock(int startLine, int endLine, int blockRelativeContentEndLine, IEnumerable<ClassificationSpan> classificationSpans, IEnumerable<ITagSpan<IOutliningRegionTag>> outliningRegions, IEnumerable<ErrorInfo> errors) 
            : base(null, null, startLine, 0, endLine - startLine, blockRelativeContentEndLine, classificationSpans, outliningRegions, errors)
        {
        }
    }
}