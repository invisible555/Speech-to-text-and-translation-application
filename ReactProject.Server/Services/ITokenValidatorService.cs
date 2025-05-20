namespace ReactProject.Server.Services
{
    public interface ITokenValidatorService
    {
        Task<bool> IsAccessTokenValidAsync(string token);
    }
}
