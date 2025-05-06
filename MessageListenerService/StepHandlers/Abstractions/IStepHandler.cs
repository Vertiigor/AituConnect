using MessageListenerService.Models;

namespace MessageListenerService.StepHandlers.Abstractions
{
    public interface IStepHandler
    {
        Task HandleAsync(UserSession session, string userInput, string messageId);
    }
}
