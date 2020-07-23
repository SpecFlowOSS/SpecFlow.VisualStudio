using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using gherkin;
using Gherkin;
using Gherkin.Ast;
using gherkin.lexer;

namespace TechTalk.SpecFlow.Parser.Gherkin
{
    public class GherkinScannerAdapter
    {
        private readonly GherkinDialectAdapter _gherkinDialectAdapter;
        private readonly GherkinBuffer buffer;

        public GherkinScannerAdapter(GherkinDialectAdapter gherkinDialectAdapter, string gherkinText)
            : this(gherkinDialectAdapter, gherkinText, 0)
        {
        }

        public GherkinScannerAdapter(GherkinDialectAdapter gherkinDialectAdapter, string gherkinText, int lineOffset)
        {
            this._gherkinDialectAdapter = gherkinDialectAdapter;
            this.buffer = new GherkinBuffer(gherkinText, lineOffset);
        }

        public void Scan(IGherkinListener listener)
        {
            ListenerExtender listenerExtender = new ListenerExtender(_gherkinDialectAdapter, listener, buffer);
            DoScan(listenerExtender, buffer.LineOffset, 0);
        }

        const int MAX_ERROR_RETRY = 5;
        const int SKIP_LINES_BEFORE_RETRY = 1;

        private void DoScan(ListenerExtender listenerExtender, int startLine, int errorRertyCount)
        {
            //NOTE: the new parser can only do a full parse
            //listenerExtender.LineOffset = startLine;
            //var contentToScan = buffer.GetContentFrom(startLine);
            var contentToScan = buffer.GetContent();

            var parser = new global::Gherkin.Parser();
            GherkinDocument document = null;
            try
            {
                using (var reader = new StringReader(contentToScan))
                {
                    document = parser.Parse(reader);
                    var notifier = new ListenerExtenderNotifier(listenerExtender);
                    notifier.NotifyDocument(document);
                }
            }
            catch (AstBuilderException astBuilderException)
            {
                listenerExtender.GherkinListener.Error(
                    astBuilderException.Message,
                    buffer.GetLineStartPosition(astBuilderException.Location.Line),
                    astBuilderException);
            }

            //try
            //{
            //    //Lexer lexer = _gherkinDialectAdapter.NativeLanguageService.lexer(listenerExtender);
            //    //lexer.scan(contentToScan);
            //}
            //catch (ScanningCancelledException)
            //{
            //    throw;
            //}
            //catch (LexingError lexingError)
            //{
            //    HandleError(GetLexingError(lexingError, listenerExtender.LineOffset), lexingError, listenerExtender, errorRertyCount);
            //}
            //catch (ScanningErrorException scanningErrorException)
            //{
            //    HandleError(scanningErrorException, scanningErrorException, listenerExtender, errorRertyCount);
            //}
            //catch (Exception ex)
            //{
            //    HandleError(GetUnknownError(ex), ex, listenerExtender, errorRertyCount);
            //}
        }

        private class ListenerExtenderNotifier
        {
            private readonly ListenerExtender _listenerExtender;

            public ListenerExtenderNotifier(ListenerExtender listenerExtender)
            {
                _listenerExtender = listenerExtender;
            }

            public void NotifyDocument(GherkinDocument document)
            {
                NotifyNode(document.Feature);

                NotifyComments(document.Comments);

                _listenerExtender.eof();
            }

            private void NotifyComments(IEnumerable<Comment> comments)
            {
                foreach (var comment in comments)
                {
                    _listenerExtender.comment(comment.Text, comment.Location.Line);
                }
            }

            public void NotifyNode(IHasLocation node)
            {
                if (node is IHasTags hasTags) NotifyTags(hasTags);

                if (node is IHasDescription hasDescription) NotifyDescription(hasDescription, node.Location);

                if (node is IHasChildren hasChildren) NotifyChildren(hasChildren);

                if (node is IHasSteps hasSteps) NotifySteps(hasSteps);

                if (node is Scenario hasExamplesList) NotifyExamplesList(hasExamplesList);
            }

            private void NotifyExamplesList(Scenario hasExamplesList)
            {
                foreach (var examples in hasExamplesList.Examples)
                {
                    NotifyExamples(examples);
                }
            }

            private void NotifyExamples(Examples examples)
            {
                NotifyNode(examples);

                NotifyTableRow(examples.TableHeader);
                NotifyTableRows(examples.TableBody);
            }

            private void NotifySteps(IHasSteps hasSteps)
            {
                foreach (var step in hasSteps.Steps)
                {
                    NotifyStep(step);
                }
            }

            private void NotifyStep(Step step)
            {
                _listenerExtender.step(step.Keyword, step.Text, step.Location.Line);

                NotifyStepArgument(step.Argument);
            }

            private void NotifyStepArgument(StepArgument stepArgument)
            {
                if (stepArgument is DataTable dataTable) NotifyDataTable(dataTable);

                if (stepArgument is DocString docString) NotifyDocString(docString);
            }

            private void NotifyDocString(DocString docString)
            {
                _listenerExtender.docString(docString.ContentType, docString.Content, docString.Location.Line);
            }

            private void NotifyDataTable(DataTable dataTable)
            {
                NotifyTableRows(dataTable.Rows);
            }

            private void NotifyTableRows(IEnumerable<global::Gherkin.Ast.TableRow> rows)
            {
                foreach (var tableRow in rows)
                {
                    NotifyTableRow(tableRow);
                }
            }

            private void NotifyTableRow(global::Gherkin.Ast.TableRow tableRow)
            {
                _listenerExtender.row(tableRow.Cells.Select(c => c.Value).ToList(), tableRow.Location.Line);
            }

            private void NotifyChildren(IHasChildren hasChildren)
            {
                foreach (var child in hasChildren.Children)
                {
                    NotifyNode(child);
                }
            }

            private void NotifyDescription(IHasDescription hasDescription, Location location)
            {
                switch (hasDescription)
                {
                    case Feature feature: _listenerExtender.feature(hasDescription.Keyword, hasDescription.Name, hasDescription.Description, location.Line);
                        break;
                    case Scenario scenario: _listenerExtender.scenario(hasDescription.Keyword, hasDescription.Name, hasDescription.Description, location.Line);
                        break;
                    case Examples examples: _listenerExtender.examples(hasDescription.Keyword, hasDescription.Name, hasDescription.Description, location.Line);
                        break;
                }
            }

            public void NotifyTags(IHasTags node)
            {
                foreach (var tag in node.Tags)
                {
                    _listenerExtender.tag(tag.Name, tag.Location.Line);
                }
            }
        }

        //        static private readonly Regex lexingErrorRe = new Regex(@"^Lexing error on line (?<lineno>\d+):\s*'?(?<nearTo>[^\r\n']*)");
        //        private ScanningErrorException GetLexingError(LexingError lexingError, int lineOffset)
        //        {
        //            var match = lexingErrorRe.Match(lexingError.Message);
        //            if (!match.Success)
        //                return GetUnknownError(lexingError);

        //            int parserdLine = Int32.Parse(match.Groups["lineno"].Value);
        //            int errorLine = parserdLine - 1 + lineOffset;
        //            string nearTo = match.Groups["nearTo"].Value;

        //            string message = string.Format("Parsing error near '{0}'", nearTo);
        //            if (nearTo.Equals("%_FEATURE_END_%", StringComparison.CurrentCultureIgnoreCase))
        //                message = "Parsing error near the end of the file. Check whether the last statement is closed properly.";

        //            return new ScanningErrorException(message, buffer.GetLineStartPosition(errorLine));
        //        }

        //        private ScanningErrorException GetUnknownError(Exception exception)
        //        {
        //            string message = string.Format("Parsing error: {0}", exception.Message);
        //            return new ScanningErrorException(message);
        //        }

        //        private void HandleError(ScanningErrorException scanningErrorException, Exception originalException, ListenerExtender listenerExtender, int errorRertyCount)
        //        {
        //            RegisterError(listenerExtender.GherkinListener, scanningErrorException, originalException);

        //            var position = scanningErrorException.GetPosition(buffer);

        ////            if (position != null &&
        ////                position.Line + SKIP_LINES_BEFORE_RETRY <= buffer.LineCount - 1 &&
        ////                errorRertyCount < MAX_ERROR_RETRY)
        //            var lastProcessedEditorLine = listenerExtender.LastProcessedEditorLine;
        //            if (position != null)
        //                lastProcessedEditorLine = Math.Max(position.Line, lastProcessedEditorLine);

        //            if (lastProcessedEditorLine + SKIP_LINES_BEFORE_RETRY <= buffer.LineCount - 1 &&
        //                errorRertyCount < MAX_ERROR_RETRY)
        //            {
        //                var restartLineNumber = lastProcessedEditorLine + SKIP_LINES_BEFORE_RETRY;

        //                DoScan(
        //                    listenerExtender,
        //                    restartLineNumber,
        //                    errorRertyCount + 1);
        //            }
        //        }

        //        private void RegisterError(IGherkinListener gherkinListener, ScanningErrorException scanningErrorException, Exception originalException)
        //        {
        //            var position = scanningErrorException.GetPosition(buffer);

        //            gherkinListener.Error(
        //                scanningErrorException.Message, 
        //                position ?? buffer.EndPosition, 
        //                originalException);
        //        }
    }
}