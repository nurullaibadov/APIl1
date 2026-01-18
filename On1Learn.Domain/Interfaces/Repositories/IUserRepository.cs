using On1Learn.Domain.Entities;
using On1Learn.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On1Learn.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailVerificationTokenAsync(string token);
        Task<User?> GetByPasswordResetTokenAsync(string token);
        Task<bool> IsEmailExistsAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
        Task<IEnumerable<User>> GetUnverifiedUsersAsync();
    }
}
