using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities;

namespace core.Interfaces
{
    public interface ITweetRepository : IRepository<Tweet>
    {
     
        Task<IEnumerable<Tweet>> SearchTweet(string hashTag, int pageNumber, int pageSize);
        Task<IEnumerable<Tweet>> UserTimeLine(string userId, int pageNumber, int pageSize);
        Task<IEnumerable<HashTag>> HashTags(int pageNumber, int pageSize);
        Task<int> UserTimeLineCount(string userId);
        Task<int> HashTagPostCount(string hashTag);
        Task<Boolean> IsValidTweet(string postId);
        Task<IEnumerable<string>> Newsfeeds(string userId, int pageNumber, int  pageSize);
    }

    public class TweetId 
    {
        public string Id {get; set;}
    }
}