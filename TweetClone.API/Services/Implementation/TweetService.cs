using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using TweetClone.API.Services.Interface;
using TweetClone.DATA.Data;
using TweetClone.DATA.Entities;
using TweetClone.DATA.Models.Request;
using TweetClone.DATA.Models.Response;

namespace TweetClone.API.Services.Implementation
{
    public class TweetService: ITweetService
    {
        private readonly ApplicationDbContext _context;
        private readonly Cloudinary _cloudinary;

        public TweetService(ApplicationDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }


        public async Task<List<PostDto>> GetTweetsAsync()
        {
            var tweets = await _context.Posts.ToListAsync();
            return tweets.Select(t => new PostDto
            {
                Id = t.Id,
                Content = t.Content,
                CreatedAt = t.CreatedAt,
                UserId = t.UserId,
                ImageUrl = t.ImageUrl,

                //LikesCount = t.LikesCount
                // Map other properties as needed
            }).ToList();
        }

        public async Task<PostDto> GetTweetByIdAsync(int id)
        {
            // Retrieve the tweet from the database based on the provided ID
            var tweet = await _context.Posts.FirstOrDefaultAsync(t => t.Id == id);

            if (tweet == null)
            {
                return null;
            }

            // Map the tweet entity to a TweetDto object and return it
            return new PostDto
            {
                Id = tweet.Id,
                UserId = tweet.UserId,
                Content = tweet.Content,
                ImageUrl = tweet.ImageUrl,
                // Map other properties as needed
            };
        }



        private async Task<string> UploadImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return null; // or throw an exception if needed
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(image.FileName, image.OpenReadStream()),
                // Optionally, you can specify additional parameters such as folder, tags, etc.
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult.SecureUrl.ToString();
        }


        public async Task<PostTweetResponse> PostTweetAsync(PostRequest request, string userId, IFormFile image)
        {

            if (request.Content.Length > 30)
            {
                return new PostTweetResponse
                {
                    Success = false,
                    ErrorMessage = "The content cannot exceed 30 characters."
                };
            }
            var imageUrl = await UploadImageAsync(request.Image);
            var post = new Post
            {
                Content = request.Content,
                ImageUrl = imageUrl,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return new PostTweetResponse
            {
                Success = true,
                TweetId = post.Id
            };


        }

    }


}
