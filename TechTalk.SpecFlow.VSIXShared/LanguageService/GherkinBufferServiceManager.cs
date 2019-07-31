using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Differencing;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    [Export(typeof(IGherkinBufferServiceManager))]
    [Export(typeof(IWpfTextViewConnectionListener))]
    [ContentType("gherkin")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    class GherkinBufferServiceManager : IGherkinBufferServiceManager, IWpfTextViewConnectionListener
    {
        private const string KEY = "GherkinBufferServiceManager";

        public TService GetOrCreate<TService>(ITextBuffer textBuffer, Func<TService> creator) where TService : class, IDisposable
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(typeof(TService), () =>
                    {
                        textBuffer.Properties.GetOrCreateSingletonProperty(KEY, () => new List<Type>()).Add(typeof(TService));
                        return creator();
                    });
        }

        public void SubjectBuffersConnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            //nop;
        }

        public void SubjectBuffersDisconnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            var textBuffers = GetTextBuffersToDispose(textView, subjectBuffers);

            foreach (var textBuffer in textBuffers)
            {
                if (textBuffer.Properties.TryGetProperty(KEY, out List<Type> property))
                {
                    textBuffer.Properties.RemoveProperty(KEY);
                    foreach (var typeKey in property)
                    {
                        if (textBuffer.Properties.TryGetProperty(typeKey, out IDisposable service))
                        {
                            textBuffer.Properties.RemoveProperty(typeKey);
                            service.Dispose();
                        }
                    }
                }
            }
        }

        private static IReadOnlyCollection<ITextBuffer> GetTextBuffersToDispose(IWpfTextView textView, Collection<ITextBuffer> subjectBuffers)
        {
            var textBuffers = new List<ITextBuffer>();
            if (textView.Roles.Contains(DifferenceViewerRoles.RightViewTextViewRole))
            {
                // The right view in VS's difference viewer is the edited file, don't dispose it
                return textBuffers;
            }

            var isInlineDiffView = textView.Roles.Contains(DifferenceViewerRoles.InlineViewTextViewRole);
            if (!isInlineDiffView)
            {
                // We're not in a difference viewer, so just return the original subjectbuffers
                return subjectBuffers;
            }

            foreach (ITextBuffer subjectBuffer in subjectBuffers)
            {
                if (isInlineDiffView && subjectBuffer != textView.TextDataModel.DocumentBuffer)
                {
                    // If the subjectBuffer does not equal the text data model's document buffer, we can dispose it
                    textBuffers.Add(subjectBuffer);
                }
            }

            return textBuffers;
        }
    }
}