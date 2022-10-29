using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using core.Entities;
using core.Interfaces;
using core.Interfaces.Redis;
using StackExchange.Redis;

namespace infrastructure.Services.Redis
{
    public class Timeline : ITimeLine
    {
        private readonly IDatabase _database;
        private readonly IRepository<Follow> _followRepository;
        
        public Timeline(IConnectionMultiplexer redis, IRepository<Follow> repository)
        {
            _database = redis.GetDatabase();
            _followRepository = repository;
        }

        public async Task AddTweet(Tweet tweet)
        {

            var follwer = _followRepository.FilterBy( filter => filter.Following == tweet.UserId, projectionExpression: filter => filter.UserId);

            foreach(string user in follwer)
            {
                var key = (typeof(Tweet).Name) + user;
                if(this.Count(key) >= 10){
                   await this.removeAt(key, 0);
                }

                await this.saveHashTag("HashTag", tweet.HashTag);
                await _database.ListRightPushAsync(key, JsonSerializer.Serialize(tweet));
            }
            
            return ;
        }

        public Task<bool> DeleteTimeline(string userId)
        {
            throw new NotImplementedException();
        }


        public int Count(string key)
        {
            return (int)_database.ListLength(key);
        }

        public async Task<TweetBasket> UserTimeline(string userId)
        {
            var data = await _database.StringGetAsync((typeof(Tweet).Name)+userId);
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<TweetBasket>(data);
        }



       
        public async Task<IEnumerable<Tweet>> GetTweets(string userId)
        {
            var key = (typeof(Tweet).Name) + userId;
            List<Tweet>  tweets = new List<Tweet>();
            for(int i = this.Count(key)-1; i >= 0; i--)
            {
                var tweet =JsonSerializer.Deserialize<Tweet>(await _database.ListGetByIndexAsync(key, i));
                tweets.Add(tweet);
            }
            return tweets;
        }


        public async Task saveHashTag(string key, string text)
        {
            var punctuation = text.Where(Char.IsPunctuation).Distinct().ToArray();
            var words = text.Split().Select(x => x.Trim(punctuation));

            foreach(var word in words)
            {
                if(this.Count(key) >= 10){
                    await this.removeAt(key, 0);
                }
                var _hashTag = await this.Contains(key, word);
                if(_hashTag != null){
                    _hashTag.Item2.Count++;
                    await Insert(key, _hashTag.Item1, _hashTag.Item2);
                }
                else {
                    var tag = new HashTag{
                        hashTag = word, Count = 1
                    };
                    await _database.ListRightPushAsync(key, JsonSerializer.Serialize(tag));
                }
            }
        }

        private async Task removeAt(string key, int index)
        {
            var value = await _database.ListGetByIndexAsync(key, index);
            if(!value.IsNull){
               await _database.ListRemoveAsync(key, value);
            }
        }

        private async Task Insert(string key, int index, HashTag hashTag)
        {            
            await _database.ListSetByIndexAsync(key, index, JsonSerializer.Serialize(hashTag));
        }

        
        private async Task<Tuple<int, HashTag>> Contains(string key, string hashTag)
        {
            for (int i = 0; i < this.Count(key); i++)
            {
                  var Tag = JsonSerializer.Deserialize<HashTag>(await _database.ListGetByIndexAsync(key, i));
                  if(Tag.hashTag == hashTag) return Tuple.Create(i, Tag);
            }
            return null;
        }
    }
}