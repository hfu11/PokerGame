using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ServerPeer
    {
        private Socket serverSocket;
        private Semaphore semaphore;
        private ClientPeerPool clientPeerPool;

        private IApplication application;
        public void SetApplication(IApplication application)
        {
            this.application = application;
        }




        public void StartServer(string ip, int port, int maxClient)
        {
            try
            {
                clientPeerPool = new ClientPeerPool(maxClient);
                semaphore = new Semaphore(maxClient, maxClient);
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //object pool
                for (int i = 0; i < maxClient; i++)
                {
                    ClientPeer temp = new ClientPeer();
                    temp.receiveCompleted = ReceiveProcessCompleted;//call back
                    temp.ReceiveArgs.Completed += ReceiveArgs_Completed;
                    clientPeerPool.Enqueue(temp);
                }

                serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                serverSocket.Listen(maxClient);
                Console.WriteLine("Server Start");
                StartAccept(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        #region Accept Connections from ClientPeer
        private void StartAccept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += E_Completed;
            }
            //result being true means accepting connections. E_Completed is called after connection
            //result being false means connection success. E_Completed is called after connection
            bool result = serverSocket.AcceptAsync(e);
            if (result == false)
            {
                ProcessAccept(e);
            }

        }

        private void E_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            semaphore.WaitOne();
            ClientPeer client = clientPeerPool.Dequeue();
            client.clientSocket = e.AcceptSocket;
            Console.WriteLine(client.clientSocket.RemoteEndPoint + "Client Connection Success");

            StartReceive(client);

            e.AcceptSocket = null;
            StartAccept(e);
        }
        #endregion

        #region Receive from client

        private void StartReceive(ClientPeer client)
        {
            try
            {
                bool result = client.clientSocket.ReceiveAsync(client.ReceiveArgs);
                if (result == false)
                {
                    ProcessReceive(client.ReceiveArgs);
                }
            }
            
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ReceiveArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceive(e);
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            ClientPeer client = e.UserToken as ClientPeer;
            if(client.ReceiveArgs.SocketError == SocketError.Success && client.ReceiveArgs.BytesTransferred > 0)
            {
                //receive success
                byte[] packet = new byte[client.ReceiveArgs.BytesTransferred];
                Buffer.BlockCopy(client.ReceiveArgs.Buffer, 0, packet, 0, client.ReceiveArgs.BytesTransferred);

                //client peer process the packet
                client.ProcessReceive(packet);
                StartReceive(client);
            }
            else
            {
                if(client.ReceiveArgs.BytesTransferred == 0)
                {
                    //disconnect
                    if(client.ReceiveArgs.SocketError == SocketError.Success)
                    {
                        //client disconnect actively
                        Disconnect(client, "disconnect actively");
                    }
                    else
                    {
                        //passively disconnect
                        Disconnect(client, "disconnect passively due to network error");
                    }
                }
            }
        }
        #endregion

        private void Disconnect(ClientPeer client, string reason)
        {
            try
            {
                if(client == null)
                {
                    throw new Exception("client is null");
                }
                Console.WriteLine(client.clientSocket.RemoteEndPoint + "client " + reason);

                application.Disconnect(client);

                client.Disconnect();

                clientPeerPool.Enqueue(client);
                semaphore.Release();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ReceiveProcessCompleted(ClientPeer client, NetMsg msg)
        {
            application.Receive(client,msg);
        }
    }
}
