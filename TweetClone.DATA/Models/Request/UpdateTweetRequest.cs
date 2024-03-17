using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetClone.DATA.Models.Request
{
    public class UpdateTweetRequest
    {
        public string Content { get; set; }
        public IFormFile Image { get; set; }
    }
}
