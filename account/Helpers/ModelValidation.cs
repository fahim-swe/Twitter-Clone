
using System.ComponentModel.DataAnnotations;
using account.Dtos;

namespace account.Helpers
{
    public class Min18Years : ValidationAttribute  
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)  
        { 
            var user = (Signup)validationContext.ObjectInstance;  
  
            if (user.DateOfBirth == null)  
                return new ValidationResult("Date of Birth is required.");  
  
            var age = DateTime.Today.Year - user.DateOfBirth.Year;  
  
            return (age >= 18)  
                ? ValidationResult.Success  
                : new ValidationResult("Student should be at least 18 years old.");  
        }  
    }

    public class CodePasswordValidation  :  ValidationAttribute 
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)  
        {  
            var user = (CodeDto)validationContext.ObjectInstance; 
            var passWord = user.Password;
       
            int validConditions = 0;     
            foreach(char c in passWord)    
            {    
                if (c >= 'a' && c <= 'z')    
                {    
                    validConditions++;    
                    break;    
                }     
            }     
            foreach(char c in passWord)    
            {    
                if (c >= 'A' && c <= 'Z')    
                {    
                    validConditions++;    
                    break;    
                }     
            }     
            
            if (validConditions == 0) return new ValidationResult("Must include at least one uppercase & lowercase Latter"); 
                
            foreach(char c in passWord)    
            {    
                if (c >= '0' && c <= '9')    
                {    
                    validConditions++;    
                    break;    
                }     
            }     
            if (validConditions == 1) return new ValidationResult("Must include at number[0-9]");      
            if(validConditions == 2)    
            {    
                char[] special = {'@', '#', '$', '%', '^', '&', '+', '='}; // or whatever    
                if (passWord.IndexOfAny(special) == -1) new ValidationResult("Special Chat is required");     
            }     
            
            return ValidationResult.Success;
        }  
    }

    public class ValidatePassword :  ValidationAttribute 
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)  
        {  
            var user = (Signup)validationContext.ObjectInstance; 

            var passWord = user.Password;
       
            int validConditions = 0;     
            foreach(char c in passWord)    
            {    
                if (c >= 'a' && c <= 'z')    
                {    
                    validConditions++;    
                    break;    
                }     
            }     
            foreach(char c in passWord)    
            {    
                if (c >= 'A' && c <= 'Z')    
                {    
                    validConditions++;    
                    break;    
                }     
            }     
            
            if (validConditions == 0) return new ValidationResult("Must include at least one uppercase & lowercase Latter"); 
                
            foreach(char c in passWord)    
            {    
                if (c >= '0' && c <= '9')    
                {    
                    validConditions++;    
                    break;    
                }     
            }     
            if (validConditions == 1) return new ValidationResult("Must include at number[0-9]");      
            if(validConditions == 2)    
            {    
                char[] special = {'@', '#', '$', '%', '^', '&', '+', '='}; // or whatever    
                if (passWord.IndexOfAny(special) == -1) new ValidationResult("Special Chat is required");     
            }     
            
            return ValidationResult.Success;
        }  
    }
}