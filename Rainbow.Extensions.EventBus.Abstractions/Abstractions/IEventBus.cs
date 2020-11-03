using Rainbow.Extensions.EventBus.Abstractions.Events;

namespace Rainbow.Extensions.EventBus.Abstractions
{
    public interface IEventBus
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="event">集成事件</param>
        void Publish(IntegrationEvent @event);

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T">集成事件</typeparam>
        /// <typeparam name="TH">集成事件处理</typeparam>
        void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <typeparam name="T">集成事件</typeparam>
        /// <typeparam name="TH">集成事件处理</typeparam>
        void Unsubscribe<T, TH>() where TH : IIntegrationEventHandler<T> where T : IntegrationEvent;
    }
}
