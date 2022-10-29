using System.ComponentModel.DataAnnotations;
using account.Helpers;

namespace account.Dtos
{
    public class Signup
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Last Name")]  
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Last Name Should be min 5 and max 20 length")]
        public string FullName {get; set;} = null!;


        [Display(Name = "Date of Birth")]  
        [DataType(DataType.Date)]  
        [Min18Years] 
        public DateTime DateOfBirth {get; set;}

        [Required]
        [MinLength(4)]
        public string UserName {get; set;} = null!;

        [Required]
        [EmailAddress]
        public string Email {get; set;} = null!;
        public DateTime CreatedAt {get; protected set;} = DateTime.Now;


        [ValidatePassword]
        public string Password {get; set;} = null!;
    }
}