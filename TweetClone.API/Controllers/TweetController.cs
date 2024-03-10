using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TweetClone.API.Services.Interface;
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

    }
}
