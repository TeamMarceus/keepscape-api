namespace keepscape_api.Services.ConfirmationCodes
{
    public interface IConfirmationCodeService
    {
        Task<bool> Send(string email);
        Task<bool> Verify(string email, string code);
    }
}
