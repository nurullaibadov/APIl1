using Microsoft.EntityFrameworkCore;
using On1Learn.Domain.Entities;
using On1Learn.Domain.Interfaces.Repositories;
using On1Learn.Infrastructure.Data;

namespace On1Learn.Infrastructure.Repositories
{
    public class PropertyImageRepository
        : GenericRepository<PropertyImage>, IPropertyImageRepository
    {
        public PropertyImageRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<PropertyImage>> GetImagesByPropertyIdAsync(int propertyId)
        {
            return await _dbSet
                .Where(i => i.PropertyId == propertyId)
                .OrderBy(i => i.DisplayOrder)
                .ToListAsync();
        }

        public async Task<PropertyImage?> GetCoverImageByPropertyIdAsync(int propertyId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(i =>
                    i.PropertyId == propertyId && i.IsCover);
        }

        public async Task DeleteImagesByPropertyIdAsync(int propertyId)
        {
            var images = await _dbSet
                .Where(i => i.PropertyId == propertyId)
                .ToListAsync();

            DeleteRange(images);
        }
    }
}
