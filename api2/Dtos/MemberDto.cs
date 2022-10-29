using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api2.Dtos
{
    public class MemberDto
    {
        public string id {get; set;}
        public string FullName { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public DateTime DateOfBirth {get; set;}

        public DateTime CreatedAt {get; set;}
        public Boolean isBlock {get; set;}

        public int TotalFollowers{get; set;}
        public int TotalFollowings {get; set;}
        public int TotalTweets {get; set;}
    }
}