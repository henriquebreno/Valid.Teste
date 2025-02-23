namespace Valid.Teste.API.Handler
{
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using global::AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Valid.Teste.API.Models;
    using Valid.Teste.Domain.Entities;
    using Valid.Teste.Domain.Interfaces;

    public class ProfilePermissionHandler : AuthorizationHandler<ProfileOperationRequirement>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private const string ADMIN_PROFILENAME = "admin";


        public ProfilePermissionHandler(IProfileRepository profileRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _profileRepository = profileRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileOperationRequirement requirement)
        {
            var routeValues = _httpContextAccessor.HttpContext?.Request.RouteValues;
            if (routeValues == null || !routeValues.ContainsKey("profileName"))
            {
                context.Fail();
                return;
            }

            var profileName = routeValues["profileName"]?.ToString();
            if (string.IsNullOrEmpty(profileName))
            {
                context.Fail();
                return;
            }

            if (profileName.ToUpper() == ADMIN_PROFILENAME.ToUpper()) {
                await DenyAccessAsync(context, $"Cannot update/delete {ADMIN_PROFILENAME} user.");
                return;
            }

            var userClaims = _httpContextAccessor.HttpContext?.User.Claims;
            var loggedInProfileName = userClaims?.FirstOrDefault(c => c.Type == "ProfileName")?.Value;
            var loggedInProfile = await _profileRepository.GetByProfileName(loggedInProfileName);
            if (loggedInProfile == null)
            {
                context.Fail();
                return;
            }

            var profileParameter = _mapper.Map<ProfileParameter>(loggedInProfile);
            bool hasPermission = false;
            
            var operationValue = profileParameter.Parameters.FirstOrDefault(kvp => kvp.Key.Equals(requirement.Operation, StringComparison.OrdinalIgnoreCase)).Value;
            hasPermission = operationValue == "true";
            
            if (!hasPermission)
            {
                await DenyAccessAsync(context, $"Operation '{requirement.Operation}' is not allowed.");
                return;
            }

            context.Succeed(requirement);
        }

        private async Task DenyAccessAsync(AuthorizationHandlerContext context, string message)
        {
            context.Fail();

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync($"{{\"error\": \"{message}\"}}");
            }
        }

    }
}
