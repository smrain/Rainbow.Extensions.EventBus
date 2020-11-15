using Rainbow.Extensions.EventBus.Abstractions.Events;

namespace Rainbow.Extensions.EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>() where TH : IIntegrationEventHandler<T> where T : IntegrationEvent;
    }
}
