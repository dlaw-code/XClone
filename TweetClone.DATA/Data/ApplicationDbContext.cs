using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TweetClone.DATA.Entities;
using static TweetClone.DATA.Entities.Comment;
using static TweetClone.DATA.Entities.CommentLike;
using static TweetClone.DATA.Entities.Post;
using static TweetClone.DATA.Entities.PostLike;
using static TweetClone.DATA.Entities.UserBlock;
using static TweetClone.DATA.Entities.UserFollow;

namespace TweetClone.DATA.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<UserBlock> UserBlocks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.ApplyConfiguration(new PostConfig());
            modelBuilder.ApplyConfiguration(new CommentConfig());
            modelBuilder.ApplyConfiguration(new PostLikeConfig());
            modelBuilder.ApplyConfiguration(new CommentLikeConfig());
            modelBuilder.ApplyConfiguration(new UserFollowConfig());
            modelBuilder.ApplyConfiguration(new UserBlockConfig());

        }

    }
}
