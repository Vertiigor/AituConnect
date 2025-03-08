using AituConnectAPI.Bot;
using AituConnectAPI.Models;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Pipelines.Abstractions
{
    public abstract class PipelineStep
    {
        protected readonly BotMessageSender _messageSender;
        protected readonly IPipelineContextService _pipelineContextService;
        protected readonly IUserService _userService;

        protected PipelineStep(BotMessageSender messageSender, IPipelineContextService pipelineContextService, IUserService userService)
        {
            _messageSender = messageSender;
            _pipelineContextService = pipelineContextService;
            _userService = userService;
        }

        public abstract Task ExecuteAsync(PipelineContext context);

        public abstract bool IsApplicable(PipelineContext context);
    }
}
