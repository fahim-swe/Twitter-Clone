using api.Cache;
using api.Helper;
using api.Middleware;
using core.Entities;
using core.Interfaces;
using core.Interfaces.RabbitMQ;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.Users
{
    [Authorize]
    [Route("like")]
    public class LikeController : ApiBaseController
    {
        private readonly IPublish<Likes> _likePublish;
        private readonly IPublish<Notification> _notificationPublish;
       
        private readonly IUnitOfWork _unitOfWork;
        public LikeController(IPublish<Likes> likePublish,IPublish<Notification> notificationPublish, IUnitOfWork unitOfWork)
        {
            _likePublish  = likePublish;
            _unitOfWork = unitOfWork;
            _notificationPublish = notificationPublish;
        }

        // [HttpGet("Count/{postId}")]
        //  [Cached(600)]
        // public async Task<IActionResult> CountTotalLikes([FromRoute]string postId)
        // {
        //     var count = await _likeRepository.CountLikes(postId);
        //     if(count == null) Ok(new Response<int>(0));
        //     return Ok(new Response<int>(count));
        // }
        //
        //
        // [HttpGet("Isliked/{postId}")]
        // [Cached(600)]
        // public async Task<IActionResult> IsLiked([FromRoute]string postId)
        // {
        //     var liked = await _likeRepository.isLiked(User.GetUserId(), postId);
        //     return Ok(new Response<Boolean>(liked));
        // }
        //
        //
        // [HttpGet("likes/{postId}")]
        //  [Cached(600)]
        // public async Task<IActionResult> LikesUsers([FromRoute] string postId, [FromQuery]PaginationFilter filter)
        // {
        //     var _filter = new PaginationFilter(filter.PageNumber, filter.PageSize);
        //     var result = await _likeRepository.LikesUserList(postId, _filter.PageNumber, _filter.PageSize);
        //
        //     var totallCount = await _likeRepository.CountAsync(filter => filter.TweetId == postId);
        //     
        //     var pagedResponse = new PagedResponse<IEnumerable<Likes>>(result, _filter.PageNumber, _filter.PageSize, totallCount);
        //     return Ok(pagedResponse);
        // }
        //



        [HttpPost("{postId}")]
        public async Task<IActionResult> LikeOnTweet([FromRoute]string postId)
        {
            var userId = User.GetUserId();
            var check = await _unitOfWork.LikeRepository.FindOneAsync(filter => filter.TweetId == postId && filter.UserId == User.GetUserId());

            if(check != null) return BadRequest(new Response<string>("Already Liked"));

            var like = new Likes
            {
                UserId = userId,
                FullName = User.GetFullName(),
                TweetId = postId
            };


            var tweet = await _unitOfWork.TweetRepository.FindOneAsync(filter => filter.id == postId);
            tweet.TotalLikes++;
            _unitOfWork.TweetRepository.ReplaceOneAsync(tweet.id, tweet);

            
            var notification = new Notification
            {
                To = tweet.UserId,
                From = userId,
                FullName = User.GetFullName(),
                PostId = postId,
                Type = "LIKE"
            };
            await _likePublish.publish(like);

            if(notification.To != notification.From)
                await _notificationPublish.publish(notification);


            return await _unitOfWork.Commit() ?  Ok(new Response<string>("Liked")): BadRequest(new Response<string>("Error"));
        }


        [HttpDelete("{postId}")]
        public async Task<IActionResult> RemoveLike([FromRoute]string postId)
        {

            var liked =await _unitOfWork.LikeRepository.FindOneAsync( filter => filter.TweetId == postId  && filter.UserId == User.GetUserId());

            if(liked == null) return BadRequest("Liked By other user!");

            var tweet = await _unitOfWork.TweetRepository.FindOneAsync(filter => filter.id == postId);
            tweet.TotalLikes--;
           
            _unitOfWork.TweetRepository.ReplaceOneAsync(tweet.id, tweet);
            
            _unitOfWork.LikeRepository.DeleteOneAsync( filter => filter.TweetId == postId && filter.UserId == User.GetUserId());
            
            return await _unitOfWork.Commit() ? Ok(new Response<string>("Ok")) : BadRequest(new Response<string>("Error"));
        }
    }
}
