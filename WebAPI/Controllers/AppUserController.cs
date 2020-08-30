using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Model;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private ApplicationSettings _appSettings;
        private ProductContext _context;

        public AppUserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<ApplicationSettings> appSettings, ProductContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _context = context;

        }
        [HttpPost]
        [Route("Register")]
        //POST : /api/AppUser/Register
        public async Task<Object> PostAppUser(AppUserModel model)
        {

            var appUser = new AppUser()
            {
                Name = model.Name,
                Surname = model.Surname,
                Email=model.Email,
                UserName=model.Username,
                Image=model.Image
                
                

            };
            try
            {
                var result = await _userManager.CreateAsync(appUser, model.Password);
                return Ok(result);
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        [Route("updateUser")]
        public async Task<Object> UpdateProduct(AppUser model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.Image = model.Image;
            await _userManager.UpdateAsync(user);
            _context.Users.Update(user);
            _context.SaveChanges();
            return Ok(user);
        }
        [HttpPost]
        [Route("Login")]
        //POST /api/AppUser/Login
        public async Task<IActionResult> Login(LoginModel model)
        {
            
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user,model.Password))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });

            }
            else
            {
                return BadRequest(new { message = "Username or password incorrect." });
            }
        }
        [HttpGet]
        [Authorize]
        [Route("GetUserProfile")]
        public async Task<Object> GetUserProfile()
        {
            
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return new
            {
                user.Email,
                user.Name,
                user.Surname,
                user.UserName,
                user.Image
            };
        }

    }
}