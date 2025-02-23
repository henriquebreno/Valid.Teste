using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Valid.Teste.API.Models;
using Valid.Teste.Domain.Entities;
using Valid.Teste.Domain.Interfaces;

namespace Valid.Teste.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMapper _mapper;

        public AuthController(IProfileRepository profileRepository, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var profile = await _profileRepository.GetByProfileName(request.ProfileName);

            if (profile == null)
                return Unauthorized("Profile not found.");


            var profileParameter = _mapper.Map<ProfileParameter>(profile);
            var claims = new List<Claim>
            {
                new Claim("ProfileName", profileParameter.ProfileName)
            };
            foreach (var item in profileParameter.Parameters)
            {
                claims.Add(new Claim(item.Key, item.Value));
            }

            var identity = new ClaimsIdentity(claims, "custom");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);

            return Ok(new { message = "Login successful." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok(new { message = "Logout successful." });
        }
    }
}
