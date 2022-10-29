using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities;

namespace core.Interfaces.Redis
{
    public interface ITimeLine
    {
        Task<TweetBasket> UserTimeline(string userId);
        Task<bool> DeleteTimeline(string userId);
        Task AddTweet(Tweet tweet);
        Task<IEnumerable<Tweet>> GetTweets(string userId);
    }
}