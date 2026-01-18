using On1Learn.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On1Learn.Domain.Interfaces.Repositories
{
    public interface IContactMessageRepository : IGenericRepository<ContactMessage>
    {
        Task<IEnumerable<ContactMessage>> GetUnreadMessagesAsync();
        Task<IEnumerable<ContactMessage>> GetMessagesByUserIdAsync(int userId);
        Task<IEnumerable<ContactMessage>> GetMessagesByPropertyIdAsync(int propertyId);
        Task<IEnumerable<ContactMessage>> GetUnrepliedMessagesAsync();
        Task MarkAsReadAsync(int messageId);
        Task ReplyToMessageAsync(int messageId, string replyMessage, int repliedByUserId);
    }
}
