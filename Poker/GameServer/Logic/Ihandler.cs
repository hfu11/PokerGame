﻿using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Logic
{
    public interface Ihandler
    {
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="client"></param>
        void Disconnect(ClientPeer client);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="subCode"></param>
        /// <param name="value">传入参数</param>
        void Receive(ClientPeer client, int subCode, object value);
    }
}
