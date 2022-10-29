using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class LikeDto
    {
        public string TweetId {get; set;} = null!;
        public string UserId {get; set;} = null!;

        
    }
}