namespace Valid.Teste.API.Handler
{
    using Microsoft.AspNetCore.Authorization;

    public class ProfileOperationRequirement : IAuthorizationRequirement
    {
        public string Operation { get; }

        public ProfileOperationRequirement(string operation)
        {
            Operation = operation;
        }
    }

}
