using MessageProducerService.Models;
using MessageProducerService.Repositories.Abstractions;
using MessageProducerService.Services.Abstractions;

namespace MessageProducerService.Services.Implementations
{
    public class PostService : Service<Post>, IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository repository) : base(repository)
        {
            _postRepository = repository;
        }
    }
}
