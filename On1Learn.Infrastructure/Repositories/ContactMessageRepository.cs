using Microsoft.EntityFrameworkCore;
using On1Learn.Domain.Entities;
using On1Learn.Domain.Interfaces.Repositories;
using On1Learn.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On1Learn.Infrastructure.Repositories
{
    public class ContactMessageRepository : GenericRepository<ContactMessage>, IContactMessageRepository
    {
        public ContactMessageRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<ContactMessage>> GetUnreadMessagesAsync()
        {
            return await _dbSet
                .Include(m => m.User)
                .Include(m => m.Property)
                .Where(m => !m.IsRead)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContactMessage>> GetMessagesByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(m => m.Property)
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContactMessage>> GetMessagesByPropertyIdAsync(int propertyId)
        {
            return await _dbSet
                .Include(m => m.User)
                .Where(m => m.PropertyId == propertyId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContactMessage>> GetUnrepliedMessagesAsync()
        {
            return await _dbSet
                .Include(m => m.User)
                .Include(m => m.Property)
                .Where(m => !m.IsRepliead)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int messageId)
        {
            var message = await _dbSet.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
            }
        }

        public async Task ReplyToMessageAsync(int messageId, string replyMessage, int repliedByUserId)
        {
            var message = await _dbSet.FindAsync(messageId);
            if (message != null)
            {
                message.IsRepliead = true;
                message.ReplyMessage = replyMessage;
                message.RepliedAt = DateTime.UtcNow;
                message.RepliedByUserId = repliedByUserId;
            }
        }
    }
}
