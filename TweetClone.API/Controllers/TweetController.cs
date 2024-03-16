using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TweetClone.API.Services.Interface;
using TweetClone.DATA.Entities.Enums;
using TweetClone.DATA.Entities;
using TweetClone.DATA.Models.Request;
using TweetClone.DATA.Models.Response;

namespace TweetClone.API.Controllers
{
    [Route("api/tweet")]
    [ApiController]
    public class TweetController : ControllerBase
    {

        private readonly ITweetService _tweetService;

        public TweetController(ITweetService tweetService)
        {
            _tweetService = tweetService;
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<PostDto>>> GetTweets()
        {
            var tweets = await _tweetService.GetTweetsAsync();
            return Ok(tweets);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PostDto>> GetTweet(int id)
        {
            var tweet = await _tweetService.GetTweetByIdAsync(id);
            return Ok(tweet);
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostTweetResponse>> PostTweet([FromForm] PostRequest request, [FromForm] IFormFile image)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = await _tweetService.PostTweetAsync(request, userId, image);

            return response.Success
            ? CreatedAtAction(nameof(GetTweet), new { id = response.TweetId }, response)
            : BadRequest(response.ErrorMessage);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<PostTweetResponse>> UpdateTweet(int id, [FromForm] PostRequest request, [FromForm] IFormFile image)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _tweetService.UpdateTweetAsync(id, request, image);

            // Return a 200 OK response with the updated tweet's ID
            return Ok(response);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteTweet(int id)
        {
            var success = await _tweetService.DeleteTweetAsync(id);

            return NoContent(); // Tweet successfully deleted
        }

        [HttpPost("follow/{followeeId}")]
        [Authorize]
        public async Task<ActionResult> FollowUser(string followeeId)
        {
            string followerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _tweetService.FollowUserAsync(followerId, followeeId);

            if (success)
            {
                return Ok(new { message = "User followed successfully" });
            }
            else
            {
                return BadRequest(new { error = "User already followed or an error occurred" });
            }
        }

        [HttpPost("unfollow/{followeeId}")]
        public async Task<ActionResult> UnfollowUser(string followeeId)
        {
            string followerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool success = await _tweetService.UnfollowUserAsync(followerId, followeeId);

            if (success)
            {
                return Ok(new { message = "Unfollowed user successfully" });
            }

            return BadRequest(new { error = "You are not following this user" });
        }

        [HttpDelete("block/{userId}")]
        [Authorize]
        public async Task<ActionResult> BlockUser(string userId)
        {
            string blockerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _tweetService.BlockUserAsync(blockerId, userId);

            if (success)
            {
                return Ok(new { message = "User blocked successfully" });
            }

            return BadRequest(new { error = "User already blocked or an error occurred" });
        }

        [HttpPost("unblock/{userId}")]
        public async Task<ActionResult> UnblockUser(string userId)
        {
            string blockerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool success = await _tweetService.UnblockUserAsync(blockerId, userId);

            if (success)
            {
                return Ok(new { message = "Unblocked user successfully" });
            }

            return BadRequest(new { error = "User is not blocked or an error occurred" });
        }

        [HttpPost("{postId}/comments")] // Use postId in the route template
        [Authorize]
        public async Task<ActionResult<CommentDto>> PostComment(int postId, [FromBody] CommentRequest request)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = await _tweetService.PostCommentAsync(postId, userId, request.Content);

            if (comment == null)
            {
                return NotFound(); // Post not found
            }

            return Ok(comment);
        }


        [HttpGet("{tweetId}/comments")]
        [Authorize]
        public async Task<ActionResult<List<CommentDto>>> GetComments(int tweetId, int pageIndex = 0, int pageSize = 10)
        {
            var comments = await _tweetService.GetCommentsAsync(tweetId, pageIndex, pageSize);
            return Ok(comments);
        }

        [HttpPost("{tweetId}/like")]
        [Authorize]
        public async Task<ActionResult> LikeTweet(int tweetId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var success = await _tweetService.LikeTweetAsync(tweetId, userId);

            if (success)
            {
                return Ok(new { message = "Tweet liked successfully" });
            }

            return BadRequest(new { error = "You have already liked this tweet" });
        }


        [HttpPost("{tweetId}/unlike")]
        [Authorize]
        public async Task<ActionResult> UnlikeTweet(int tweetId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var success = await _tweetService.UnlikeTweetAsync(tweetId, userId);

            //if (success)
            //{
                return Ok(new { message = "Tweet unliked successfully" });
            //}

            return BadRequest(new { error = "You have not liked this tweet" });
        }


        [HttpGet("{tweetId}/total-likes")]
        [Authorize]
        public async Task<ActionResult<int>> GetTotalLikesCount(int tweetId)
        {
            // Call the tweet service method to get the total likes count
            var totalLikesCount = await _tweetService.GetTotalLikesCountAsync(tweetId);

            // Return the total likes count
            return Ok(totalLikesCount);
        }


    }
}
