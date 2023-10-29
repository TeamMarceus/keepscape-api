using keepscape_api.Models.Primitives;

namespace keepscape_api.Repositories.Generics
{
    public interface IProfileRepository<T> where T : IProfile
    {
        Task<T?> GetProfileByUserId(Guid userId);
        Task<int> GetTotalProfileCount();
    }
}
