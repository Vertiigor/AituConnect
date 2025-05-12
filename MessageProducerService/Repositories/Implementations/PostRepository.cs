using MessageProducerService.Data;
using MessageProducerService.Models;
using MessageProducerService.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace MessageProducerService.Repositories.Implementations
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationContext context) : base(context) { }
        
        public override async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await GetAllWithIncludes()
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllByUniversity(string university)
        {
            return await GetAllWithIncludes()
                .Where(p => p.University == university)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllByUserId(string userId)
        {
            return await GetAllWithIncludes()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public IQueryable<Post> GetAllWithIncludes()
        {
            return GetAllAsQueryable()
                .Include(p => p.User)
                .Include(p => p.Subjects);
        }
    }
}
