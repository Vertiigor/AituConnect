using AituConnectAPI.Models;

namespace AituConnectAPI.Services.Abstractions
{
    public interface IPostService : IService<Post>
    {
        public Task<Post> GetByAuthorIdAsync(string authorId);
    }
}
