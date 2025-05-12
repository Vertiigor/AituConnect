using MessageListenerService.Contracts;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;
using MessageListenerService.StepHandlers.Abstractions;

namespace MessageListenerService.StepHandlers.Implementations.ProfileEditing
{
    public class OptionStepHandler : StepHandler
    {
        private readonly InputValidator _inputValidator;
        public OptionStepHandler(IMessageProducer producer, UserSessionService userSessionService, InputValidator inputValidator) : base(producer, userSessionService)
        {
            _inputValidator = inputValidator;
        }

        public override string StepName => "ChoosingOption";

        public override string PipelineName => "EditingProfile";

        public override async Task HandleAsync(UserSession session, string userInput, string messageId)
        {
            if (_inputValidator.TryValidate(userInput, "Option", out string option))
            {
                Console.WriteLine($"Handling university step for user {session.ChatId} with input: {userInput}");

                var payload = new RegistrationContract
                {
                    ChatId = session.ChatId,
                    UserName = session.Username,
                    University = string.Empty,
                    Major = string.Empty,
                    MessageId = messageId,
                };

                // Send the message to the producer
                await _producer.PublishMessageAsync(
                    eventType: $"Editing{option}",
                    payload: payload,
                    exchange: "aituBot.exchange",
                    routingKey: "user.editProfile"
                );

                Console.WriteLine($"User {session.ChatId} selected university: {userInput}");

                // Update the session in Redis
                session.CurrentStep = $"Editing{option}"; // Update to the next step
                await _userSessionService.SetSessionAsync(session);
            }
        }
    }
}
