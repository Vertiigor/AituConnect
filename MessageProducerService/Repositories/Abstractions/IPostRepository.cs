using MessageProducerService.Models;

namespace MessageProducerService.Repositories.Abstractions
{
    public interface IPostRepository : IRepository<Post>
    {
        public Task<IEnumerable<Post>> GetAllByUniversity(string university);
        public Task<IEnumerable<Post>> GetAllByUserId(string userId);
        public IQueryable<Post> GetAllWithIncludes();
    }
}
