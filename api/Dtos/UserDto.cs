
using core.Dtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api.Dtos
{
    public class UserDto
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]     
        public string Id { get; set;}
        public string FullName { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        
        public DateTime DateOfBirth {get; set;}
        public DateTime CreatedAt { get; set; }
        public string Role {get; set;}

        public int TotalFollowers{get; set;}
        public int TotalFollowings {get; set;}
        public int TotalTweets {get; set;}
        
    }
}