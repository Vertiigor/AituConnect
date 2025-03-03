using AituConnectAPI.Models;

namespace AituConnectAPI.Services.Abstractions
{
    public interface IPipelineContextService : IService<PipelineContext>
    {
        public Task<PipelineContext> GetByChatIdAsync(string chatId);
    }
}
