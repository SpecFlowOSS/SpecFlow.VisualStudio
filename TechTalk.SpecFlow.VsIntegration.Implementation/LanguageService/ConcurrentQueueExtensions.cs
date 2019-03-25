using System.Collections.Concurrent;

namespace TechTalk.SpecFlow.VsIntegration.LanguageService
{
    internal static class ConcurrentQueueExtensions
    {
        public static bool Dequeue<T>(this ConcurrentQueue<T> queue)
        {
            T dummy;
            return queue.TryDequeue(out dummy);
        }
    }
}