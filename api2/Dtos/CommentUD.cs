using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api2.Dtos
{
    public class CommentUD
    {
         [Required]
        [MinLength(24)]
         public string CommentId {get; set;} = null!;


         [Required]
        [MinLength(4,ErrorMessage ="Too short!")]
        public string Content {get; set;} = null!;
    }
}