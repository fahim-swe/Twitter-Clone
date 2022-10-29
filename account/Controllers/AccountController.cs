using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using account.Dtos;
using account.Helper;
using account.Helpers;
using AutoMapper;
using core.Entities;
using core.Interfaces;
using core.Interfaces.Email;
using core.Interfaces.Redis;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.Users
{
    [ApiController]    
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IRedisService _resetCode;
        private readonly IMailService _mail;
        private readonly IMapper _mapper;


        public AccountController(IRedisService resetCode, IUnitOfWork unitOfWork, ITokenService tokenService,IMailService mail, IMapper mapper)
        {
           
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mail = mail;
            _mapper = mapper;
            _resetCode = resetCode;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Signup(Signup signup)
        {
            if(!ModelState.IsValid){
                return BadRequest(new Response<String>("Wrong Formate"));
            }
            var _user = await _unitOfWork.UserRepository.ExistsAsync(filter => filter.UserName == signup.UserName);

            if(_user) return Conflict(new Response<String>("Username Already Exits"));

            if(await _unitOfWork.UserRepository.ExistsAsync(filter => filter.Email == signup.Email)){
                return Conflict(new Response<String>("Email Already Exits"));
            }
            
            
            using var hmac = new HMACSHA512();
            var user = _mapper.Map<AppUser>(signup);

            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signup.Password));
            user.PasswordSalt = hmac.Key;
            await _unitOfWork.UserRepository.InsertOne(user);
            
            var userDto = _mapper.Map<UserDto>(user);

            var tokenDto = _mapper.Map<TokenDto>(_tokenService.CreateToken(user));

            tokenDto.RefreshToken = new RandomCode().GenerateRefreshToken();
            userDto.Role = user.Role;

            
            _unitOfWork.TokenRepository.InsertOneAsync(_mapper.Map<Token>(tokenDto));
            
            return await _unitOfWork.Commit() ? Ok(new Response<Tuple<UserDto,TokenDto>>(Tuple.Create(userDto, tokenDto))) : BadRequest(new Response<String>("Error to create account"));
        }
    

        [HttpPost]
        [Route("login")]
        
        public async Task<ActionResult> Login(Login loginDto )
        {
            if(!ModelState.IsValid){
                return BadRequest(new Response<String>("Wrong Formate"));
            }

            var user = await _unitOfWork.UserRepository.FindOneAsync( x => x.UserName == loginDto.UserName);
            if(user == null) return BadRequest (new Response<String>("Invalid username"));
          
            var date = await _unitOfWork.AdminBlockRepository.FilterOneAsync(filter => filter.UserId == user.id, projectionExpression:filter => filter.CreatedAt);

            if(user.isBlock) return Unauthorized(new Response<string>("Block By Admin" + date.ToString()));
        
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i < computedHash.Length; i++){
                if(computedHash[i] != user.PasswordHash[i]){
                    return BadRequest(new Response<String>("Wrong Password"));
                }
            }

            var userDto = _mapper.Map<UserDto>(user);
            var tokenDto =_mapper.Map<TokenDto>(_tokenService.CreateToken(user));
            tokenDto.RefreshToken = new RandomCode().GenerateRefreshToken();
            userDto.Role = user.Role;
            _unitOfWork.TokenRepository.InsertOneAsync(_mapper.Map<Token>(tokenDto));

            
            return await _unitOfWork.Commit() ? Ok(new Response<Tuple<UserDto,TokenDto>>(Tuple.Create(userDto, tokenDto))) : BadRequest(new Response<String>("Error to login"));
        }


        [HttpGet]
        [Route("valid-username")]
        public async Task<IActionResult> ValidUserName( [RegularExpression(@"^[a-zA-Z0-9]{3,15}", ErrorMessage = " username has to be start with letters and contains letters and number characters in 6 to 12 length")]string userName)
        {
            return !await _unitOfWork.UserRepository.ExistsAsync(filter => filter.UserName == userName) ? Ok(new Response<bool>(true)) : BadRequest(new Response<bool>(false));
        }


       
        [HttpPost]
        [Route("refresh-key")]
        public async Task<IActionResult> RefreshKey(RToken rtoken)
        {

            if(!ModelState.IsValid) BadRequest(new Response<string>("Invalid"));

            string token = rtoken.token;
            string refreshKey = rtoken.refreshKey;

            if(!await _unitOfWork.TokenRepository.ExistsAsync(filter => filter.UToken == token && filter.RefreshToken == refreshKey)) return NotFound(new Response<string>("Not found"));


        
            string refreshToken = refreshKey;
            var principal = _tokenService.GetPrincipalFromExpiredToken(token);
            if(principal == null) return BadRequest(new Response<string>("wrong token"));

            var tokenDto = _mapper.Map<TokenDto>(_tokenService.CreateToken(principal.Claims.ToList()));   
            tokenDto.RefreshToken = new RandomCode().GenerateRefreshToken();
            
            _unitOfWork.TokenRepository.InsertOneAsync(_mapper.Map<Token>(tokenDto));
            _unitOfWork.TokenRepository.DeleteOneAsync(filter => filter.UToken == token);
        

            return await _unitOfWork.Commit() ? Ok(new Response<TokenDto>(tokenDto)) : BadRequest(new Response<string>("Error"));
        }
    }
}
