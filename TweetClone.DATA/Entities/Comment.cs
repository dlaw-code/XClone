using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClone.DATA.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public Post Post { get; set; } // Navigation property to Post

        // Navigation property to IdentityUser
        public ApplicationUser User { get; set; }

        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public class CommentConfig : IEntityTypeConfiguration<Comment>
        {
            public void Configure(EntityTypeBuilder<Comment> builder)
            {
                builder.HasKey(c => c.Id);

                // Define relationships
                builder.HasOne(c => c.User)
                       .WithMany()
                       .HasForeignKey(c => c.UserId)
                       .OnDelete(DeleteBehavior.Cascade); // Example: If a user is deleted, delete all associated comments
            }
        }
    }
}
