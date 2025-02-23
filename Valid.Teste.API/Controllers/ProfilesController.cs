using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Valid.Teste.API.Models;
using Valid.Teste.Domain.Entities;
using Valid.Teste.Domain.Interfaces;
using Profile = Valid.Teste.Domain.Entities.Profile;

namespace Valid.Teste.API.Controllers
{
    [ApiController]
    [Route("api/profiles")]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger<ProfilesController> _logger;
        private readonly IMapper _mapper;

        public ProfilesController(IProfileRepository profileRepository, ILogger<ProfilesController> logger, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _logger = logger;
            _mapper = mapper;         
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProfiles()
        {
            try
            {
                var profiles = await _profileRepository.GetAll();
                var result = profiles.Select(profile => _mapper.Map<ProfileParameter>(profile)).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all profiles.");
                return StatusCode(500, "An error occurred while fetching the profiles.");
            }
        }

        [HttpGet("{profileName}")]
        [Authorize]
        public async Task<IActionResult> GetProfile(string profileName)
        {
            try
            {
                var profile = await _profileRepository.GetByProfileName(profileName);
                if (profile == null) {
                    return Ok($"Profile '{profileName}' not found.");
                }
                var result = _mapper.Map<ProfileParameter>(profile);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching the profile '{ProfileName}'.", profileName);
                return StatusCode(500, "An error occurred while fetching the profile.");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddProfile([FromBody] ProfileParameter profileParameter)
        {
            try
            {
                if (profileParameter == null)
                    return BadRequest("Profile data is required.");

                var profile = _mapper.Map<Profile>(profileParameter);
                var createdId = await _profileRepository.Add(profile);
                return CreatedAtAction(nameof(GetProfile), new { profileName = profile.ProfileName }, profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding a new profile.");
                return StatusCode(500, $"An error occurred while adding the profile.{ex.Message}");
            }
        }

        [HttpPut("{profileName}")]
        [Authorize(Policy = "CanEditProfile")]
        public async Task<IActionResult> UpdateProfile(string profileName, [FromBody] ProfileParameter profileParameter)
        {
            try
            {
                if (profileParameter == null || string.IsNullOrWhiteSpace(profileParameter.ProfileName))
                    return BadRequest("Invalid profile data.");

               
                if (!profileName.Equals(profileParameter.ProfileName, StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Profile name in path and body must match.");

                var existingProfile = await _profileRepository.GetByProfileName(profileName);
                if (existingProfile == null)
                    return NotFound($"Profile '{profileName}' not found.");

                Profile profile = new Profile();
                _mapper.Map(profileParameter, profile,opts => {
                    profile.Id = existingProfile.Id;
                });
               
                await _profileRepository.Update(profile);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating the profile '{ProfileName}'.", profileName);
                return StatusCode(500, "An error occurred while updating the profile.");
            }
        }

        [HttpDelete("{profileName}")]
        [Authorize(Policy = "CanDeleteProfile")]
        public async Task<IActionResult> DeleteProfile(string profileName)
        {
            try
            {
                var existingProfile = await _profileRepository.GetByProfileName(profileName);
                if (existingProfile == null)
                    return Ok($"Profile '{profileName}' not found.");

                await _profileRepository.DeleteByProfileName(existingProfile.ProfileName);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting the profile '{ProfileName}'.", profileName);
                return StatusCode(500, "An error occurred while deleting the profile.");
            }
        }

        [HttpGet("{profileName}/validate")]
        [Authorize]
        public async Task<IActionResult> ValidateProfile(string profileName, [FromQuery] string operation)
        {
            var profile = await _profileRepository.GetByProfileName(profileName);
            if (profile == null)
                return Forbid();

            var profileParameter = _mapper.Map<ProfileParameter>(profile);

            bool hasPermission = false;

            var operationValue = profileParameter.Parameters.FirstOrDefault(kvp => kvp.Key.Equals(operation, StringComparison.OrdinalIgnoreCase)).Value;
            hasPermission = operationValue == "true";

            if (!hasPermission)
                 return StatusCode(403, $"Operation '{operation}' is not allowed.");

            return Ok(new { Profile = profileName, Operation = operation, Allowed = true });
        }
    }
}
