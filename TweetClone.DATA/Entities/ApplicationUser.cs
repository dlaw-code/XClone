using Microsoft.AspNetCore.Identity;

namespace TweetClone.DATA.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<PostLike> LikedPosts { get; set; }
        public virtual ICollection<UserFollow> Following { get; set; }
        public virtual ICollection<UserFollow> Followers { get; set; }
        public virtual ICollection<UserBlock> BlockedUsers { get; set; }
    }
}
