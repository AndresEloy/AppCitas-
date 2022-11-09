using System;

public class MessageController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public MessageController(IUserRepository userRepository, IMessageRepository messageRepository)
    {
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _mapper = _mapper;
    }

    [HttpPost]

    public async Task<ActionResult<MessageDto>> CreateMessage(MessageCreateDto messageCreateDto)
    {
        var username = Username.GetUsername();

        if (username.ToLower().Equals(messageCreateDto.RecipientUsername.ToLowe()))
            return BadRequest("You cannot send message yo yourself");

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(messageCreateDto.Recipient)

        if (recipient == null) return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = messageCreateDto.Content
        };

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
            return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send the message");


    }
}
