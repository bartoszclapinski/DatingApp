namespace API.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderUsername { get; set; } = null!;
    public AppUser Sender { get; set; } = null!;
    public Guid RecipientId { get; set; }
    public string RecipientUsername { get; set; } = null!;
    public AppUser Recipient { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; } = DateTime.UtcNow;
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }
}