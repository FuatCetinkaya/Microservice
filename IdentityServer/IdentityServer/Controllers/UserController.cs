using IdentityServer.Dtos;
using IdentityServer.Models;
using IdentityServer4.Hosting.LocalApiAuthentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Shared.Dtos;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize(LocalApi.PolicyName)]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignupDto signupDto)
        {
            var user = new ApplicationUser 
            { 
                UserName = signupDto.UserName,
                Email = signupDto.Email,
                City= signupDto.City,
            };

            var result = await _userManager.CreateAsync(user, signupDto.Password);

            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(p=> p.Description).ToList();

                return BadRequest(Response<NoContent>.Fail(errors, 404));
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var useridClaim = User.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Sub);

            if(useridClaim == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(useridClaim.Value);

            if(user == null)
            {
                return BadRequest();
            }

            return Ok(new 
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                City = user.City,
            });
        }
    }
}
