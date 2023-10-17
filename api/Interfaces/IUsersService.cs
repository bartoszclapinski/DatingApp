using API.Entities;

namespace API.Interfaces;

public interface IUsersService
{
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser> GetUserAsync(Guid id);
}