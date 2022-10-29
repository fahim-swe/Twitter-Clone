using System.Linq.Expressions;
using api.Cache;
using api.Dtos;
using api.Helper;
using api.Middleware;
using AutoMapper;
using core.Dtos;
using core.Entities;
using core.Interfaces;
using core.Interfaces.RabbitMQ;

using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.Users
{
    [Authorize]
    [Route("tweet")]
    public class TweetController : ApiBaseController
    {
        private readonly IPublish<Tweet> _tweetPublish;
        private readonly IPublish<Notification> _notificationPublish;
        private readonly IPublish<Comments> _commentPublish;
        private readonly IPublish<Likes> _likePublish;


        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TweetController(IPublish<Tweet> tweetPublish, IMapper mapper, IPublish<Comments> commentPublish, IPublish<Likes> likePublish, IPublish<Notification> notificationPublish, IUnitOfWork unitOfWork) 
        {
            _tweetPublish = tweetPublish;
            _mapper = mapper;
            _commentPublish = commentPublish;
            _likePublish = likePublish;
            _unitOfWork = unitOfWork;
            _notificationPublish = notificationPublish;
        }


        [HttpPost]
        public async Task<IActionResult> CreateTweet(CreateTweetDto createTweet)
        {
            if(!ModelState.IsValid)
                return BadRequest(new Response<String>("Wrong Formate"));

            var userId = User.GetUserId();
            
            if(createTweet.HashTag != "")
                this.saveHashTag(createTweet.HashTag);

            var tweet = _mapper.Map<Tweet>(createTweet);
            tweet.UserId = User.GetUserId();
            tweet.FullName = User.GetFullName();
            tweet.UserName = User.GetUserName();
           

            await _tweetPublish.publish(tweet);
            
            var user = await _unitOfWork.UserRepository.FindOneAsync(filter => filter.id == userId);
            user.TotalTweets++;
            _unitOfWork.UserRepository.ReplaceOneAsync(userId, user);

            return await _unitOfWork.Commit() ? Ok(new Response<string>("Published")) : BadRequest(new Response<string>("Error to create tweet")); 
        }

        
        [HttpPost]
        [Route("{tweetId}")]
        public async Task<IActionResult> RetweetPost([FromRoute]string tweetId)
        {
            if(!await _unitOfWork.TweetRepository.IsValidTweet(tweetId)) return NotFound();

            var userId = User.GetUserId();

            var retweet = new Tweet
            {
                IsRetweet = true,
                Content = tweetId,
                UserId = userId,
                FullName = User.GetFullName(),
                UserName = User.GetUserName()
            };

            var user = await _unitOfWork.UserRepository.FindOneAsync(filter => filter.id == userId);
            user.TotalTweets++;
            _unitOfWork.UserRepository.ReplaceOneAsync(userId, user);

            
            var tweet = await _unitOfWork.TweetRepository.FindOneAsync(filter => filter.id == tweetId);
            tweet.TotalRetweets++;
            _unitOfWork.TweetRepository.ReplaceOneAsync(tweetId, tweet);
            
            
            var notification = new Notification
            {
                To = await _unitOfWork.TweetRepository.FilterOneAsync(filter => filter.id == tweetId, projectionExpression: filter => filter.UserId),
                From = user.id,
                FullName = user.FullName,
                PostId = tweetId,
                Type = "RETWEET"
            };
            
            if(notification.To != notification.From)
                await _notificationPublish.publish(notification);
            
            await _tweetPublish.publish(retweet);

            return await _unitOfWork.Commit() ? Ok(new Response<string>("Retweeted")) : BadRequest(new Response<string>("Error to retweet")); 
        }

    
        [HttpDelete]
        [Route("{tweetId}")]
        public async Task<IActionResult> DeleteTweet([FromRoute]string tweetId)
        {
            var tweet = await _unitOfWork.TweetRepository.FindOneAsync( x=> x.id == tweetId && x.UserId == User.GetUserId());

            if(tweet == null) return BadRequest(new Response<string>("U can't delete other's tweet"));

            if(tweet.HashTag != null) await this.deleteHashTag(tweet.HashTag);

            var user = await _unitOfWork.UserRepository.FindOneAsync(filter => filter.id == User.GetUserId());
            user.TotalTweets--;
            

            _unitOfWork.UserRepository.ReplaceOneAsync(user.id, user);
            _unitOfWork.TweetRepository.DeleteManyAsync(x => x.Content == tweetId);
            _unitOfWork.TweetRepository.DeleteOneAsync(x => x.id == tweetId);
            _unitOfWork.LikeRepository.DeleteManyAsync( x=> x.TweetId == tweetId);
            _unitOfWork.CommentRepository.DeleteManyAsync(x => x.TweetId == tweetId);
        

            return await _unitOfWork.Commit() ? Ok(new Response<string>("Deleted Post sucessfully")) : BadRequest(new Response<string>("Error while delete"));
        }

    
       private async void saveHashTag(string text)
       {
            var words = text.Split(' ');
            Console.WriteLine(words);

            foreach(var word in words)
            {
                var hashTag = await _unitOfWork.HashTagRepository.FindOneAsync(filter => filter.hashTag == word);

                if(hashTag == null){
                    var _hashTag = new HashTag
                    {
                        hashTag = word,
                        Count = 1
                    };
                    await _unitOfWork.HashTagRepository.InsertOne(_hashTag);
                }
                else {
                    hashTag.Count++;
                    hashTag.CreatedAt = DateTime.Now;
                    _unitOfWork.HashTagRepository.ReplaceOneAsync(hashTag.id, hashTag);
                    await _unitOfWork.Commit();
                }
            }
       }
    
        
      
       private async Task deleteHashTag(string text)
       {
          var punctuation = text.Where(Char.IsPunctuation).Distinct().ToArray();
           var words = text.Split().Select(x => x.Trim(punctuation));

          foreach(var word in words)
            {
                var hashTag = await _unitOfWork.HashTagRepository.FindOneAsync(filter => filter.hashTag == word);

                if(hashTag != null){
                    try{
                        hashTag.Count--;
                        if(hashTag.Count == 0) _unitOfWork.HashTagRepository.DeleteOneAsync(filter => filter.hashTag == word);
                        else 
                            _unitOfWork.HashTagRepository.ReplaceOneAsync(hashTag.id, hashTag);
                        await _unitOfWork.Commit();
                    }
                    catch(Exception e)
                    {
                        
                    }
                }
            }

            return ;
       }
    }
}
