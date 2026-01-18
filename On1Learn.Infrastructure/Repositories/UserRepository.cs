using Microsoft.EntityFrameworkCore;
using On1Learn.Domain.Entities;
using On1Learn.Domain.Enums;
using On1Learn.Domain.Interfaces.Repositories;
using On1Learn.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On1Learn.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>,IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByEmailVerificationTokenAsync(string token)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
        }

        public async Task<User?> GetByPasswordResetTokenAsync(string token)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u =>
                    u.PasswordResetToken == token &&
                    u.PasswordResetTokenExpiry > DateTime.UtcNow);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _dbSet
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _dbSet
                .Where(u => u.Role == role)
                .OrderBy(u => u.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUnverifiedUsersAsync()
        {
            return await _dbSet
                .Where(u => !u.IsEmailVerified)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }
    }
}
