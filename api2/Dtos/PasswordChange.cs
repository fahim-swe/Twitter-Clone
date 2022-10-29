using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api2.Dtos
{
    public class PasswordChange
    {
        [Required]
        public string oldPassword {get; set;} = null!;

        [Required]
        public string newPassword {get; set;} = null!;
    }
}