using MessageProducerService.Models;

namespace MessageProducerService.Services.Abstractions
{
    public interface IPostService :  IService<Post>
    {
        public Task<Post> GetLastDraftedPost(string userId);
    }
}
