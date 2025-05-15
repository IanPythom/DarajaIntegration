namespace DarajaAPI.Services.Daraja
{
    public interface IAccountService
    {
        Task<bool> ValidateAccountAsync(string accountNumber);
    }
}
