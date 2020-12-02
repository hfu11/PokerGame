using GameServer.Database;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerPeer server = new ServerPeer();
            server.SetApplication(new NetMsgCenter());
            server.StartServer("127.0.0.1", 6666, 100);
            DatabaseManager.StartConnect();
            Console.ReadKey();
        }
    }
}
