using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Helper
{
    public static class ClaimsPrincipleData
    {
        public static string GetUserName(this ClaimsPrincipal user){
            return user.FindFirst(ClaimTypes.Name).Value;
        }

        public static string GetUserId(this ClaimsPrincipal user)
         {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;
         }

        public static string GetFullName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.GivenName).Value;
        }
    
    }
}