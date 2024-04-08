using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PhotoRepository : IPhotoRepository
{
    private readonly AppDbContext _context;

    public PhotoRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        return await _context.Photos.IgnoreQueryFilters()
                        .Where(p => p.IsApproved == false)
                        .Select(u => new PhotoForApprovalDto
                        {
                                        Id = u.Id,
                                        Username = u.AppUser.UserName,
                                        Url = u.Url,
                                        IsApproved = u.IsApproved
                        }).ToListAsync();

    }

    public async Task<Photo> GetPhotoById(Guid id)
    {
        return await _context.Photos.IgnoreQueryFilters().SingleOrDefaultAsync(p => p.Id == id);
    }

    public void RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
    }
}