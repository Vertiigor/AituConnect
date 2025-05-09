using MessageProducerService.Models;

namespace MessageProducerService.Repositories.Abstractions
{
    public interface IPostRepository : IRepository<Post>
    {
        public Task<IEnumerable<Post>> GetAllByUniversity(string university);

    }
}
