using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API;

[Authorize]
public class PresenceHub : Hub
{
    private readonly PresenceTracker presenceTracker;

    public PresenceHub(PresenceTracker presenceTracker)
    {
        this.presenceTracker = presenceTracker;
    }

    public override async Task OnConnectedAsync()
    {
        var UserIsOnline = await presenceTracker.UserConnected(Context.User.getUserName(), Context.ConnectionId);
        if (UserIsOnline)
        {
            await Clients.Others.SendAsync("UserIsOnline", Context.User.getUserName());
        }

        var currentUser = await presenceTracker.GetOnlineUsers();

        await Clients.All.SendAsync("GetOnlineUsers", currentUser);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var UserIsOffine = await presenceTracker.UserDisconnected(Context.User.getUserName(), Context.ConnectionId);

        if (UserIsOffine)
        {
            await Clients.Others.SendAsync("UserIsOffine", Context.User.getUserName());
        }
        await base.OnDisconnectedAsync(exception);
    }
}
