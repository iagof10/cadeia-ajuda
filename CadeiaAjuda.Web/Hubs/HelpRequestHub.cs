using Microsoft.AspNetCore.SignalR;

namespace CadeiaAjuda.Web.Hubs;

public class HelpRequestHub : Hub
{
    public async Task JoinTenant(string tenantId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, tenantId);
    }
}
