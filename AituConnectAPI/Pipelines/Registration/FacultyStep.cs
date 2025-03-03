using AituConnectAPI.Bot;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Pipelines.Registration
{
    public class FacultyStep : PipelineStep
    {
        private readonly IPipelineContextService _pipelineContextService;
        private readonly IUserService _userService;

        public FacultyStep(BotClient botClient, IPipelineContextService pipelineContextService, IUserService userService) : base(botClient)
        {
            _pipelineContextService = pipelineContextService;
            _userService = userService;
        }

        public override async Task ExecuteAsync(PipelineContext context)
        {
            if (string.IsNullOrEmpty(context.Content))
            {
                // Ask user for the title
                await _botClient.SendTextMessageAsync(context.ChatId, "Enter the name of the educational program you're learning: ");
            }
            else
            {
                var user = await _userService.GetByChatIdAsync(context.ChatId);
                user.Faculty = context.Content;
                await _userService.UpdateAsync(user);

                context.CurrentStep = "CONGRATULATION"; // Move to the next step
                await _pipelineContextService.UpdateAsync(context);
            }
        }

        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == "FACULTY";
        }
    }
}
