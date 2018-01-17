namespace Remote.Infrastructure.PublicEvents
{
    /// <summary>
    /// 当WCF服务创建时，获取服务对象，仅用于PubSubEvent
    /// </summary>
    /// <typeparam name="T">WCF服务</typeparam>
    public class ServiceCreatedEvent<T>
    {
        public ServiceCreatedEvent(T t)
        {
            this.Service = t;
        }

        public T Service { get; }
    }
}
