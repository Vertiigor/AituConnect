using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Services.Implementations
{
    public class PipelineContextService : Service<PipelineContext>, IPipelineContextService
    {
        private readonly IPipelineContextRepository _repository;
        public PipelineContextService(IPipelineContextRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<PipelineContext> GetByChatIdAsync(string chatId)
        {
            var pipeline = await _repository.GetByChatIdAsync(chatId);
            if (pipeline == null)
            {
                return null;
            }

            return pipeline;
        }
    }
}
