using MessageProducerService.Contracts;

namespace MessageProducerService.StepHandlers.Abstractions
{
    public abstract class StepHandler
    {
        public abstract string StepName { get; }

        public abstract Task HandleAsync(MessageEnvelope envelope);
    }
}
