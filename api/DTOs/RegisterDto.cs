using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required] public string UserName { get; set; }
    [Required] [StringLength(8, MinimumLength = 4)] public string Password { get; set; }
    [Required] public string KnownAs { get; set; }
    [Required] public string Gender { get; set; }
    [Required] public DateOnly? DateOfBirth { get; set; } // optional to make required, add ? after DateOnly
    [Required] public string City { get; set; }
    [Required] public string Country { get; set; }
    
}