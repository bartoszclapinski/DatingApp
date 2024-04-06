﻿using API.DTOs;
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
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uow;

    public MessagesController(IMapper mapper, IUnitOfWork uow)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage([FromBody] CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();
        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot send messages to yourself");
        
        var sender = await _uow.UserRepository.GetUserByUserNameAsync(username);
        var recipient = await _uow.UserRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
        
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
        
        _uow.MessageRepository.AddMessage(message);
        
        if (await _uow.Complete()) return Ok(_mapper.Map<MessageDto>(message));
        
        return BadRequest("Failed to send message");
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.UserName = User.GetUsername();
        
        var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);
        
        Response.AddPaginationsHeader(
            new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));
        
        return messages;
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(Guid id)
    {
        var username = User.GetUsername();
        var message = await _uow.MessageRepository.GetMessage(id);
        
        if (message.SenderUsername != username && message.RecipientUsername != username)
            return Unauthorized();
        
        if (message.SenderUsername == username) 
            message.SenderDeleted = true;
        if (message.RecipientUsername == username) 
            message.RecipientDeleted = true;
        
        if (message.SenderDeleted && message.RecipientDeleted) 
            _uow.MessageRepository.DeleteMessage(message);
        
        if (await _uow.Complete()) return Ok();
        
        return BadRequest("Problem deleting the message");
    } 
}