using MassTransit;
using TechMastery.UsermanagementService.Messages;

public class NewUserRegistrationConsumer : IConsumer<NewUserRegistration>
{
    public async Task Consume(ConsumeContext<NewUserRegistration> context)
    {
        var newUserMessage = context.Message;
        Console.WriteLine($"newUserMessageAdded - Email: {newUserMessage.Email}, UserId: {newUserMessage.UserId}");
    }
}


