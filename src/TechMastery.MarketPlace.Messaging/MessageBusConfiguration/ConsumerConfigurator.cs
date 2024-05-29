using MassTransit;

public static class ConsumerConfigurator
{
    public static void Configure(IBusFactoryConfigurator cfg, string queueName, params Type[] consumerTypes)
    {
        cfg.ReceiveEndpoint(queueName, e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5)));

            // Dynamically add consumers based on the passed-in types
            foreach (var consumerType in consumerTypes)
            {
                if (typeof(IConsumer).IsAssignableFrom(consumerType))
                {
                    e.Consumer(consumerType, type => Activator.CreateInstance(type));
                }
                else
                {
                    throw new ArgumentException($"{consumerType.Name} does not implement IConsumer interface.");
                }
            }
        });
    }
}
