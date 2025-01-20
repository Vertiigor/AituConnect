namespace AituConnectAPI.Policies.Abstractions
{
    public interface IPolicy
    {
        public Task<bool> CanEditPostsAsync(string userId);
    }
}
