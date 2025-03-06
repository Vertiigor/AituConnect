using AituConnectAPI.Bot;
using AituConnectAPI.Models;

namespace AituConnectAPI.Pipelines.Abstractions
{
    public abstract class PipelineStep
    {
        protected readonly BotMessageSender _messageSender;

        protected PipelineStep(BotMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public abstract Task ExecuteAsync(PipelineContext context);

        public abstract bool IsApplicable(PipelineContext context);
    }
}
