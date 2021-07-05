using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public interface IGherkinFileBlock
    {
        /// <summary>
        /// The keyword as it was specified in the file, including the tailing colon and space.
        /// </summary>
        string Keyword { get; }

        /// <summary>
        /// The title of the block as a direct concatenation of the <see cref="Keyword"/> in the file (no space trimming in front). 
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The absolute line number (zero-indexed) of the line containing the <see cref="Keyword"/>.
        /// </summary>
        int KeywordLine { get; }

        /// <summary>
        /// A line number relative to <see cref="KeywordLine"/> specifying the first line of the block (can be negative).
        /// </summary>
        int BlockRelativeStartLine { get; }

        /// <summary>
        /// A line number relative to <see cref="KeywordLine"/> specifying the last line of the block (can be zero).
        /// </summary>
        int BlockRelativeEndLine { get; }

        /// <summary>
        /// A line number relative to <see cref="KeywordLine"/> specifying the last line of the block containing important (non-comment) content (can be zero).
        /// </summary>
        int BlockRelativeContentEndLine { get; }

        /// <summary>
        /// The coloring information for this block (null, if no coloring).
        /// </summary>
        IEnumerable<ClassificationSpan> ClassificationSpans { get; }

        /// <summary>
        /// The outlining information for this block (null, if no outlining).
        /// </summary>
        IEnumerable<ITagSpan<IOutliningRegionTag>> OutliningRegions { get; }

        /// <summary>
        /// Any parsing errors in this block.
        /// </summary>
        IEnumerable<ErrorInfo> Errors { get; }
    }
}