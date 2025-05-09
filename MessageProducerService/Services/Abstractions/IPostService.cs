using MessageProducerService.Models;

namespace MessageProducerService.Services.Abstractions
{
    public interface IPostService : IService<Post>
    {
        public Task<Post> GetLastDraftedPost(string userId);
        public Task<IEnumerable<Post>> GetAllPostsByUserId(string userId);
        public Task<IEnumerable<Post>> GetAllByUniversity(string university);

    }
}
