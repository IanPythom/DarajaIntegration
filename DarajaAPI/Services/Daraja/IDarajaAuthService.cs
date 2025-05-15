namespace DarajaAPI.Services.Daraja
{
    public interface IDarajaAuthService
    {
        Task<string> GetAccessTokenAsync();
    }
}