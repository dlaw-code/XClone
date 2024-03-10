using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClone.DATA.Entities
{
    public class UserBlock
    {
        public int Id { get; set; }

        [ForeignKey(nameof(SourceUserId))]
        public ApplicationUser SourceUser { get; set; }
        public string SourceUserId { get; set; }

        [ForeignKey(nameof(BlockedUserId))]
        public ApplicationUser BlockedUser { get; set; }
        public string BlockedUserId { get; set; }

        public class UserBlockConfig : IEntityTypeConfiguration<UserBlock>
        {
            public void Configure(EntityTypeBuilder<UserBlock> builder)
            {
                builder.HasKey(ub => ub.Id);

                // Define relationships
                builder.HasOne(ub => ub.SourceUser)
                       .WithMany(u => u.BlockedUsers)
                       .HasForeignKey(ub => ub.SourceUserId)
                       .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
