namespace API.Entities;

public class Photo
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
    public string PublicId { get; set; }
}