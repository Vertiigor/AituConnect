using MessageListenerService.Data.Connections.Redis;
using MessageListenerService.Models;
using MessageListenerService.Producers.Abstractions;
using MessageListenerService.Services;

namespace MessageListenerService.StepHandlers.Abstractions
{
    public abstract class StepHandler
    {
        protected readonly IMessageProducer _producer;
        protected readonly UserSessionService _userSessionService;
        public abstract string StepName { get; }
        public abstract string PipelineName { get; }

        protected StepHandler(IMessageProducer producer, UserSessionService userSessionService)
        {
            _producer = producer;
            _userSessionService = userSessionService;
        }

        public abstract Task HandleAsync(UserSession session, string userInput, string messageId);
    }
}
