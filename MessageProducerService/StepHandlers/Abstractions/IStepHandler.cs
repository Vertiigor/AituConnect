using MessageProducerService.Contracts;

namespace MessageProducerService.StepHandlers.Abstractions
{
    public interface IStepHandler
    {
        Task HandleAsync(MessageEnvelope envelope);
    }
}
