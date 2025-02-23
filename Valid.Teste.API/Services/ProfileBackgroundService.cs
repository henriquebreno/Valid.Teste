using Newtonsoft.Json;
using Valid.Teste.API.FakerConfig;
using Valid.Teste.Domain.Interfaces;

namespace Valid.Teste.API.Services
{
    public class ProfileBackgroundService : BackgroundService
    {
        private const string ADMIN_PROFILENAME = "admin";
        private readonly IProfileRepository _profileRepository;
        private readonly ProfileParameterGenerator _profileDataGenerator;

        public ProfileBackgroundService(IProfileRepository profileRepository, ProfileParameterGenerator profileDataGenerator)
        {
            _profileRepository = profileRepository;
            _profileDataGenerator = profileDataGenerator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var profiles = await _profileRepository.GetAll();

                foreach (var profile in profiles)
                {
                    if (profile.ProfileName.ToUpper() == ADMIN_PROFILENAME.ToUpper()) {                    
                        continue;
                    }
                    var newProfileData = _profileDataGenerator.GenerateProfile();
                    profile.Parameters = JsonConvert.SerializeObject(newProfileData);
                    await _profileRepository.Update(profile);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

}
