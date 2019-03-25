using System;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Utils
{
    public class SynchInitializedInstance<T> where T : class
    {
        private T instance = null;

        private readonly Func<T> initializer;

        public SynchInitializedInstance(Func<T> initializer)
        {
            this.initializer = initializer;
        }

        public bool IsInitialized
        {
            get { return instance != null; }
        }

        public void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                lock (this)
                {
                    if (!IsInitialized)
                    {
                        var newInstance = initializer();
                        System.Threading.Thread.MemoryBarrier();
                        instance = newInstance;
                    }
                }
            }
        }

        public T Value
        {
            get
            {
                EnsureInitialized();
                return instance;
            }
            set
            {
                lock (this)
                {
                    instance = value;
                }
            }
        }
    }
}