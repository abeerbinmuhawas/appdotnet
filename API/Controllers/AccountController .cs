using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController :BaseApiController
    {
        private readonly DataContext _context;

        private readonly ITokenService _tokenService;
        public AccountController(DataContext context , ITokenService tokenService)
        {
            _context = context;
        }  
         
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){

            if(await UserExists(registerDto.Username)) return BadRequest("UserName is Taken");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(), 
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswirdSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }
            [HttpPost("login")]
            public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){

                var user = await _context.Users.SingleOrDefaultAsync(x=>x.UserName == loginDto.UserName);

                if(user == null) return Unauthorized("Invalid username");

                using var hmac = new HMACSHA512(user.PasswirdSalt);

                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                for(int i=0; i<computedHash.Length;i++){
                    if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
                }
                return new UserDto
            {
                   Username = user.UserName,
                   Token = _tokenService.CreateToken(user)
            };
            }


        private async Task<bool> UserExists(string usernamr){

            return await _context.Users.AnyAsync(X=> X.UserName == usernamr.ToLower());

        }
        
     }

}