using RabbitMQ.Client;
using System;

namespace Rainbow.Extensions.EventBus.RabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
