using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIN_Server
{
    public class ChatHub : Hub
    {
        private static List<string> groups = new List<string> { "Piaskownica", "testowe" };
        private static Hashtable id2user = new();
        private static Hashtable user2id = new();

        public async void SetIdentity(string userName)
        {
            // TODO?
            //Controls.GetInstance().users.Add(Context.ConnectionId, "Test");

            if(!user2id.ContainsKey(userName))
            {
                id2user.Add(Context.ConnectionId, userName);
                user2id.Add(userName, Context.ConnectionId);
                await Clients.Caller.SendAsync("IdentityRead");
                await Clients.Caller.SendAsync("ReceiveChatMessage", "Pomyślnie zarejestrowano nazwę " + userName+ "!");
            } else
            {
                await Clients.Caller.SendAsync("ReceiveChatMessage", "Nazwa " + userName + " już istnieje!");
            }

        }

        public async Task GetRooms()
        {
            foreach (string abs in groups) {
                await Clients.Caller.SendAsync("ReceiveRooms", abs);
            }
        }

        public async Task SendChatMessage(string roomName, string message)
        {
            await Clients.Group(roomName).SendAsync("ReceiveChatMessage", id2user[Context.ConnectionId] + ": " + message);
        }

        public async Task SendPrivMessage(string userName, string message)
        {
            string connectionId = (string)user2id[userName];
            await Clients.Client(connectionId).SendAsync("ReceiveChatMessage", id2user[Context.ConnectionId] + " szepcze: " + message);
        }

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.OthersInGroup(roomName).SendAsync("ReceiveChatMessage", id2user[Context.ConnectionId] + " dołączył do pokoju.");
            await Clients.Caller.SendAsync("ReceiveChatMessage", "Dołączyłeś do pokoju " + roomName);
            if(!groups.Contains(roomName))
            {
                groups.Add(roomName);
            }
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.OthersInGroup(roomName).SendAsync("ReceiveChatMessage", id2user[Context.ConnectionId] + " opuścił pokój.");
            await Clients.Caller.SendAsync("ReceiveChatMessage", "Opuściłeś pokój " + roomName);
        }
    }
}
