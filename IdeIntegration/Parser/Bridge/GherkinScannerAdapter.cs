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
        private readonly GherkinBuffer _buffer;

        public GherkinScannerAdapter(GherkinDialectAdapter gherkinDialectAdapter, string gherkinText)
            : this(gherkinDialectAdapter, gherkinText, 0)
        {
        }

        public GherkinScannerAdapter(GherkinDialectAdapter gherkinDialectAdapter, string gherkinText, int lineOffset)
        {
            this._gherkinDialectAdapter = gherkinDialectAdapter;
            this._buffer = new GherkinBuffer(gherkinText, lineOffset);
        }

        public void Scan(IGherkinListener listener)
        {
            ListenerExtender listenerExtender = new ListenerExtender(_gherkinDialectAdapter, listener, _buffer);

            //NOTE: the new parser can only do a full parse
            //listenerExtender.LineOffset = startLine;
            //var contentToScan = buffer.GetContentFrom(startLine);
            var contentToScan = _buffer.GetContent();
            var parser = new global::Gherkin.Parser();
            var notifier = new ListenerExtenderNotifier(listenerExtender, _buffer);

            GherkinDocument document = null;
            ParserException firstParserException = null;

            try
            {
                document = Parse(contentToScan, parser);
            }
            catch (ParserException parserException)
            {
                firstParserException = parserException;

                var firstErrorLine = parserException.Expand()
                               .Where(e => e.Location != null)
                               .Min(e => e.Location.Line);


                //TODO: review line number in AST vs. GherkinBuffer
                contentToScan =_buffer.GetContentBefore(firstErrorLine - 1);

                //TODO: what if the second parse also fails?
                document = Parse(contentToScan, parser);
            }

            notifier.NotifyDocument(document, firstParserException);

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

        private static GherkinDocument Parse(string contentToScan, global::Gherkin.Parser parser)
        {
            GherkinDocument document;
            using (var reader = new StringReader(contentToScan))
            {
                document = parser.Parse(reader);
            }

            return document;
        }

        private class ListenerExtenderNotifier
        {
            private readonly ListenerExtender _listenerExtender;
            private readonly GherkinBuffer _gherkinBuffer;

            public ListenerExtenderNotifier(ListenerExtender listenerExtender, GherkinBuffer gherkinBuffer)
            {
                _listenerExtender = listenerExtender;
                _gherkinBuffer = gherkinBuffer;
            }

            public void NotifyDocument(GherkinDocument document, ParserException error)
            {
                if (document == null) return;

                NotifyNode(document.Feature);

                NotifyComments(document.Comments);

                NotifyError(error);

                _listenerExtender.eof();
            }

            private void NotifyComments(IEnumerable<Comment> comments)
            {
                if (comments == null) return;

                foreach (var comment in comments)
                {
                    _listenerExtender.comment(comment.Text, comment.Location.Line);
                }
            }

            private void NotifyNode(IHasLocation node)
            {
                if (node == null) return;

                if (node is IHasTags hasTags) NotifyTags(hasTags);

                if (node is IHasDescription hasDescription) NotifyDescription(hasDescription, node.Location);

                if (node is IHasChildren hasChildren) NotifyChildren(hasChildren);

                if (node is IHasSteps hasSteps) NotifySteps(hasSteps);

                if (node is Scenario hasExamplesList) NotifyExamplesList(hasExamplesList);
            }

            private void NotifyExamplesList(Scenario hasExamplesList)
            {
                if (hasExamplesList?.Examples == null) return;

                foreach (var examples in hasExamplesList.Examples)
                {
                    NotifyExamples(examples);
                }
            }

            private void NotifyExamples(Examples examples)
            {
                if (examples == null) return;

                NotifyNode(examples);

                NotifyTableRow(examples.TableHeader);
                NotifyTableRows(examples.TableBody);
            }

            private void NotifySteps(IHasSteps hasSteps)
            {
                if (hasSteps?.Steps == null) return;

                foreach (var step in hasSteps.Steps)
                {
                    NotifyStep(step);
                }
            }

            private void NotifyStep(Step step)
            {
                if (step == null) return;
                
                _listenerExtender.step(step.Keyword, step.Text, step.Location.Line);

                NotifyStepArgument(step.Argument);
            }

            private void NotifyStepArgument(StepArgument stepArgument)
            {
                if (stepArgument == null) return;

                if (stepArgument is DataTable dataTable) NotifyDataTable(dataTable);

                if (stepArgument is DocString docString) NotifyDocString(docString);
            }

            private void NotifyDocString(DocString docString)
            {
                if (docString == null) return;

                _listenerExtender.docString(docString.ContentType, docString.Content, docString.Location.Line);
            }

            private void NotifyDataTable(DataTable dataTable)
            {
                if (dataTable == null) return;

                NotifyTableRows(dataTable.Rows);
            }

            private void NotifyTableRows(IEnumerable<global::Gherkin.Ast.TableRow> rows)
            {
                if (rows == null) return;

                foreach (var tableRow in rows)
                {
                    NotifyTableRow(tableRow);
                }
            }

            private void NotifyTableRow(global::Gherkin.Ast.TableRow tableRow)
            {
                if (tableRow == null) return;

                _listenerExtender.row(tableRow.Cells.Select(c => c.Value).ToList(), tableRow.Location.Line);
            }

            private void NotifyChildren(IHasChildren hasChildren)
            {
                if (hasChildren?.Children == null) return;

                foreach (var child in hasChildren.Children)
                {
                    NotifyNode(child);
                }
            }

            private void NotifyDescription(IHasDescription hasDescription, Location location)
            {
                if (hasDescription == null || location == null) return;

                switch (hasDescription)
                {
                    case Feature _: _listenerExtender.feature(hasDescription.Keyword, hasDescription.Name, hasDescription.Description, location.Line);
                        break;
                    case Scenario _: _listenerExtender.scenario(hasDescription.Keyword, hasDescription.Name, hasDescription.Description, location.Line);
                        break;
                    case Examples _: _listenerExtender.examples(hasDescription.Keyword, hasDescription.Name, hasDescription.Description, location.Line);
                        break;
                    case Background _: _listenerExtender.background(hasDescription.Keyword, hasDescription.Name, hasDescription.Description, location.Line);
                        break;
                }
            }

            public void NotifyTags(IHasTags hasTags)
            {
                if (hasTags?.Tags == null) return;

                foreach (var tag in hasTags.Tags)
                {
                    _listenerExtender.tag(tag.Name, tag.Location.Line);
                }
            }

            public void NotifyError(ParserException parserException)
            {
                if (parserException == null) return;

                foreach (var exception in parserException.Expand())
                {
                    NotifySingleError(exception);
                }
            }

            private void NotifySingleError(ParserException parserException)
            {
                _listenerExtender.GherkinListener.Error(
                    parserException.Message,
                    GetErrorPosition(parserException),
                    parserException);
            }

            private GherkinBufferPosition GetErrorPosition(ParserException parserException) => 
                parserException.Location == null ? 
                    _gherkinBuffer.StartPosition :
                    _gherkinBuffer.GetLineStartPosition(parserException.Location.Line).ShiftByCharacters(parserException.Location.Column);
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

    public static class ParserExceptionExtensions
    {
        public static IEnumerable<ParserException> Expand(this ParserException parserException)
        {
            switch (parserException)
            {
                case CompositeParserException composite when composite.Errors != null: return composite.Errors;
                default: return new[]{parserException};
            }
        }
    }
}