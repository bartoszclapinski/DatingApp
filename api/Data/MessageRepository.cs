using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace API.Data;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public MessageRepository(AppDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(Guid id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context.Messages
                        .OrderByDescending(m => m.MessageSent)
                        .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.UserName),
            "Outbox" => query.Where(u => u.Sender.UserName == messageParams.UserName),
            _ => query.Where(u => u.Recipient.UserName == messageParams.UserName && u.DateRead == null)
        };
        
        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public Task<IEnumerable<MessageDto>> GetMessageThread(Guid currentUserId, Guid recipientId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}