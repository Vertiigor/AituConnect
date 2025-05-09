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
            var posts = await _context.Posts
                .Include(p => p.Subjects)
                .ToListAsync();

            return posts;
        }
    }
}
