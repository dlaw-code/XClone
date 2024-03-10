using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClone.DATA.Entities
{
    public class CommentLike
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string UserId { get; set; }

        public Comment Comment { get; set; }

        // Navigation property to IdentityUser
        public ApplicationUser User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public class CommentLikeConfig : IEntityTypeConfiguration<CommentLike>
        {
            public void Configure(EntityTypeBuilder<CommentLike> builder)
            {
                builder.HasKey(cl => cl.Id);

                // Define relationships
                builder.HasOne(cl => cl.User)
                       .WithMany()
                       .HasForeignKey(cl => cl.UserId)
                       .OnDelete(DeleteBehavior.Cascade);

                // Define the relationship with the Comment entity
                builder.HasOne(cl => cl.Comment)
                       .WithMany()  // Since we're not storing likes directly in the Comment entity
                       .HasForeignKey(cl => cl.CommentId);
            }
        }
    }
}
