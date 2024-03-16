using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClone.DATA.Models.Response
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
