using AituConnectAPI.Bot;
using AituConnectAPI.Models;

namespace AituConnectAPI.Pipelines.Abstractions
{
    public abstract class Pipeline
    {
        protected readonly List<PipelineStep> _steps;
        protected readonly BotMessageSender _messageSender;

        public Pipeline(BotMessageSender messageSender)
        {
            _messageSender = messageSender;
            _steps = new List<PipelineStep>();
        }

        public async Task ExecuteAsync(PipelineContext context)
        {
            if (!context.IsCompleted)
            {
                foreach (var step in _steps)
                {
                    if (step.IsApplicable(context))
                    {
                        await step.ExecuteAsync(context);
                        break; // Execute only the current step
                    }
                }
            }
        }
    }
}
