using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIN_Client.Services
{
    public class ChatService
    {
        private readonly HubConnection _connection;

        public string roomNameLocal;

        //Identity setting
        public event Action IdentitySet;

        public async void SetIdentity(string userName)
        {
            await _connection.SendAsync("SetIdentity", userName);
        }

        // List rooms
        public event Action<String> RoomListReceived;

        public async Task ListRooms()
        {
            await _connection.SendAsync("GetRooms");
        }

        // Room management, supposed to lock up interface until complete
        public void JoinRoom(string roomName)
        {
            _connection.SendAsync("JoinRoom", roomName);
            roomNameLocal = roomName;
        }

        public void LeaveRoom()
        {
            _connection.SendAsync("LeaveRoom", roomNameLocal);
            roomNameLocal = string.Empty;
        }

        // Sending messages to room
        public event Action<string> ChatMessageReceived;

        public async Task SendChatMessage(string message)
        {
            await _connection.SendAsync("SendChatMessage", roomNameLocal, message);
        }

        public async Task SendPrivMessage(string connectionId, string message)
        {
            await _connection.SendAsync("SendPrivMessage", connectionId, message);
        }

        public ChatService(HubConnection connection)
        {
            _connection = connection;

            _connection.On<string>("ReceiveChatMessage", (message) => ChatMessageReceived?.Invoke(message));

            _connection.On<string>("ReceiveRooms", (groups) => RoomListReceived?.Invoke(groups));

            _connection.On("IdentityRead", () => IdentitySet?.Invoke());
        }

        public async Task Connect()
        {
            await _connection.StartAsync();
        }
    }
}
