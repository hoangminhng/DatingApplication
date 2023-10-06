namespace API;

public class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUser = new Dictionary<string, List<string>>();

    public Task<bool> UserConnected(string username, string connectionId)
    {
        bool UserIsOnline = false;
        lock (OnlineUser)
        {
            if (OnlineUser.ContainsKey(username))
            {
                OnlineUser[username].Add(connectionId);
            }
            else
            {
                OnlineUser.Add(username, new List<string> { connectionId });
                UserIsOnline = true;
            }
        }
        return Task.FromResult(UserIsOnline);
    }

    public Task<bool> UserDisconnected(string username, string connnectionId)
    {
        bool UserIsOffine = false;
        lock (OnlineUser)
        {
            if (!OnlineUser.ContainsKey(username))
            {
                return Task.FromResult(UserIsOffine);
            }
            OnlineUser[username].Remove(connnectionId);
            if (OnlineUser[username].Count == 0)
            {
                OnlineUser.Remove(username);
                UserIsOffine = true;
            }
        }
        return Task.FromResult(UserIsOffine);
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUser;
        lock (OnlineUser)
        {
            onlineUser = OnlineUser.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
        }
        return Task.FromResult(onlineUser);
    }

    public static Task<List<string>> GetConnectionsForUser(string username)
    {
        List<string> connectionId;
        lock (OnlineUser)
        {
            connectionId = OnlineUser.GetValueOrDefault(username);
        }

        return Task.FromResult(connectionId);
    }
}
