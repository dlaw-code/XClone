using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClone.DATA.Entities
{
    public class UserFollow
    {
        public int Id { get; set; }

        //[ForeignKey(nameof(SourceUserId))]
        public ApplicationUser SourceUser { get; set; }
        public string SourceUserId { get; set; }

        //[ForeignKey(nameof(FollowedUserId))]
        public ApplicationUser FollowedUser { get; set; }
        public string FollowedUserId { get; set; }

        public class UserFollowConfig : IEntityTypeConfiguration<UserFollow>
        {
            public void Configure(EntityTypeBuilder<UserFollow> builder)
            {
                builder.HasKey(uf => uf.Id);

                // Define relationships
                builder.HasOne(uf => uf.SourceUser)
                       .WithMany(u => u.Following)
                       .HasForeignKey(uf => uf.SourceUserId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(uf => uf.FollowedUser)
                       .WithMany(u => u.Followers)
                       .HasForeignKey(uf => uf.FollowedUserId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
