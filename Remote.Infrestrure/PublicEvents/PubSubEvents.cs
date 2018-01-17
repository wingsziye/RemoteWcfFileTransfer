using System;
using System.Threading;
using Reactive.EventAggregator;

namespace Remote.Infrastructure.PublicEvents
{
    public class PubSubEvents : IDisposable
    {
        private PubSubEvents()
        {
            Console.WriteLine($"{DateTime.Now}PubsubEvents Singleton Create");
            Console.WriteLine($"Thread id = {Thread.CurrentThread.ManagedThreadId}");
            eventPublisher = new EventAggregator();
        }

        private EventAggregator eventPublisher;

        public static readonly PubSubEvents Singleton = new PubSubEvents();
        
        public IObservable<T> GetEvent<T>()
        {
            return eventPublisher.GetEvent<T>();
        }

        public void Publish<T>(T tEvent)
        {
            eventPublisher.Publish<T>(tEvent);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    eventPublisher.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~PubSubEvents() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
