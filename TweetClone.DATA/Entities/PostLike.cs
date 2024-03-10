using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TweetClone.DATA.Entities.Enums;

namespace TweetClone.DATA.Entities
{
    public class PostLike
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public LikeStatus LikeStatus { get; set; }
        public Post Post { get; set; }

        public class PostLikeConfig : IEntityTypeConfiguration<PostLike>
        {
            public void Configure(EntityTypeBuilder<PostLike> builder)
            {
                // Primary key
                builder.HasKey(pl => pl.Id);

                // Relationships
                builder.HasOne(pl => pl.Post)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(pl => pl.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(pl => pl.User)
                    .WithMany(u => u.LikedPosts)
                    .HasForeignKey(pl => pl.UserId)
                    .OnDelete(DeleteBehavior.Cascade);



            }
        }
    }
}
