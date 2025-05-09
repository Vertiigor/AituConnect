using MessageListenerService.Models;
using MessageListenerService.Repositories.Abstractions;
using MessageListenerService.Services.Abstractions;

namespace MessageListenerService.Services.Implementations
{
    public class PostService : Service<Post>, IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository repository) : base(repository)
        {
            _postRepository = repository;
        }

        public async Task<IEnumerable<Post>> GetAllPostsByUserId(string userId)
        {
            var posts = await _postRepository.GetAllAsync();

            var userPosts = posts
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            if (userPosts.Count == 0)
            {
                Console.WriteLine("No posts found for the user.");
            }

            return userPosts;
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
