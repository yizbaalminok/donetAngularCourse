

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repository, IConfiguration config)
        {
            _config = config;
            _repository = repository;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDTO registrationDTO)
        {
            // Validation of request

            registrationDTO.username = registrationDTO.username.ToLower();

            if (await _repository.UserExists(registrationDTO.username))
            {
                return BadRequest("username already exists");
            }

            var newUser = new User
            {
                Username = registrationDTO.username
            };

            var createdUser = await _repository.Register(newUser, registrationDTO.password);

            return StatusCode(201);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO loginDTO)
        {
            var user = await _repository.Login(loginDTO.username.ToLower(), loginDTO.password);

            if (user == null)
            {
                return Unauthorized();
            }

            // Set up the claims our authenticated user can make. They can say who they are (their ID and user name)
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // Create secret
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.
                GetBytes(_config.GetSection("AppData:TokenKeySeed").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create token
            var tokenDesc = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity( claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDesc);

            // return the token as a field in a JSON object
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });

        }
    }
}