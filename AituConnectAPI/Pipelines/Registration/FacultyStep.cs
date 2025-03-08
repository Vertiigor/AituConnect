using AituConnectAPI.Bot;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Pipelines.Registration
{
    public class FacultyStep : PipelineStep
    {
        public FacultyStep(BotMessageSender messageSender, IPipelineContextService pipelineContextService, IUserService userService) : base(messageSender, pipelineContextService, userService)
        {
        }

        public override async Task ExecuteAsync(PipelineContext context)
        {
            if (string.IsNullOrEmpty(context.Content))
            {
                // Ask user for the title
                await _messageSender.SendTextMessageAsync(context.ChatId, "Enter the name of the educational program you're learning: ");
            }
            else
            {
                var user = await _userService.GetByChatIdAsync(context.ChatId);
                user.Faculty = context.Content;
                await _userService.UpdateAsync(user);

                context.CurrentStep = "CONGRATULATION"; // Move to the next step
                context.Content = string.Empty;
                await _pipelineContextService.UpdateAsync(context);
            }
        }

        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == "FACULTY";
        }
    }
}
