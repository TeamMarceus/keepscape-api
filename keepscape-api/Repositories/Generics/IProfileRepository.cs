using keepscape_api.Models.Primitives;

namespace keepscape_api.Repositories.Generics
{
    public interface IProfileRepository<T> where T : IProfile
    {
        Task<T?> GetProfileByUserGuid(Guid userId);
        Task<int> GetTotalProfileCount();
    }
}
