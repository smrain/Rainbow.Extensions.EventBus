using System;

namespace Rainbow.Extensions.EventBus.Abstractions.Events
{
    public class SubscriptionInfo
    {
        public Type HandlerType { get; }

        public SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType;
        }
        public static SubscriptionInfo Initial(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }
    }
}
