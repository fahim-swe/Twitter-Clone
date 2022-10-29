using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class CommentDto
    {
        [Required]
        [MinLength(24)]
        public string TweetId {get; set;} = null!;


        [Required]
        [MinLength(4,ErrorMessage ="Too short!")]
        public string Content {get; set;} = null!;
    }
}