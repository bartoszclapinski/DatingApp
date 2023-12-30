using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class MessagesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage([FromBody] CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();
        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot send messages to yourself");
        
        var sender = await _userRepository.GetUserByUserNameAsync(username);
        var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
        
        if (recipient == null) return NotFound();
        
        var message = new Message
        {
            Id = new Guid(),
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };
        
        _messageRepository.AddMessage(message);
        
        if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));
        
        return BadRequest("Failed to send message");
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.UserName = User.GetUsername();
        
        var messages = await _messageRepository.GetMessagesForUser(messageParams);
        
        Response.AddPaginationsHeader(
            new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));
        
        return messages;
    }
    
    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();
        return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(Guid id)
    {
        var username = User.GetUsername();
        var message = await _messageRepository.GetMessage(id);
        
        if (message.Sender.UserName != username && message.Recipient.UserName != username)
            return Unauthorized();
        
        if (message.Sender.UserName == username) message.SenderDeleted = true;
        if (message.Recipient.UserName == username) message.RecipientDeleted = true;
        
        if (message.SenderDeleted && message.RecipientDeleted) _messageRepository.DeleteMessage(message);
        
        if (await _messageRepository.SaveAllAsync()) return Ok();
        
        return BadRequest("Problem deleting the message");
    } 
}