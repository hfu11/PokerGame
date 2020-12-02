using GameServer.Logic;
using Protocol.Code;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class NetMsgCenter : IApplication
    {
        private AccountHandler accountHandler = new AccountHandler();
        private MatchHandler matchHandler = new MatchHandler();
        private FightHandler fightHandler = new FightHandler();

        public NetMsgCenter()
        {
            matchHandler.startFight += fightHandler.StartFight;
        }

        public void Disconnect(ClientPeer client)
        {
            fightHandler.Disconnect(client);
            matchHandler.Disconnect(client);
            accountHandler.Disconnect(client);
        }

        public void Receive(ClientPeer client, NetMsg msg)
        {
            switch (msg.opCode)
            {
                case OpCode.Account:
                    accountHandler.Receive(client, msg.subCode, msg.value);
                    break;
                case OpCode.Match:
                    matchHandler.Receive(client, msg.subCode, msg.value);
                    break;
                case OpCode.Fight:
                    fightHandler.Receive(client, msg.subCode, msg.value);
                    break;
                default:
                    break;
            }
        }
    }
}
