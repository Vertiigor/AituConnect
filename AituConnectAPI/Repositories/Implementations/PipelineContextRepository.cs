using AituConnectAPI.Data;
using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AituConnectAPI.Repositories.Implementations
{
    public class PipelineContextRepository : Repository<PipelineContext>, IPipelineContextRepository
    {
        public PipelineContextRepository(ApplicationContext context) : base(context)
        {
        }

        public async Task<PipelineContext?> GetByChatIdAsync(string chatId)
        {
            return await _context.Pipelines.FirstOrDefaultAsync(p => p.ChatId == chatId);
        }
    }
}
