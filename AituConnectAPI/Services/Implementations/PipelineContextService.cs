using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Services.Implementations
{
    public class PipelineContextService : Service<PipelineContext>, IPipelineContextService
    {
        private readonly IPipelineContextRepository _pipelineRepository;

        public PipelineContextService(IPipelineContextRepository repository) : base(repository)
        {
            _pipelineRepository = repository;
        }

        public async Task<PipelineContext> GetByChatIdAsync(string chatId)
        {
            var pipeline = await _pipelineRepository.GetByChatIdAsync(chatId);
            if (pipeline == null)
            {
                return null;
            }

            return pipeline;
        }
    }
}
