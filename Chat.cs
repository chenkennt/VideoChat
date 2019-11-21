using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Linq;
using static AspNet.Security.OAuth.GitHub.GitHubAuthenticationConstants;

namespace Microsoft.Azure.SignalR.VideoChat
{
    [Authorize]
    public class Chat : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("userList", Users.Instance.GetAllUsers());
            var user = new User(
                Context.UserIdentifier,
                Context.User.Claims.FirstOrDefault(c => c.Type == Claims.Name)?.Value ?? Context.User.Identity.Name,
                Context.User.Claims.FirstOrDefault(c => c.Type == AuthController.AvatarUrlClaim)?.Value);
            Users.Instance.Add(user);
            await Clients.Others.SendAsync("online", user);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Users.Instance.Remove(Context.UserIdentifier);
            return Clients.Others.SendAsync("changeStatus", Context.UserIdentifier, "offline");
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

        public Task ClientHangup(string id)
        {
            return Clients.User(id).SendAsync("clientHangup", Context.UserIdentifier);
        }

        public Task SendMessage(string id, string message)
        {
            return Clients.User(id).SendAsync("newMessage", Context.UserIdentifier, message);
        }

        public Task ChangeStatus(string status)
        {
            Users.Instance.GetUser(Context.UserIdentifier).Status = status;
            return Clients.Others.SendAsync("changeStatus", Context.UserIdentifier, status);
        }
    }
}
