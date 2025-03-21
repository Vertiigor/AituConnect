using AituConnectAPI.Models;

namespace AituConnectAPI.Repositories.Abstractions
{
    public interface IPostRepository : IRepository<Post>
    {
        public Task<Post> GetByAuthorIdAsync(string authorId);
        public Task<IEnumerable<Post>> GetAllByAuthorIdAsync(string authorId);
    }
}
