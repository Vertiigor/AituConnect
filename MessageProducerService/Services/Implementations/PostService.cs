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

        public async Task<Post> GetLastDraftedPost(string userId)
        {
            var posts = await _postRepository.GetAllAsync();
            var lastDraftedPost = posts
                .Where(p => p.UserId == userId && p.Status == Status.Draft)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefault();

            if (lastDraftedPost == null)
            {
                throw new Exception("No drafted post found for the user.");
            }

            return lastDraftedPost;
        }
    }
}
