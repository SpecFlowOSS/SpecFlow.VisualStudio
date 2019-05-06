using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.EditorCommands
{
    internal class FormatDocumentCommand
    {
        private const string FeatureIndent = "";
        private const string ScenarioIndent = "";
        private const string StepIndent = "\t";
        private const string TableIndent = "\t\t";
        private const string MultilineIndent = "\t\t";
        private const string ExampleIndent = "\t";

        private const string CommentSymbol = "#";
        private const string TableSeparator = "|";
        private const string TagSymbol = "@";
        private const string MultilineArgumentDelimeter = "\"\"\"";

        private bool _normalizeLineBreaks;
        private int _lineBreaksBeforeStep;
        private int _lineBreaksBeforeScenario;
        private int _lineBreaksBeforeExamples;
        private int _lineBreaksBeforeFeature;


        public bool FormatDocument(GherkinEditorContext editorContext)
        {
            ConfigureLineBreakOptions(editorContext);

            var dialect = GetDialect(editorContext.LanguageService);
            var textSnapshot = editorContext.TextView.TextSnapshot;

            var textLines = textSnapshot.Lines
                .Select(line => line.GetText())
                .ToList();

            var formattedTextLines = FormatText(textLines, dialect);
            ReplaceText(textSnapshot, formattedTextLines);

            return true;
        }

        private void ConfigureLineBreakOptions(GherkinEditorContext editorContext)
        {
            var options = editorContext.LanguageService.ProjectScope.IntegrationOptionsProvider.GetOptions();
            _normalizeLineBreaks = options.NormalizeLineBreaks;
            _lineBreaksBeforeStep = options.LineBreaksBeforeStep;
            _lineBreaksBeforeScenario = options.LineBreaksBeforeScenario;
            _lineBreaksBeforeExamples = options.LineBreaksBeforeExamples;
            _lineBreaksBeforeFeature = options.LineBreaksBeforeFeature;
        }

        private void ReplaceText(ITextSnapshot textSnapshot, List<string> newTextLines)
        {
            //Replacing text line-by-line preserves scroll and caret position
            using (var edit = textSnapshot.TextBuffer.CreateEdit())
            {
                var currentLines = textSnapshot.Lines.ToList();

                int k;
                //Replace line-by-line
                for (k = 0; k < currentLines.Count && k < newTextLines.Count; k++)
                {
                    var line = currentLines[k];
                    if (line.GetText() != newTextLines[k])
                    {
                        var span = new SnapshotSpan(line.Start, line.End);
                        edit.Replace(span, newTextLines[k]);
                    }
                }

                //Replace anything left
                var lastLine = currentLines[k - 1];
                var endSpan = new SnapshotSpan(lastLine.End, currentLines.Last().EndIncludingLineBreak);
                string remainingText = newTextLines.Count > k
                    ? Environment.NewLine + string.Join(Environment.NewLine, newTextLines.Skip(k))
                    : string.Empty;
                if (endSpan.GetText() != remainingText)
                {
                    edit.Replace(endSpan, remainingText);
                }

                edit.Apply();
            }
        }

        private List<string> FormatText(List<string> textLines, GherkinDialect dialect)
        {
            var formattedTextLines = new List<string>();

            var stringsToInsertBefore = new List<string>();
            for (int i = 0; i < textLines.Count; i++)
            {
                string trimmedLine = textLines[i].Trim();

                if (string.IsNullOrWhiteSpace(textLines[i]))
                {
                    if (!_normalizeLineBreaks)
                    {
                        formattedTextLines.AddRange(stringsToInsertBefore);
                        stringsToInsertBefore.Clear();
                        formattedTextLines.Add(string.Empty);
                    }
                }
                else if (IsCommentLine(trimmedLine) || IsTagLine(trimmedLine))
                {
                    // Commment or tag lines should have same indent as following line,
                    // that's why we put them to temporary collection
                    stringsToInsertBefore.Add(trimmedLine);
                }
                else if (IsTableLine(trimmedLine))
                {
                    formattedTextLines.AddRange(stringsToInsertBefore.Select(str => TableIndent + str));
                    stringsToInsertBefore.Clear();

                    //Find whole table, and format it using FormatTableCommand.FormatTableString
                    var tableLines = new List<string>();
                    tableLines.Add(TableIndent + trimmedLine);
                    while (i + 1 < textLines.Count && IsTableLine(textLines[i + 1]))
                    {
                        i++;
                        tableLines.Add(textLines[i]);
                    }
                    var formattedTable =
                        FormatTableCommand.FormatTableString(string.Join(Environment.NewLine, tableLines));
                    formattedTextLines.AddRange(formattedTable.Split(new[] { Environment.NewLine },
                        StringSplitOptions.None));
                }
                else if (IsMultilineDelimeterLine(trimmedLine))
                {
                    formattedTextLines.AddRange(stringsToInsertBefore.Select(str => MultilineIndent + str));
                    stringsToInsertBefore.Clear();

                    formattedTextLines.Add(MultilineIndent + trimmedLine);

                    //Find original indent of multiline argument
                    int whitespaces = 0;
                    while (char.IsWhiteSpace(textLines[i][whitespaces]))
                    {
                        whitespaces++;
                    }
                    string originalIndent = textLines[i].Substring(0, whitespaces);

                    while (!IsMultilineDelimeterLine(textLines[++i]))
                    {
                        string formattedLine;
                        if (textLines[i].StartsWith(originalIndent))
                        {
                            //replace original indent with MultilineIndent
                            formattedLine = MultilineIndent + textLines[i].Substring(originalIndent.Length).TrimEnd();
                        }
                        else
                        {
                            //invalid case - leave it as it is
                            formattedLine = textLines[i];
                        }
                        formattedTextLines.Add(formattedLine);
                    }

                    formattedTextLines.Add(MultilineIndent + textLines[i].Trim());
                }
                else if (IsBlockLine(trimmedLine, dialect) || IsStepLine(trimmedLine, dialect))
                {
                    if (_normalizeLineBreaks)
                    {
                        int addLinesBefore = GetPreceedingLineBreaks(trimmedLine, dialect);
                        for (int j = 0; j < addLinesBefore; j++)
                        {
                            formattedTextLines.Add(string.Empty);
                        }
                    }
                    string indent = GetIndent(trimmedLine, dialect);

                    formattedTextLines.AddRange(stringsToInsertBefore.Select(str => indent + str));
                    stringsToInsertBefore.Clear();

                    formattedTextLines.Add(indent + trimmedLine);
                }
                else
                {
                    //Other lines - leave unchanged
                    formattedTextLines.AddRange(stringsToInsertBefore);
                    stringsToInsertBefore.Clear();

                    formattedTextLines.Add(textLines[i]);
                }
            }

            formattedTextLines.AddRange(stringsToInsertBefore);
            stringsToInsertBefore.Clear();
            return formattedTextLines;
        }

        private string GetIndent(string line, GherkinDialect dialect)
        {
            if (IsBlockLine(line, dialect))
            {
                var keyword = GetBlockKeyword(line, dialect);
                switch (keyword)
                {
                    case GherkinBlockKeyword.Scenario:
                    case GherkinBlockKeyword.ScenarioOutline:
                    case GherkinBlockKeyword.Background:
                        return ScenarioIndent;

                    case GherkinBlockKeyword.Examples:
                        return ExampleIndent;

                    case GherkinBlockKeyword.Feature:
                        return FeatureIndent;
                }
            }
            else if (IsStepLine(line, dialect))
            {
                return GetConfiguredStepLineBreaksAndIndent();
            }
            else if (IsTableLine(line))
            {
                return TableIndent;
            }

            return string.Empty;
        }

        private string GetConfiguredStepLineBreaksAndIndent()
        {
            var str = string.Empty;

            if (_normalizeLineBreaks)
            {
                for (var i = 0; i < _lineBreaksBeforeStep; i++)
                {
                    str = str + Environment.NewLine;
                }
            }

            return str + StepIndent;
        }

        private int GetPreceedingLineBreaks(string line, GherkinDialect dialect)
        {
            if (IsBlockLine(line, dialect))
            {
                var keyword = GetBlockKeyword(line, dialect);
                switch (keyword)
                {
                    case GherkinBlockKeyword.Scenario:
                    case GherkinBlockKeyword.ScenarioOutline:
                    case GherkinBlockKeyword.Background:
                        return _lineBreaksBeforeScenario;

                    case GherkinBlockKeyword.Examples:
                        return _lineBreaksBeforeExamples;

                    case GherkinBlockKeyword.Feature:
                        return _lineBreaksBeforeFeature;
                }
            }

            return 0;
        }


        private GherkinDialect GetDialect(GherkinLanguageService languageService)
        {
            var fileScope = languageService.GetFileScope(waitForResult: false);
            return fileScope != null
                ? fileScope.GherkinDialect
                : languageService.ProjectScope.GherkinDialectServices.GetDefaultDialect();
        }

        private bool IsBlockLine(string line, GherkinDialect dialect)
        {
            var trimmedLine = line.TrimStart();
            return dialect.GetBlockKeywords().Any(keyword => trimmedLine.StartsWith(keyword));
        }

        private GherkinBlockKeyword GetBlockKeyword(string line, GherkinDialect dialect)
        {
            var trimmedLine = line.TrimStart();
            return Enum.GetValues(typeof(GherkinBlockKeyword))
                .Cast<GherkinBlockKeyword>()
                .First(keyword => dialect.GetBlockKeywords(keyword).Any(word => trimmedLine.StartsWith(word)));
        }

        private bool IsStepLine(string line, GherkinDialect dialect)
        {
            var trimmedLine = line.TrimStart();
            return dialect.GetStepKeywords().Any(keyword => trimmedLine.StartsWith(keyword));
        }

        private bool IsTableLine(string line)
        {
            return line.TrimStart().StartsWith(TableSeparator);
        }

        private bool IsCommentLine(string line)
        {
            return line.TrimStart().StartsWith(CommentSymbol);
        }

        private bool IsTagLine(string line)
        {
            return line.TrimStart().StartsWith(TagSymbol);
        }

        private bool IsMultilineDelimeterLine(string line)
        {
            return line.Trim() == MultilineArgumentDelimeter;
        }
    }
}
