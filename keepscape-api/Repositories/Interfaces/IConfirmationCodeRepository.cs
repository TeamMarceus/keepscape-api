using keepscape_api.Models;

using keepscape_api.Repositories.Generics;
namespace keepscape_api.Repositories.Interfaces
{
    public interface IConfirmationCodeRepository : IBaseRepository<ConfirmationCode>
    {
        Task<ConfirmationCode?> GetLatestConfirmationCodeByUserGuid(Guid userId);
    }
}
