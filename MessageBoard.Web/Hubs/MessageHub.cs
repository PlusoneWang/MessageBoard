namespace MessageBoard.Web.Hubs
{
    using Microsoft.AspNet.SignalR;

    public class MessageHub : Hub
    {
        public void SendMessage()
        {
            Clients.All.updateMessage();
        }
    }
}