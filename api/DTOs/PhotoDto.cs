namespace API.DTOs;

public class PhotoDto
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
}