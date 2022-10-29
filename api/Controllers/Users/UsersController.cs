using api.Dtos;
using api.Helper;
using AutoMapper;
using core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using api.Middleware;
using api.Cache;
using System.Linq.Expressions;
using core.Entities;

namespace api.Controllers.Users
{
    [Authorize]
    [Route("user")]
    public class UsersController  : ApiBaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UsersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        [HttpPut]
        public async Task<IActionResult> UpdateUserDetails(UUserDto uUserDto )
        {
            if(!ModelState.IsValid)
                return BadRequest(new Response<string>("BadFormate"));

            var Id = User.GetUserId();
            if(!await _unitOfWork.UserRepository.ExistsAsync(filter => filter.id == User.GetUserId()))
                return NotFound(new Response<string>("User Not Fuond"));


            var user = await _unitOfWork.UserRepository.FindOneAsync(filter => filter.id == Id);
            if(uUserDto.FullName != null) user.FullName = uUserDto.FullName;
            if(uUserDto.DateOfBirth != null) user.DateOfBirth = uUserDto.DateOfBirth;

            _unitOfWork.UserRepository.ReplaceOneAsync(Id, user);

            return (await _unitOfWork.Commit()) ? Ok(new Response<string>("Updated Successfully")) : BadRequest(new Response<string>("Opp!, Not updated! An unknown expection occurs"));
        }
          


        [HttpGet]
        [Route("get-users")]
        [Cached(180)]
        public async Task<IActionResult> GetUsersList([FromQuery]PaginationFilter filter)
        {
            var _filter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            Expression<Func<AppUser, Object>> sortByCreateDate = (s) => s.CreatedAt;
            
            var users =await _unitOfWork.UserRepository.FindManyAsync(filter => filter.isBlock == false, sortByCreateDate, _filter.PageNumber, _filter.PageSize);
            int totallCount =await _unitOfWork.UserRepository.CountAsync(filter => filter.isBlock == false);


            var userlist = new List<SUserDto>();
            foreach(var user in users)
            {
                if(
                    await _unitOfWork.UserBlockRepository.ExistsAsync( filter => filter.UserId == User.GetUserId() && filter.BlockId == user.id)

                    ||

                    await _unitOfWork.UserBlockRepository.ExistsAsync(filter => filter.UserId == user.id && filter.BlockId == User.GetUserId())

                ) continue;

                var _user = _mapper.Map<SUserDto>(user);
                _user.isFollow = await _unitOfWork.FollowRepository.ExistsAsync(filter => filter.UserId == User.GetUserId() && filter.Following == user.id);
                userlist.Add(_user);
            }

            var pagedResponse = new PagedResponse<IEnumerable<SUserDto>>(userlist, _filter.PageNumber, _filter.PageSize, totallCount);
            return Ok(pagedResponse);
        }



        [HttpGet("{id}")]
        [Cached(180)]
        public async Task<IActionResult> GetUser(string id)
        {
          if(!await _unitOfWork.UserRepository.ExistsAsync(filter => filter.id == id && filter.isBlock == false) 
            ||
          await _unitOfWork.UserBlockRepository.ExistsAsync(filter => filter.UserId == User.GetUserId() && filter.BlockId == id)

            || 
          await _unitOfWork.UserBlockRepository.ExistsAsync(filter => filter.UserId == id && filter.BlockId == User.GetUserId()
          
          )
          ) return NotFound(new Response<string>("Not found"));


          var user = await _unitOfWork.UserRepository.FindOneAsync(filter => filter.id == id &&filter.isBlock == false);
          var _user = _mapper.Map<SUserDto>(user);

          _user.isFollow = await _unitOfWork.FollowRepository.ExistsAsync(filter => filter.UserId == User.GetUserId() && filter.Following == id);
          return Ok(new Response<SUserDto>(_user));
        }        
    }
}
