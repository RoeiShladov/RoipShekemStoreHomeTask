using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RoipBackend.Hubs;

namespace RoipBackend.Pages.Admin
{
    public class ConnectionStatusModel : PageModel
    {
        private readonly IHubContext<UserConnectionHub> _hubContext;

        public ConnectionStatusModel(IHubContext<UserConnectionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void OnGet()
        {
            // Example: Broadcast a message to all connected clients using SignalR
            //_hubContext.Clients.All.SendAsync("ReceiveMessage", "Server", "ConnectionStatus page accessed.");
        }
    }


}