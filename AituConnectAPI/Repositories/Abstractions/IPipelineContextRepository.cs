using AituConnectAPI.Models.Abstractions;

namespace AituConnectAPI.Repositories.Abstractions
{
    public interface IPipelineContextRepository : IRepository<PipelineContext>
    {
        public Task<PipelineContext> GetByChatIdAsync(string chatId);
    }
}
