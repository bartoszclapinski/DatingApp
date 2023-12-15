using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(Guid id);
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);  
    Task<IEnumerable<MessageDto>> GetMessageThread(Guid currentUserId, Guid recipientId);
    Task<bool> SaveAllAsync();
}