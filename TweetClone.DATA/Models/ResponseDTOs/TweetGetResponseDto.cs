using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClone.DATA.Models.Response
{
    public class TweetGetResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ViewCount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
