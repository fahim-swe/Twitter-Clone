using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos
{
    public class SUserDto
    {
        public string id {get; set;}
        public string FullName { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth {get; set;}
        public DateTime CreatedAt {get; set;}
        public string Role {get; set;} = "User";
        public int TotalFollowers{get; set;}
        public int TotalFollowings {get; set;}
        public int TotalTweets {get; set;}
        public Boolean isFollow {get; set;}
    }
}