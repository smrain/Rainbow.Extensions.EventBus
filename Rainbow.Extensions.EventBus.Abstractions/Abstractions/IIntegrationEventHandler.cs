using Rainbow.Extensions.EventBus.Abstractions.Events;
using System.Threading.Tasks;

namespace Rainbow.Extensions.EventBus.Abstractions
{
    public interface IIntegrationEventHandler
    {
    }

    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
        where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }


}
