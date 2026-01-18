using On1Learn.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On1Learn.Domain.Interfaces.Repositories
{
    public interface IPropertyImageRepository : IGenericRepository<PropertyImage>
    {
        Task<IEnumerable<PropertyImage>> GetImagesByPropertyIdAsync(int propertyId);
        Task<PropertyImage?> GetCoverImageByPropertyIdAsync(int propertyId);
        Task DeleteImagesByPropertyIdAsync(int propertyId);
    }
}
