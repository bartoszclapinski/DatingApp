namespace API.DTOs;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderUsername { get; set; } = null!;
    public string SenderPhotoUrl { get; set; } = null!;
    public Guid RecipientId { get; set; }
    public string RecipientUsername { get; set; } = null!;
    public string RecipientPhotoUrl { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; }
}
    