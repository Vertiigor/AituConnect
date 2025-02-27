using AituConnectAPI.Bot;
using AituConnectAPI.Models.Abstractions;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Pipelines.Registration
{
    public class CongratulationStep : PipelineStep
    {
        private readonly IPipelineContextService _pipelineContextService;
        private readonly IUserService _userService;
        public CongratulationStep(BotClient botClient, IPipelineContextService pipelineContextService, IUserService userService) : base(botClient)
        {
            _pipelineContextService = pipelineContextService;
            _userService = userService;
        }

        public async override Task ExecuteAsync(PipelineContext context)
        {
            var user = await _userService.GetByChatIdAsync(context.ChatId);
            // Mark the pipeline as completed
            context.IsCompleted = true;
            await _pipelineContextService.UpdateAsync(context);
            await _botClient.SendTextMessageAsync(context.ChatId, $"Welcome! You have been registered. These are your data:\nUsername: {user.UserName}\nUniversity: {user.University}\nFaculty: {user.Faculty}");
        }

        public override bool IsApplicable(PipelineContext context)
        {
            return context.CurrentStep == "CONGRATULATION";
        }
    }
}
