using AituConnectAPI.Data;
using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AituConnectAPI.Repositories.Implementations
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ApplicationContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Post>> GetAllByAuthorIdAsync(string authorId)
        {
            return await _context.Posts.Where(p => p.AuthorId == authorId).ToListAsync();
        }

        public async Task<Post> GetByAuthorIdAsync(string authorId)
        {
            return await _context.Posts.FirstOrDefaultAsync(p => p.AuthorId == authorId);
        }
    }
}
