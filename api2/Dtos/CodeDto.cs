using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api2.Dtos
{
    public class CodeDto
    {
        [Required]
        [EmailAddress]
        public String Email {get; set;}

        [Required]
        [MinLength(6,ErrorMessage ="must be 8 char")]
        public String Code {get; set;}

        [Required]
        // [CodePasswordValidation]
        public String Password {get; set;}
    }
}