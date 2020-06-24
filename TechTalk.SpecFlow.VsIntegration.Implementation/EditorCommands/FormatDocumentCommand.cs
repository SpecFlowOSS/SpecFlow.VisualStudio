using System;
using System.Collections.Generic;
using System.Linq;
using Gherkin;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.EditorCommands
{
    public class FormatDocumentCommand
    {
        private string _featureIndent;
        private string _scenarioIndent;
        private string _stepIndent;
        private string _tableIndent;
        private string _multilineIndent;
        private string _exampleIndent;

        private const string CommentSymbol = "#";
        private const string TableSeparator = "|";
        private const string TagSymbol = "@";
        private const string MultilineArgumentDelimeter = "\"\"\"";

        private bool _normalizeLineBreaks;
        private const int _lineBreaksBeforeStep = 0;
        private int _lineBreaksBeforeScenario;
        private int _lineBreaksBeforeExamples;
        private const int _lineBreaksBeforeFeature = 0;


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
            _lineBreaksBeforeScenario = options.LineBreaksBeforeScenario;
            _lineBreaksBeforeExamples = options.LineBreaksBeforeExamples;

            var indentType = options.UseTabsForIndent ? '\t' : ' ';
            _featureIndent = new string(indentType, options.FeatureIndent);
            _scenarioIndent = new string(indentType, options.ScenarioIndent);
            _stepIndent = new string(indentType, options.StepIndent);
            _tableIndent = new string(indentType, options.TableIndent);
            _multilineIndent = new string(indentType, options.MultilineIndent);
            _exampleIndent = new string(indentType, options.ExampleIndent);
        }

        private void ReplaceText(ITextSnapshot textSnapshot, List<string> newTextLines)
        {
            //Replacing text line-by-line preserves scroll and caret position
            using (var edit = textSnapshot.TextBuffer.CreateEdit())
            {
                var currentLines = textSnapshot.Lines.ToList();

                int currentLineIndex, newLineIndex;

                //Replace line-by-line to preserve scroll and cursor position
                for (currentLineIndex = 0, newLineIndex = 0;
                     currentLineIndex < currentLines.Count
                     && newLineIndex < newTextLines.Count;
                     currentLineIndex++, newLineIndex++)
                {
                    var currentLine = currentLines[currentLineIndex];
                    var currentLineText = currentLine.GetText();
                    if (currentLine.GetText() != newTextLines[newLineIndex])
                    {
                        // if existing text has excessive (or missing) lines - remove (or add them),
                        // and adjust index.
                        // Needed to avoid "jumping" cursor position
                        if (string.IsNullOrWhiteSpace(currentLineText)
                            && !string.IsNullOrWhiteSpace(newTextLines[newLineIndex]))
                        {
                            newLineIndex--;
                            var span = new SnapshotSpan(currentLine.Start, currentLine.EndIncludingLineBreak);
                            edit.Delete(span);
                        }
                        else if (!string.IsNullOrWhiteSpace(currentLineText)
                                 && string.IsNullOrWhiteSpace(newTextLines[newLineIndex]))
                        {
                            currentLineIndex--;
                            edit.Insert(currentLine.Start, newTextLines[newLineIndex] + Environment.NewLine);
                        }
                        else
                        {
                            var span = new SnapshotSpan(currentLine.Start, currentLine.End);
                            edit.Replace(span, newTextLines[newLineIndex]);
                        }
                    }
                }

                //Replace anything left
                var lastLine = currentLines[currentLineIndex - 1];
                var endSpan = new SnapshotSpan(lastLine.End, currentLines.Last().EndIncludingLineBreak);
                string remainingText = newTextLines.Count > newLineIndex
                    ? Environment.NewLine + string.Join(Environment.NewLine, newTextLines.Skip(newLineIndex))
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
                    // Comment or tag lines should have same indent as following line,
                    // that's why we put them to temporary collection
                    stringsToInsertBefore.Add(trimmedLine);
                }
                else if (IsTableLine(trimmedLine))
                {
                    formattedTextLines.AddRange(stringsToInsertBefore.Select(str => _tableIndent + str));
                    stringsToInsertBefore.Clear();

                    //Find whole table, and format it using FormatTableCommand.FormatTableString
                    var tableLines = new List<string>();
                    tableLines.Add(_tableIndent + trimmedLine);
                    while (i + 1 < textLines.Count && IsTableLine(textLines[i + 1]))
                    {
                        i++;
                        tableLines.Add(textLines[i]);
                    }

                    var formattedTable =
                        FormatTableCommand.FormatTableString(string.Join(Environment.NewLine, tableLines));

                    if (formattedTable != null)
                    {
                        formattedTextLines.AddRange(
                            formattedTable.Split(
                                new[] { Environment.NewLine },
                                StringSplitOptions.None));
                    }
                    else
                    {
                        //if table fails to format - leave it as it is
                        formattedTextLines.AddRange(tableLines);
                    }
                }
                else if (IsMultilineDelimeterLine(trimmedLine))
                {
                    formattedTextLines.AddRange(stringsToInsertBefore.Select(str => _multilineIndent + str));
                    stringsToInsertBefore.Clear();

                    formattedTextLines.Add(_multilineIndent + trimmedLine);

                    //Find original indent of multi-line argument
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
                            formattedLine = _multilineIndent + textLines[i].Substring(originalIndent.Length);
                        }
                        else
                        {
                            //invalid case - leave it as it is
                            formattedLine = textLines[i];
                        }

                        formattedTextLines.Add(formattedLine);
                    }

                    formattedTextLines.Add(_multilineIndent + textLines[i].Trim());
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
                var trimmedLine = line.TrimStart();

                if (dialect.ExamplesKeywords.Any(ek => trimmedLine.StartsWith(ek)))
                    return _exampleIndent;
                if (dialect.FeatureKeywords.Any(fk => trimmedLine.StartsWith(fk)))
                    return _featureIndent;

                if (dialect.ScenarioKeywords.Any(sk => trimmedLine.StartsWith(sk)) ||
                    dialect.ScenarioOutlineKeywords.Any(sok => trimmedLine.StartsWith(sok)) ||
                    dialect.BackgroundKeywords.Any(bk => trimmedLine.StartsWith(bk)))
                {
                    return _scenarioIndent;
                }
            }
            else if (IsStepLine(line, dialect))
            {
                return GetConfiguredStepLineBreaksAndIndent();
            }
            else if (IsTableLine(line))
            {
                return _tableIndent;
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

            return str + _stepIndent;
        }

        private int GetPreceedingLineBreaks(string line, GherkinDialect dialect)
        {
            if (IsBlockLine(line, dialect))
            {
                var trimmedLine = line.TrimStart();

                if (dialect.ExamplesKeywords.Any(ek => trimmedLine.StartsWith(ek)))
                    return _lineBreaksBeforeExamples;
                if (dialect.FeatureKeywords.Any(fk => trimmedLine.StartsWith(fk)))
                    return _lineBreaksBeforeFeature;

                if (dialect.ScenarioKeywords.Any(sk => trimmedLine.StartsWith(sk)) ||
                    dialect.ScenarioOutlineKeywords.Any(sok => trimmedLine.StartsWith(sok)) ||
                    dialect.BackgroundKeywords.Any(bk => trimmedLine.StartsWith(bk)))
                {
                    return _lineBreaksBeforeScenario;
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
            return dialect.FeatureKeywords.Any(keyword => trimmedLine.StartsWith(keyword)) ||
                   dialect.ScenarioKeywords.Any(keyword => trimmedLine.StartsWith(keyword)) ||
                   dialect.ScenarioOutlineKeywords.Any(keyword => trimmedLine.StartsWith(keyword)) ||
                   dialect.ExamplesKeywords.Any(keyword => trimmedLine.StartsWith(keyword)) ||
                   dialect.BackgroundKeywords.Any(keyword => trimmedLine.StartsWith(keyword));
        }

        private bool IsStepLine(string line, GherkinDialect dialect)
        {
            var trimmedLine = line.TrimStart();
            return dialect.StepKeywords.Any(keyword => trimmedLine.StartsWith(keyword));
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