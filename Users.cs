using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Azure.SignalR.VideoChat
{
    public class User
    {
        public string Id { get; private set; }

        public string Name { get; private set; }

        public string Avatar { get; private set; }


        public string Status { get; set; }

        public User(string id, string name, string avatar)
        {
            Id = id;
            Name = name;
            Avatar = avatar;
            Status = "online";
        }
    }

    public class Users
    {
        ConcurrentDictionary<string, User> users = new ConcurrentDictionary<string, User>();

        public static Users Instance { get; } = new Users();

        public void Add(User user)
        {
            users[user.Id] = user;
        }

        public void Remove(string id)
        {
            users.Remove(id, out _);
        }

        public User GetUser(string id)
        {
            return users[id];
        }

        public IEnumerable<User> GetAllUsers()
        {
            return users.Values;
        }
    }
}
