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

        public async Task<IEnumerable<Post>> GetAllByAuthorIdAsync(string authorId)
        {
            return await _postRepository.GetAllByAuthorIdAsync(authorId);
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

        public async Task<Post> GetLastDraftByAuthorIdAsync(string authorId)
        {
            var posts = await GetAllByAuthorIdAsync(authorId);
            var sortedPosts = posts.Where(p => p.Status == PostStatus.Draft).OrderByDescending(p => p.CreationDate).ToList();

            if (sortedPosts.Count == 0)
                return null;

            return sortedPosts.First();
        }

        public async Task<Post> GetLastPublisherByAuthorIdAsync(string authorId)
        {
            var posts = await GetAllByAuthorIdAsync(authorId);
            var sortedPosts = posts.Where(p => p.Status == PostStatus.Published).OrderByDescending(p => p.CreationDate).ToList();

            if (sortedPosts.Count == 0)
                return null;

            return sortedPosts.First();
        }
    }
}
