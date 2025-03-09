using AituConnectAPI.Models;
using AituConnectAPI.Pipelines.Abstractions;
using AituConnectAPI.Pipelines.Editing.Profile;
using AituConnectAPI.Pipelines.PostCreation;
using AituConnectAPI.Pipelines.Registration;

namespace AituConnectAPI.Bot
{
    public class PipelineHandler
    {
        private readonly Dictionary<PipelineType, Func<PipelineContext, Task>> _handlers;
        private readonly RegistrationPipeline _registrationPipeline;
        private readonly PostCreationPipeline _postCreationPipeline;
        private readonly ProfileEditingPipeline _profileEditingPipeline;

        public PipelineHandler(IServiceProvider serviceProvider, RegistrationPipeline registrationPipeline, PostCreationPipeline postCreationPipeline, ProfileEditingPipeline profileEditingPipeline)
        {
            _handlers = new Dictionary<PipelineType, Func<PipelineContext, Task>>
            {
                [PipelineType.Registration] = async (context) => await HandleRegistration(context),
                [PipelineType.PostCreation] = async (context) => await HandlePost(context),
                [PipelineType.ProfileEditing] = async (context) => await HandleProfileEditing(context)
            };
            _registrationPipeline = registrationPipeline;
            _postCreationPipeline = postCreationPipeline;
            _profileEditingPipeline = profileEditingPipeline;
        }

        private async Task HandleProfileEditing(PipelineContext context)
        {
            await _profileEditingPipeline.ExecuteAsync(context);
        }

        private async Task HandlePost(PipelineContext context)
        {
            await _postCreationPipeline.ExecuteAsync(context);
        }

        private async Task HandleRegistration(PipelineContext context)
        {
            await _registrationPipeline.ExecuteAsync(context);
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
