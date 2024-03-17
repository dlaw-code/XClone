using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using TweetClone.API.Services.Interface;
using TweetClone.DATA.Data;
using TweetClone.DATA.Entities;
using TweetClone.DATA.Entities.Enums;
using TweetClone.DATA.Models.Request;
using TweetClone.DATA.Models.Response;

namespace TweetClone.API.Services.Implementation
{
    public class TweetService : ITweetService
    {
        private readonly ApplicationDbContext _context;
        private readonly Cloudinary _cloudinary;

        public TweetService(ApplicationDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }


        public async Task<List<TweetGetResponse>> GetTweetsAsync()
        {
            var tweets = await _context.Posts.ToListAsync();
            return tweets.Select(t => new TweetGetResponse
            {
                Id = t.Id,
                Content = t.Content,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                UserId = t.UserId,
                ImageUrl = t.ImageUrl,

                //LikesCount = t.LikesCount
                // Map other properties as needed
            }).ToList();
        }

        public async Task<TweetGetResponse> GetTweetByIdAsync(int id)
        {
            // Retrieve the tweet from the database based on the provided ID
            var tweet = await _context.Posts.FirstOrDefaultAsync(t => t.Id == id);

            if (tweet == null)
            {
                return null;
            }

            // Map the tweet entity to a TweetDto object and return it
            return new TweetGetResponse
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

            if (string.IsNullOrWhiteSpace(request.Content) || request.Content.Length > 30)
            {

                throw new InvalidRequestException("Tweet content is required and cannot exceed 30 characters.");
                //return new PostTweetResponse
                //{
                //    Success = false,
                //    ErrorMessage = "The content cannot exceed 30 characters."
                //};
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


        public async Task<PostTweetResponse> UpdateTweetAsync(int id, UpdateTweetRequest request, IFormFile image)  
        {

            // Retrieve the tweet from the database
            var tweet = await _context.Posts.FirstOrDefaultAsync(t => t.Id == id);

            if (tweet == null)
            {
                return new PostTweetResponse
                {
                    Success = false,
                    ErrorMessage = "Tweet not found."
                };
            }

            // Update tweet content
            tweet.Content = request.Content;

            // Upload new image if provided
            if (image != null)
            {
                tweet.ImageUrl = await UploadImageAsync(image);
            }

            _context.Posts.Update(tweet);
            await _context.SaveChangesAsync();

            return new PostTweetResponse
            {
                Success = true,
                TweetId = tweet.Id
            };
        }


        public async Task<bool> DeleteTweetAsync(int id)
        {

            // Retrieve the tweet from the database
            var tweet = await _context.Posts.FirstOrDefaultAsync(t => t.Id == id);

            if (tweet == null)
            {
                return false; // Tweet not found
            }

            _context.Posts.Remove(tweet);
            await _context.SaveChangesAsync();

            return true; // Tweet successfully deleted
        }






        public async Task<bool> FollowUserAsync(string followerId, string followeeId)
        {
            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.SourceUserId == followerId && uf.FollowedUserId == followeeId);

            if (existingFollow != null)
            {
                return false; // Already following
            }

            var userFollow = new UserFollow
            {
                SourceUserId = followerId,
                FollowedUserId = followeeId
            };

            _context.UserFollows.Add(userFollow);
            await _context.SaveChangesAsync();
            //await UpdateFollowerCountAsync(followeeId);


            return true;
        }



        public async Task<int> GetFollowerCountAsync(string userId)
        {
            var followerCount = await _context.UserFollows
                .Where(uf => uf.FollowedUserId == userId)
                .CountAsync();

            return followerCount;
        }

        public async Task<bool> UnfollowUserAsync(string followerId, string followeeId)
        {
            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.SourceUserId == followerId && uf.FollowedUserId == followeeId);

            if (existingFollow == null)
                return false; // Follow relationship does not exist

            _context.UserFollows.Remove(existingFollow);
            await _context.SaveChangesAsync();

            await GetFollowerCountAsync(followeeId);
            //await UpdateFollowerCountAsync(followeeId);

            return true; // Unfollowed successfully
        }


        public async Task<bool> BlockUserAsync(string blockerId, string blockedUserId)
        {
            var existingBlock = await _context.UserBlocks
                .FirstOrDefaultAsync(ub => ub.SourceUserId == blockerId && ub.BlockedUserId == blockedUserId);

            if (existingBlock != null)
            {
                return false; // Already blocked
            }

            var userBlock = new UserBlock
            {
                SourceUserId = blockerId,
                BlockedUserId = blockedUserId
            };

            _context.UserBlocks.Add(userBlock);
            await _context.SaveChangesAsync();

            return true;


        }

        public async Task<bool> UnblockUserAsync(string blockerId, string userId)
        {
            var existingBlock = await _context.UserBlocks
                .FirstOrDefaultAsync(ub => ub.SourceUserId == blockerId && ub.BlockedUserId == userId);

            if (existingBlock == null)
                return false; // Block relationship does not exist

            _context.UserBlocks.Remove(existingBlock);
            await _context.SaveChangesAsync();

            return true; // Unblocked successfully
        }

        public async Task<CommentDto> PostCommentAsync(int postId, string userId, string content)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                return null;

            }

            var comment = new Comment
            {
                PostId = postId,
                UserId = userId, // Associate the comment with the user identified by their UserId
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return new CommentDto
            {
                Id = comment.Id,
                CommentId = comment.Id,
                UserId = comment.UserId,
                CreatedAt = comment.CreatedAt,
                Content = comment.Content // You can map other properties as needed
            };


        }

        public async Task<List<CommentDto>> GetCommentsAsync(int tweetId, int pageIndex, int pageSize)
        {
            var comments = await _context.Comments
                .Where(c => c.PostId == tweetId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return comments.Select(c => new CommentDto
            {
                Id = c.Id,
                CommentId = c.Id,
                UserId = c.UserId,
                CreatedAt = c.CreatedAt,
                Content = c.Content
            }).ToList();
        }

        public async Task<bool> LikeTweetAsync(int tweetId, string userId)
        {
            // Check if the user has already liked the tweet
            var existingLike = await _context.PostLikes
                .FirstOrDefaultAsync(like => like.PostId == tweetId && like.UserId == userId);

            if (existingLike != null)
            {
                // User has already liked the tweet
                return false;
            }

            // Add a new like
            var like = new PostLike
            {
                PostId = tweetId,
                UserId = userId,
                LikeStatus = LikeStatus.Liked
            };

            _context.PostLikes.Add(like);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> UnlikeTweetAsync(int tweetId, string userId)
        {
            // Find the like corresponding to the user and tweet
            var like = await _context.PostLikes
                .FirstOrDefaultAsync(like => like.PostId == tweetId && like.UserId == userId);

            if (like == null)
            {
                // User has not liked the tweet
                return false;
            }

            // Remove the like
            _context.PostLikes.Remove(like);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<int> GetTotalLikesCountAsync(int postId)
        {
            // Query the database for the count of total likes on the post
            var totalLikesCount = await _context.PostLikes
                .Where(pl => pl.PostId == postId)
                .CountAsync();

            return totalLikesCount;      
        }

        //public Task<PostTweetResponse> UpdateTweetAsync(int id, UpdateTweetRequest updaterequest, IFormFile image)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
