using ChatServer.Net.IO;
using ChatServer;
using System.Net;
using System.Net.Sockets;

namespace chatApp
{
    class Program
    {
        // Estabelecendo conexão com o servidor
        static List<Client> _users;
        static TcpListener _listener;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
            _listener.Start();

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);
                /* Conexão */
                BroadcastConnection();
            }
        }
        // Enviando dados do Usuario para o servidor
        static void BroadcastConnection()
        {
            foreach (var client in _users)
            {
                foreach (var user in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(user.Username);
                    broadcastPacket.WriteMessage(user.UID.ToString());
                    client.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }
        // Enviando as mensagens para o servidor
        public static void BroadcastMessage(string message)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
                _users.Remove(disconnectUser);
            foreach (var user in _users)
            {
                var BroadcastPacket = new PacketBuilder();
                BroadcastPacket.WriteOpCode(10);
                BroadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(BroadcastPacket.GetPacketBytes());
            }

            BroadcastMessage($"[{disconnectUser.Username}] Disconnected!");
        }
    }
}