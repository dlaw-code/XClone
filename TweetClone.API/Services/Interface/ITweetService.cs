

using TweetClone.DATA.Models.Request;
using TweetClone.DATA.Models.Response;

namespace TweetClone.API.Services.Interface
{
    public interface ITweetService
    {
        Task<List<PostDto>> GetTweetsAsync();
        Task<PostDto> GetTweetByIdAsync(int id);
        Task<PostTweetResponse> PostTweetAsync(PostRequest request, string userId, IFormFile image);

    }
}
