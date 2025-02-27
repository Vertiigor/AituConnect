using AituConnectAPI.Bot;
using AituConnectAPI.Models.Abstractions;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Pipelines.Registration
{
    public class UniversityStep : PipelineStep
    {
        private readonly IPipelineContextService _pipelineContextService;
        private readonly IUserService _userService;

        public UniversityStep(BotClient botClient, IPipelineContextService pipelineContextService, IUserService userService) : base(botClient)
        {
            _pipelineContextService = pipelineContextService;
            _userService = userService;
        }

        public override async Task ExecuteAsync(PipelineContext context)
        {
            if (string.IsNullOrEmpty(context.Content))
            {
                // Ask user for the title
                await _botClient.SendTextMessageAsync(context.ChatId, "Enter the name of university you're styding in: ");
            }
            else
            {
                var user = await _userService.GetByChatIdAsync(context.ChatId);
                user.University = context.Content;
                await _userService.UpdateAsync(user);

                //context.IsCompleted = true; // Move to the next step
                context.CurrentStep = "FACULTY";
                context.Content = "";
                await _pipelineContextService.UpdateAsync(context);
            }
        }

        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == "UNIVERSITY";
        }
    }
}
