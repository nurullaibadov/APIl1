using On1Learn.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On1Learn.Domain.Interfaces.Repositories
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(int userId);

       
        Task<bool> IsFavoriteAsync(int userId, int propertyId);

      
        Task<IEnumerable<Property>> GetFavoritePropertiesByUserIdAsync(int userId);

       
        Task<bool> ToggleFavoriteAsync(int userId, int propertyId);
    }
}
