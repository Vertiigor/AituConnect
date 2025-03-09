using AituConnectAPI.Bot;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Pipelines.Editing.Profile
{
    public class FacultyEditingStep : PipelineStep
    {
        public FacultyEditingStep(BotMessageSender messageSender, IPipelineContextService pipelineContextService, IUserService userService) : base(messageSender, pipelineContextService, userService)
        {
        }

        public override async Task ExecuteAsync(PipelineContext context)
        {
            if (string.IsNullOrEmpty(context.Content))
            {
                var user = await _userService.GetByChatIdAsync(context.ChatId);
                // Ask user for the title
                await _messageSender.SendTextMessageAsync(context.ChatId, $"Current: {user.Faculty}\n(Editing) Enter the name of the educational program you're learning: ");
            }
            else
            {
                var user = await _userService.GetByChatIdAsync(context.ChatId);
                user.Faculty = context.Content;
                await _userService.UpdateAsync(user);

                context.IsCompleted = true;
                context.FinishedDate = DateTime.UtcNow;
                await _pipelineContextService.DeleteAsync(context.Id);
            }
        }

        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == PipelineStepType.Faculty;
        }
    }
}
