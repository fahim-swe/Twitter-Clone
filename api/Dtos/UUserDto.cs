
using System.ComponentModel.DataAnnotations;
using api.Helpers;

namespace api.Dtos
{
    public class UUserDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Provide Last Name")]  
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Name Should be min 5 and max 100 length")]
        public string FullName {get; set;} = null!;


        [Display(Name = "Date of Birth")]  
        [DataType(DataType.Date)]  

        public DateTime DateOfBirth {get; set;}

    }
}