using MessageListenerService.Models;

namespace MessageListenerService.Services.Abstractions
{
    public interface IPostService : IService<Post>
    {
        public Task<Post> GetLastDraftedPost(string userId);
        public Task<IEnumerable<Post>> GetAllPostsByUserId(string userId);
    }
}
