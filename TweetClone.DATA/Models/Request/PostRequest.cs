using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TweetClone.DATA.Models.Request
{
    public class PostRequest
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        public IFormFile Image { get; set; }
    }
}
