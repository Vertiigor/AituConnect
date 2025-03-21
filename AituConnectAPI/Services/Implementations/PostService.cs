using AituConnectAPI.Models;
using AituConnectAPI.Repositories.Abstractions;
using AituConnectAPI.Services.Abstractions;

namespace AituConnectAPI.Services.Implementations
{
    public class PostService : Service<Post>, IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository repository) : base(repository)
        {
            _postRepository = repository;
        }

        public async Task<Post> GetByAuthorIdAsync(string authorId)
        {
            var post = await _postRepository.GetByAuthorIdAsync(authorId);
            if (post == null)
            {
                return null;
            }

            return post;
        }
    }
}
