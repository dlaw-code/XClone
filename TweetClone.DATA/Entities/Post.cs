using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClone.DATA.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } // Navigation property to IdentityUser
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property to comments
        public List<Comment> Comments { get; set; } = [];

        public List<PostLike> Likes { get; set; } = [];

        public class PostConfig : IEntityTypeConfiguration<Post>
        {
            public void Configure(EntityTypeBuilder<Post> builder)
            {
                // Define relationships
                builder.HasOne(p => p.User)
                       .WithMany()
                       .HasForeignKey(p => p.UserId)
                       .OnDelete(DeleteBehavior.Cascade); // Example: If a user is deleted, delete all associated posts

                // Configure the relationship with comments
                builder.HasMany(p => p.Comments)
                       .WithOne(c => c.Post)
                       .HasForeignKey(c => c.PostId)
                       .OnDelete(DeleteBehavior.Cascade);
            }
        }
    }
}
