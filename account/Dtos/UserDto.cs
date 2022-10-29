
namespace account.Dtos
{
    public class UserDto
    {
        public string Id { get; set;} = null!;
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        
        public DateTime DateOfBirth {get; set;}
        public DateTime CreatedAt { get; set; }
        public string Role {get; set;} = null!;

        public int TotalFollowers{get; set;}
        public int TotalFollowings {get; set;}
        public int TotalTweets {get; set;}
        
    }
}