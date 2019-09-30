using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

namespace Microsoft.Azure.SignalR.VideoChat
{
    public class Chat : Hub
    {
        static ConcurrentDictionary<string, string> users = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("userList", users.Select(p => new { Id = p.Key, Name = p.Value }));
            users[Context.UserIdentifier] = Context.User.Identity.Name;
            await Clients.Others.SendAsync("connected", Context.UserIdentifier, Context.User.Identity.Name);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            users.TryRemove(Context.UserIdentifier, out _);
            return Clients.Others.SendAsync("disconnected", Context.User.Identity.Name);
        }

        public Task ClientRequest(string id, object desc)
        {
            return Clients.User(id).SendAsync("clientRequest", Context.UserIdentifier, desc);
        }

        public Task ClientAnswer(string id, object desc)
        {
            return Clients.User(id).SendAsync("clientAnswer", Context.UserIdentifier, desc);
        }

        public Task ClientCandidate(string id, object candidate)
        {
            return Clients.User(id).SendAsync("clientCandidate", Context.UserIdentifier, candidate);
        }
    }
}
