using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataSeed
{
    public static async Task SeedData(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;
        
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
        
        foreach (var user in users)
        {
            user.Id = Guid.NewGuid();
            user.UserName = user.UserName.ToLower();
            context.Users.Add(user);    
        }

        await context.SaveChangesAsync();

    }
}