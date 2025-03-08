using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.PostCreation;
using AituConnectAPI.Pipelines.Registration;
using Telegram.Bot;

namespace AituConnectAPI.Bot
{
    public class PipelineHandler
    {
        private readonly Dictionary<string, Func<PipelineContext, Task>> _handlers;
        private readonly ITelegramBotClient _botClient;
        private readonly RegistrationPipeline _registrationPipeline;
        private readonly PostCreationPipeline _postCreationPipeline;

        public PipelineHandler(IServiceProvider serviceProvider, ITelegramBotClient telegramBotClient, RegistrationPipeline registrationPipeline, PostCreationPipeline postCreationPipeline)
        {
            _handlers = new Dictionary<string, Func<PipelineContext, Task>>
            {
                ["REGISTRATION"] = async (context) => await HandleRegistration(context),
                ["POST"] = async (context) => await HandlePost(context)
            };
            _botClient = telegramBotClient;
            _registrationPipeline = registrationPipeline;
            _postCreationPipeline = postCreationPipeline;
        }

        private async Task HandlePost(PipelineContext context)
        {
            await _postCreationPipeline.ExecuteAsync(context);
        }

        private async Task HandleRegistration(PipelineContext context)
        {
            await _registrationPipeline.ExecuteAsync(context);
            //await _registrationPipeline.ExecuteAsync(context);
        }

        public async Task HandlePipelineAsync(PipelineContext context)
        {
            if (context == null)
            {
                return;
            }
            if (_handlers.TryGetValue(context.Type, out var handler))
            {
                await handler(context);
            }
        }
    }
}
