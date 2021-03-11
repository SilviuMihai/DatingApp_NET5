using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    //implementation for only one server
    public class PresenceTracker
    {
        private static readonly Dictionary<string,List<string>> OnlineUsers = 
        new Dictionary<string, List<string>>();

        public Task<bool> UserConnected(string username, string connectionId)
        {

            bool isOnline = false;

            //using lock, because Dictionary is not thread safe (meaning that is not capable to be runned by multiple users at the same time)
            lock(OnlineUsers)
            {
                //check if the respective user has a key
                if(OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                //if it doesn't have a key, create one
                else
                {
                    OnlineUsers.Add(username,new List<string>{connectionId});
                    isOnline =true;    
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username,string connectionId)
        {
            bool isOffline = false;
            lock(OnlineUsers)
            {
                //doesn't contain the username with the ID
                if(!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                //if it does contain, disconnect
                OnlineUsers[username].Remove(connectionId);

                if(OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline =true;
                }
            }

            return Task.FromResult(isOffline);
        }


        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineusers;

            lock(OnlineUsers)
            {
                onlineusers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineusers);
        }

        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock(OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }
            return Task.FromResult(connectionIds);
        }
    }
}