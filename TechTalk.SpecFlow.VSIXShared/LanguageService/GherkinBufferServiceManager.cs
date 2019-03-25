using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
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
            return textBuffer.Properties.GetOrCreateSingletonProperty(typeof (TService),
                                                                      () =>
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
            foreach (var subjectBuffer in subjectBuffers)
            {
                List<Type> property;
                if (subjectBuffer.Properties.TryGetProperty(KEY, out property))
                {
                    subjectBuffer.Properties.RemoveProperty(KEY);
                    foreach (var typeKey in property)
                    {
                        IDisposable service;
                        if (subjectBuffer.Properties.TryGetProperty(typeKey, out service))
                        {
                            subjectBuffer.Properties.RemoveProperty(typeKey);
                            service.Dispose();
                        }
                    }
                }
            }
        }
    }
}