using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class CreateTweetDto
    {
        [Required]
        [MinLength(10,ErrorMessage ="Too short!")]
        public string Content {get; set;} = null!;
        public string? HashTag {get; set;} 
    }
}