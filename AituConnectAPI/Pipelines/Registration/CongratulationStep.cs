using AituConnectAPI.Bot;
using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Pipelines.Registration
{
    public class CongratulationStep : PipelineStep
    {
        public CongratulationStep(BotMessageSender messageSender, IPipelineContextService pipelineContextService, IUserService userService) : base(messageSender, pipelineContextService, userService)
        {
        }

        public async override Task ExecuteAsync(PipelineContext context)
        {
            var user = await _userService.GetByChatIdAsync(context.ChatId);
            // Mark the pipeline as completed
            context.IsCompleted = true;
            context.FinishedDate = DateTime.UtcNow;
            await _pipelineContextService.UpdateAsync(context);
            await _pipelineContextService.DeleteAsync(context.Id);
            await _messageSender.SendTextMessageAsync(context.ChatId, $"Welcome! You have been registered. These are your data:\nUsername: {user.UserName}\nUniversity: {user.University}\nFaculty: {user.Faculty}");
        }

        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == "CONGRATULATION";
        }
    }
}
