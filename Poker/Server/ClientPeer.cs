using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientPeer
    {
        #region init
        public Socket clientSocket { get; set; }
        public string Username { get; set; }
        public int Id { get; set; }
        private NetMsg msg;
        public ClientPeer()
        {
            msg = new NetMsg();
            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.UserToken = this;
            ReceiveArgs.SetBuffer(new byte[2048], 0, 2048);
        }
        #endregion

        #region receive

        //socket arguments
        public SocketAsyncEventArgs ReceiveArgs { get; set; }

        /// <summary>
        /// msg queue
        /// </summary>
        private List<byte> cache = new List<byte>();
        private bool isProcessingReceive = false;
        /// <summary>
        /// delegate to distribute msg
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        public delegate void ReceiveCompleted(ClientPeer client, NetMsg msg);
        public ReceiveCompleted receiveCompleted;


        public void ProcessReceive(byte[] packet)
        {
            //add packet to queue
            cache.AddRange(packet);
            if (isProcessingReceive == false)
            {
                ProcessData();
            }
        }

        private void ProcessData()
        {
            isProcessingReceive = true;
            //decode packet
            byte[] packet = EncodeTool.DecodePacket(ref cache);
            if (packet == null)
            {
                isProcessingReceive = false;
                return;
            }
            NetMsg msg = EncodeTool.DecodeMsg(packet);

            if (receiveCompleted != null)
            {
                //delegate
                receiveCompleted(this, msg);
            }
            //loop check until queue is empty
            ProcessData();
        }
        #endregion

        #region send

        public void SendMsg(int opCode, int subCode, object value)
        {
            msg.Change(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet =  EncodeTool.EncodePacket(data);
            SendMsg(packet);
        }

        private void SendMsg(byte[] packet)
        {
            try
            {
                clientSocket.Send(packet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw;
            }
        }

        #endregion

        #region disconnect
        public void Disconnect()
        {
            cache.Clear();
            isProcessingReceive = false;
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            clientSocket = null;
        }
        #endregion

    }
}
