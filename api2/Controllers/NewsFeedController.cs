using System.Linq.Expressions;
using api2.Cache;
using api2.Dtos;
using api2.Helper;
using api2.Middleware;
using AutoMapper;
using core.Entities;
using core.Interfaces;
using core.Interfaces.Redis;
using Microsoft.AspNetCore.Mvc;

namespace api2.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class NewsFeedController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IResponseCacheService _cache;
        private readonly IMapper _mapper;

        public NewsFeedController(IUnitOfWork unitOfWork, IResponseCacheService cache, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _mapper = mapper;
        }
        

        [HttpGet]
        [Route("{tweetId}")]
        [Cached(10)]
        public async Task<IActionResult> GetTweet([FromRoute]string tweetId)
        {
            var result = await _unitOfWork.TweetRepository.FindOneAsync(filter => filter.id == tweetId);

            var tweet = _mapper.Map<TweetDto>(result);
            tweet.isLiked = await _unitOfWork.LikeRepository.ExistsAsync(filter => filter.UserId == User.GetUserId() && filter.TweetId == tweetId);

            return Ok(new Response<TweetDto>(tweet));
        }


        [HttpGet("hashTag")]
        [Cached(10)]
        public async Task<IActionResult> HashTagList([FromQuery]PaginationFilter filter)
        {
            var _filter = new PaginationFilter(filter.PageNumber, filter.PageSize);
        
            var results = await _unitOfWork.TweetRepository.HashTags(_filter.PageNumber, _filter.PageSize);
        
            var totallCount = await _unitOfWork.HashTagRepository.CountAsync(x => true);
            var pagedResponse = new PagedResponse<IEnumerable<HashTag>>(results, _filter.PageNumber, _filter.PageSize, totallCount);
            return Ok(pagedResponse);
        }
        

        [HttpGet]
        [Route("newsfeed")]
        [Cached(180)]
        public async Task<IActionResult> Newsfeed([FromQuery]PaginationFilter filter)
        {
          
             Console.WriteLine("Timeline");
            var _filter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var timeline = await _unitOfWork.TweetRepository.UserTimeLine(User.GetUserId(), _filter.PageNumber++, _filter.PageSize);

            var tweets = await this.addLikedAndFilterBlockTweet(User.GetUserId(),timeline);
          
            var totallCount = await _unitOfWork.TweetRepository.UserTimeLineCount(User.GetUserId());

            var pagedResponse = new PagedResponse<IEnumerable<TweetDto>>(tweets, _filter.PageNumber, _filter.PageSize, totallCount);
            
            return Ok(pagedResponse);
        }



        [HttpGet]
        [Route("tweets/{userId}")]
        [Cached(10)]
        public async Task<IActionResult> GetTweets([FromRoute]String userId, [FromQuery]PaginationFilter filter)
        {
            if(!await _unitOfWork.UserRepository.ExistsAsync(filter => filter.id == userId && filter.isBlock == false))
                return NotFound(new Response<String>("User Not Found")); 
        
            var _filter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            
            Expression<Func<Tweet, Object>> sortByCreateDate = (s) => s.CreatedAt;
            var UserId = userId;
            var myTweets = await _unitOfWork.TweetRepository.FindManyAsync(filter => filter.UserId == userId, sortByCreateDate,_filter.PageNumber, _filter.PageSize);
           
            var tweetList = await this.addLikedAndFilterBlockTweet(userId, myTweets);
            var totallCount = await _unitOfWork.TweetRepository.CountAsync( x => x.UserId == UserId);


            var pagedResponse = new PagedResponse<IEnumerable<Object>>( tweetList, _filter.PageNumber, _filter.PageSize, totallCount);
            return Ok(pagedResponse);
        }





        private async Task<IEnumerable<TweetDto>> addLikedAndFilterBlockTweet(string userId, IEnumerable<Tweet> tweets)
        {
            var tweetList = new List<TweetDto>();
            foreach(var tweet in tweets)
            {
                var _tweet = _mapper.Map<TweetDto>(tweet);
                _tweet.isLiked = await _unitOfWork.LikeRepository.ExistsAsync(filter => filter.UserId == userId && filter.TweetId == tweet.id);

                if(_tweet.IsRetweet){
                    var retweet = await _unitOfWork.TweetRepository.FindOneAsync(filter => filter.id == _tweet.Content);
                    if(!await _unitOfWork.UserBlockRepository.ExistsAsync(filter => filter.UserId == userId && filter.BlockId == retweet.UserId)){
                        _tweet.ParentTweet = _mapper.Map<TweetDto>(retweet);
                    }
                 }

                tweetList.Add(_tweet);
            }
            
            return tweetList;
        }
    }
}