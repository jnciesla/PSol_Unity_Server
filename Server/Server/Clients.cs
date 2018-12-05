using System;
using System.Net.Sockets;

namespace Server
{
    class Clients
    {
        public int connectionID;
        public string ip;
        public TcpClient socket;
        public NetworkStream myStream;
        private byte[] readBuff;
        public ByteBuffer playerBuffer;

        public void Start()
        {
            socket.SendBufferSize = 4096;
            socket.ReceiveBufferSize = 4096;
            myStream = socket.GetStream();
            readBuff = new byte[4096];
            myStream.BeginRead(readBuff, 0, socket.ReceiveBufferSize, OnReceiveData, null);
        }

        private void OnReceiveData(IAsyncResult result)
        {
            try
            {
                var readbytes = myStream.EndRead(result);
                if (readbytes <= 0)
                {
                    //client is not connected to the server anymore
                    CloseSocket();
                    return;
                }
                var newBytes = new byte[readbytes];
                Buffer.BlockCopy(readBuff, 0, newBytes, 0, readbytes);
                ServerHandleData.HandleData(connectionID, newBytes);
                myStream.BeginRead(readBuff, 0, socket.ReceiveBufferSize, OnReceiveData, null);

            }
            catch (Exception)
            {

                CloseSocket();
            }
        }

        public void CloseSocket()
        {
            Log.Message("Connection from " + ip + " has been terminated");
            socket.Close();
            socket = null;
        }
    }
}
