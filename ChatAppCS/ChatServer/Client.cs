using chatApp;
using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
     class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _PacketReader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();

            _PacketReader = new PacketReader(ClientSocket.GetStream());
            
            var opcode = _PacketReader.ReadByte();
            Username = _PacketReader.ReadMessage();


            Console.WriteLine($"[{DateTime.Now}]: Usuario conectado com o nome: {Username}");

            Task.Run(() => Process());


        }
        void Process()
        {
            while(true)
            {
                try
                {
                    var opcode = _PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _PacketReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Mensagem enviada! {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                            break;
                        default:

                            break;
                    }
                } catch(Exception)
                {
                    Console.WriteLine($"[{UID.ToString()}]: Desconectado!");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }

    }
}
