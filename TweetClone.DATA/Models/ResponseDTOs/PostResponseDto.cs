using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClone.DATA.Models.Response
{
    public class PostTweetResponse
    {
        public bool Success { get; set; }
        public int? TweetId { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
