using AituConnectAPI.Bot;
using AituConnectAPI.Models.Abstractions;

namespace AituConnectAPI.Pipelines.Abstractions
{
    public abstract class PipelineStep
    {
        protected readonly BotClient _botClient;

        protected PipelineStep(BotClient botClient)
        {
            _botClient = botClient;
        }

        public abstract Task ExecuteAsync(PipelineContext context);

        public abstract bool IsApplicable(PipelineContext context);
    }
}
