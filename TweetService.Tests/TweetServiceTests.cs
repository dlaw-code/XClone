using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using TweetClone.API.Services.Implementation;
using TweetClone.DATA.Data;
using TweetClone.DATA.Entities;
using TweetClone.DATA.Models.Request;

namespace TweetServices.Tests
{
    public class TweetServiceTests : IAsyncLifetime
    {

        private readonly ApplicationDbContext _context;
        private readonly PostgreSqlContainer _postgresContainer;
        private readonly TweetService _tweetService;

        public TweetServiceTests()
        {
            // Initialize the PostgreSQL container
            _postgresContainer = new PostgreSqlBuilder()
                .WithDatabase("testdb")
                .WithUsername("testuser")
                .WithPassword("testpassword")
                .Build();

            // Start the container
            _postgresContainer.StartAsync().Wait();

            // Configure DbContext to use the PostgreSQL container
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_postgresContainer.GetConnectionString())
                .Options;

            _context = new ApplicationDbContext(options);


            // Apply migrations to the test database
            _context.Database.Migrate();

            // Initialize TweetService with the configured DbContext
            _tweetService = new TweetService(_context, null); // You can mock Cloudinary if needed
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            // Dispose the DbContext and stop the PostgreSQL container
            await _context.DisposeAsync();
            await _postgresContainer.DisposeAsync();
        }

        [Fact]
        public async Task CanCreatePostAsync()
        {
            // Arrange
            var user = new ApplicationUser
            {
                
                Email = "test@example.com",
                UserName = "password123"
                //Other properties as needed
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var postContent = "This is a test post content.";
            var imageFile = new FormFile(new MemoryStream(), 0, 0, "Data", "image.jpg");

            // Act
            var postId = await _tweetService.PostTweetAsync(new PostRequest { Content = postContent }, user.Id, imageFile);

            // Assert
            var retrievedPost = await _context.Posts.FindAsync(postId.TweetId);

            Assert.NotNull(retrievedPost);
            Assert.Equal(user.Id, retrievedPost.UserId);
            Assert.Equal(postContent, retrievedPost.Content);

            // Clean up
            _context.Posts.Remove(retrievedPost);
            await _context.SaveChangesAsync();

        }

        [Fact]
        public async Task CanGetTweetsAsync()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Email = "test@example.com",
                UserName = "password123"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Add some tweets to the database
            var tweet1 = new Post
            {
                UserId = user.Id,
                Content = "Tweet 1 content",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                ImageUrl = "image1.jpg"
            };
            var tweet2 = new Post
            {
                UserId = user.Id,
                Content = "Tweet 2 content",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                ImageUrl = "image2.jpg"
            };
            _context.Posts.AddRange(tweet1, tweet2);

            await _context.SaveChangesAsync();

            // Act
            var tweets = await _tweetService.GetTweetsAsync();

            // Assert
            Assert.NotNull(tweets);
            Assert.Equal(2, tweets.Count); // Assuming 2 tweets were added

            // Check tweet content
            Assert.Contains(tweets, t => t.Content == "Tweet 1 content");
            Assert.Contains(tweets, t => t.Content == "Tweet 2 content");

            // Check tweet user ID
            Assert.All(tweets, t => Assert.Equal(user.Id, t.UserId));

            // Clean up
            _context.Posts.RemoveRange(tweet1, tweet2);
            await _context.SaveChangesAsync();
        }
    }
}
