using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Differencing;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService;
using TechTalk.SpecFlow.VsIntegration.Implementation.Tracing;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    [Export(typeof(IGherkinBufferServiceManager))]
    [Export(typeof(IWpfTextViewConnectionListener))]
    [ContentType("gherkin")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    class GherkinBufferServiceManager : IGherkinBufferServiceManager, IWpfTextViewConnectionListener
    {
        [Import]
        IVisualStudioTracer Tracer = null;

        private const string KEY = "GherkinBufferServiceManager";

        private const string CONNECTEDVIEWSKEY = "GherkinBufferServiceManager_ConnectedViews";

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
            foreach (var textBuffer in subjectBuffers)
            {
                SubjectBufferConnected(textView, textBuffer);
            }
        }

        public void SubjectBuffersDisconnected(IWpfTextView textView, ConnectionReason reason, Collection<ITextBuffer> subjectBuffers)
        {
            foreach (var textBuffer in subjectBuffers)
            {
                SubjectBufferDisconnected(textView, textBuffer);
            }
        }

        private void SubjectBufferConnected(IWpfTextView textView, ITextBuffer textBuffer)
        {
            Trace(() => $"Connect text buffer {EnsureId(textBuffer)} to view {EnsureId(textView)}");

            var connectedViews = textBuffer.Properties.GetOrCreateSingletonProperty(CONNECTEDVIEWSKEY, () => new HashSet<IWpfTextView>());
            connectedViews.Add(textView);
            Trace(() => $"Text buffer {EnsureId(textBuffer)} is now connected to {connectedViews.Count} views");
        }

        private void SubjectBufferDisconnected(IWpfTextView textView, ITextBuffer textBuffer)
        {
            Trace(() => $"Disconnect text buffer {EnsureId(textBuffer)} from view {EnsureId(textView)}");

            var canDisposeTextBuffer = true;

            if (textBuffer.Properties.TryGetProperty(CONNECTEDVIEWSKEY, out HashSet<IWpfTextView> connectedTextViews))
            {
                connectedTextViews.Remove(textView);
                Trace(() => $"Text buffer {EnsureId(textBuffer)} is now connected to {connectedTextViews.Count} views");
                if (connectedTextViews.Count > 0) canDisposeTextBuffer = false;
            }

            if (canDisposeTextBuffer) DisposeTextBuffer(textBuffer);
        }

        private void DisposeTextBuffer(ITextBuffer textBuffer)
        {
            Trace(() => $"Dispose services of text buffer {EnsureId(textBuffer)}");

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

        private string EnsureId(IPropertyOwner propertyOwner)
        {
            return propertyOwner.GetType().Name + propertyOwner.Properties.GetOrCreateSingletonProperty("ID", () => Guid.NewGuid().ToString());
        }


        private const string Category = "GherkinBufferServiceManager";
        private void Trace(Func<string> message)
        {
            if (Tracer.IsEnabled(Category))
                Tracer.Trace(message(), Category);
        }

    }
}