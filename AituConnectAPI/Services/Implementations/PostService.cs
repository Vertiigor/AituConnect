using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using AituConnectAPI.Repositories.Implementations;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Services.Implementations
{
    public class PostService : Service<Post>, IPostService
    {
        private readonly IPostRepository _repository;
        public PostService(IPostRepository repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<Post> GetByAuthorIdAsync(string authorId)
        {
            var post = await _repository.GetByAuthorIdAsync(authorId);
            if (post == null)
            {
                return null;
            }

            return post;
        }
    }
}
