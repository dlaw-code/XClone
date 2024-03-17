

using TweetClone.DATA.Models.Request;
using TweetClone.DATA.Models.Response;

namespace TweetClone.API.Services.Interface
{
    public interface ITweetService
    {
        Task<List<TweetGetResponse>> GetTweetsAsync();
        Task<TweetGetResponse> GetTweetByIdAsync(int id);
        Task<PostTweetResponse> PostTweetAsync(PostRequest request, string userId, IFormFile image);
        Task<PostTweetResponse> UpdateTweetAsync(int id, UpdateTweetRequest request, IFormFile image);
        Task<bool> DeleteTweetAsync(int id);
        Task<bool> FollowUserAsync(string followerId, string followeeId);
        Task<bool> UnfollowUserAsync(string followerId, string followeeId);
        Task<bool> BlockUserAsync(string blockerId, string userId);
        Task<bool> UnblockUserAsync(string blockerId, string userId);
        Task<int> GetFollowerCountAsync(string userId);
        Task<CommentDto> PostCommentAsync(int postId, string userId, string content);
        Task<List<CommentDto>> GetCommentsAsync(int tweetId, int pageIndex, int pageSize);
        Task<bool> LikeTweetAsync(int tweetId, string userId);
        Task<bool> UnlikeTweetAsync(int tweetId, string userId);
        Task<int> GetTotalLikesCountAsync(int postId);
        Task IncrementPostViewCountAsync(int postId);
        Task<int> GetTotalViewsCountForUserAsync(string userId);
        //Task<int> GetPostViewCountAsync(int postId);
        //Task<int> GetTotalViewCountAsync();


    }
}
