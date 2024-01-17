namespace API.SignalR;

public class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUsers = new ();
    
    public Task UserConnected(string username, string connectionId)
    {
        lock (OnlineUsers)
        {
            if (OnlineUsers.TryGetValue(username, out var user))
            {
                user.Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(username, new List<string> {connectionId});
            }
        }

        return Task.CompletedTask;
    }
    
    public Task UserDisconnected(string username, string connectionId)
    {
        lock (OnlineUsers)
        {
            if (!OnlineUsers.TryGetValue(username, out var user)) return Task.CompletedTask;
            user.Remove(connectionId);
            if (user.Count == 0)
            {
                OnlineUsers.Remove(username);
            }
        }

        return Task.CompletedTask;
    }
    
    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
        }

        return Task.FromResult(onlineUsers);
    }
}