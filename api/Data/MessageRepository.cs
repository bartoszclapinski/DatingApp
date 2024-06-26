﻿using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

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
            "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.UserName && u.RecipientDeleted == false),
            "Outbox" => query.Where(u => u.Sender.UserName == messageParams.UserName && u.SenderDeleted == false),
            _ => query.Where(u => u.Recipient.UserName == messageParams.UserName && u.RecipientDeleted == false && u.DateRead == null)
        };
        
        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
    {
        var query = _context.Messages
                        .Where(m =>
                                        m.Recipient.UserName == currentUserName &&
                                        m.RecipientDeleted == false &&
                                        m.Sender.UserName == recipientUserName ||
                                        m.Recipient.UserName == recipientUserName &&
                                        m.SenderDeleted == false &&
                                        m.Sender.UserName == currentUserName)
                        .OrderBy(m => m.MessageSent)
                        .AsQueryable();
                        
        
        var unreadMessages = query
                        .Where(m => m.DateRead == null && m.Recipient.UserName == currentUserName)
                        .ToList();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
        }

        return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();

    }

    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await _context.Groups
                        .Include(c => c.Connections)
                        .FirstOrDefaultAsync(g => g.Name == groupName);
    }
    
    public async Task<Group> GetGroupForConnection(string connectionId)
    {
        return await _context.Groups
                        .Include(c => c.Connections)
                        .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                        .FirstOrDefaultAsync();
    }
}