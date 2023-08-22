using MassTransit;
using TechMastery.MarketPlace.Messaging.Consumers;
using TechMastery.Messaging.Consumers.Consumers;

namespace TechMastery.Messaging.Consumers
{
    internal static class ConsumerConfigurator
    {
        public static void Configure(IBusFactoryConfigurator cfg, string queueName)
        {
            cfg.ReceiveEndpoint(queueName, e =>
            {
                e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                e.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5)));
                e.Consumer<PaymentInitiatedConsumer>();
                e.Consumer<PaymentCompletedConsumer>();
                e.Consumer<ProductAddedConsumer>();
                e.Consumer<OrderPlacedConsumer>();
            });
        }
    }
}
