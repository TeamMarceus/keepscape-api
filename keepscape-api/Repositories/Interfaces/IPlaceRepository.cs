﻿using keepscape_api.Models;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IPlaceRepository : IBaseRepository<Place>
    {
        Task<int> GetPlaceCountAsync();
    }
}
