using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using VerifyEmailForgetPassword.Data;
using VerifyEmailForgetPassword.Models;

namespace VerifyEmailForgetPassword.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
       
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
          if(_context.Users.Any(u=>u.Email==request.Email))
            {                      
                return BadRequest("User already exists");

            }

            CreatePasswordhash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = CreateRandomToken()


            };
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User Succssfully Created!");


 
    }
        [HttpPost(template: "Login")]

        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var user=await _context.Users.FirstOrDefaultAsync(u=>u.Email==request.Email);
            if(user==null)
            {
                return BadRequest("user Not Found ");
            }
            if (!VerifyPasswordhash(request.Password, user.PasswordHash, user.PasswordSalt))

            {
                return BadRequest("Password is incorrect.");
            }
            if (user.VerifiedAt == null)
            {
               return BadRequest("Not Verified!");

            }
          
            return Ok($"Welcome Back ,{user.Email}!");



        }
        [HttpPost(template: "Verify")]

        public async Task<IActionResult>Verify(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
              if(user==null ) { return BadRequest("InValid token"); }
            user.VerifiedAt=DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok("User Verifed !");




        }
        [HttpPost(template: "forget-password")]

        public async Task<IActionResult> ForgetPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) {
                return BadRequest("InValid email"); }
            user.ResetTokenExpires = DateTime.Now.AddMinutes(30);
            user.PasswordResetToken=CreateRandomToken();

            await _context.SaveChangesAsync();
            return Ok("your password May Be Reset  !");



        }
        [HttpPost(template: "Reset-password")]

        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);
            if (user == null|| user.ResetTokenExpires<DateTime.Now)
            {
                return BadRequest("InValid Token");
            }
            CreatePasswordhash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.ResetTokenExpires = null;
            user.PasswordResetToken = null;

            await _context.SaveChangesAsync();
            return Ok("your password has changed Successfully !");



        }



        private static void CreatePasswordhash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac= new HMACSHA512())
            {

                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));    

            }
        }
        private static bool VerifyPasswordhash(string password,  byte[] passwordHash,  byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {


                var  ComputedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return ComputedHash.SequenceEqual(passwordHash);
            }
        }
        private static string  CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
    }
}
