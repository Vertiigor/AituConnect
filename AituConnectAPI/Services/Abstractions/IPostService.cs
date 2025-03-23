using AituConnectAPI.Models;

namespace AituConnectAPI.Services.Abstractions
{
    public interface IPostService : IService<Post>
    {
        public Task<Post> GetByAuthorIdAsync(string authorId);
        public Task<IEnumerable<Post>> GetAllByAuthorIdAsync(string authorId);
        public Task<Post> GetLastDraftByAuthorIdAsync(string authorId);
        public Task<Post> GetLastPublishedByAuthorIdAsync(string authorId);
    }
}
