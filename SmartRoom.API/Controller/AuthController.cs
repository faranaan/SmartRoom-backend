using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartRoom.API.Data;
using SmartRoom.API.DTOs;
using SmartRoom.API.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SmartRoom.API.Controller
{
    /// <summary>
    /// Authentication service for handling user registration and login using JWT.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        // database and config settings
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Register a new user into the system.
        /// </summary>
        /// <param name="request">
        /// Registration data containing Username, Password, and Role.
        /// </param>
        /// <response code="200">User registered successfully.</response>
        /// <response code="400">Username already exists or invalid input.</response>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("Username already exists.");
            }

            if(request.Role == UserRole.Admin)
            {
                var serverSecret = _configuration.GetSection("AppSettings:AdminSecretKey").Value;

                if (string.IsNullOrEmpty(request.AdminSecretKey) || request.AdminSecretKey != serverSecret)
                {
                    return BadRequest("Invalid Admin Secret Key! You are not authorized to register as an Admin");
                }
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Password = passwordHash,
                Role = request.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        /// <summary>
        /// Verify user credentials and generates a JWT access token.
        /// </summary>
        /// <remarks>
        /// The generated token contains the following claims:
        /// - NameIdentifier (User ID)
        /// - Name (Username)
        /// - Role (User Role)
        /// This token must be included in the request header:
        /// Authorization: Beaerer [token]
        /// to access protected endpoints.
        /// </remarks>
        /// <param name="request">
        /// Login data containing Username and Password.
        /// </param>
        /// <returns>JWT Token as a string</returns>
        /// <response code="200">Login successful and token returned.</response>
        /// <response code="401">Invalid credentials or user not found.</response>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            string token = CreateToken(user);

            return Ok(new { token });
        }

        /// <summary>
        /// Internal method to generate a JWT token.
        /// </summary>
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;

        }
    }
}