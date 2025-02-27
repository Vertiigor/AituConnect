using AituConnectAPI.Models.Abstractions;

namespace AituConnectAPI.Services.Abstractions
{
    public interface IPipelineContextService : IService<PipelineContext>
    {
        public Task<PipelineContext> GetByChatIdAsync(string chatId);
    }
}
