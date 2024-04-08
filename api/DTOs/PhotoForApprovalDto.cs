namespace API.DTOs;

public class PhotoForApprovalDto
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public string Username { get; set; }
    public bool IsApproved { get; set; }
}