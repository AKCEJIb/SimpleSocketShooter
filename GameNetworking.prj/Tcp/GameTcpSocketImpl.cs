using System;
using System.Net;
using System.Net.Sockets;

namespace Game.Networking
{
    internal class GameTcpSocketImpl : IDisposable
    {
        public event EventHandler<TcpCompletedEventArgs> ReadCompleted;
        public event EventHandler<TcpCompletedEventArgs> SendCompleted;
        public event EventHandler<TcpCompletedEventArgs> ShutdownCompleted;
        protected Socket Socket { get; private set; }

        public GameTcpSocketImpl(Socket socket)
        {
            Socket = socket;
        }

        public void SendAsync(byte[] buffer, int offset, int size)
        {
            Socket.BeginSend(buffer,
                offset, 
                size,
                SocketFlags.None, 
                new AsyncCallback(TrySend),
                null);
        }

        public void ReadAsync(byte[] buffer, int offset, int size)
        {
            Socket.BeginReceive(buffer,
                offset,
                size,
                SocketFlags.None,
                new AsyncCallback(TryRead),
                null);
        }

        private void TryRead(IAsyncResult ar)
        {
            try
            {
                ReadCompleted?.Invoke(this, new TcpCompletedEventArgs(Socket.EndReceive(ar)));
            }
            catch(Exception ex) {
                var eventArgs = new TcpCompletedEventArgs(ex);
                eventArgs.Error = true;

                ReadCompleted?.Invoke(this, eventArgs);
            }
        }

        private void TrySend(IAsyncResult ar)
        {
            var size = Socket.EndSend(ar);
            SendCompleted?.Invoke(this, new TcpCompletedEventArgs(size));
        }

        public void SetAbortive()
        {
            Socket.LingerState = new LingerOption(true, 0);
        }

        public void Dispose()
        {
            Socket.Close();
        }

        public void ShutdownAsync()
        {
            Socket.BeginDisconnect(false, new AsyncCallback(TryShutdown), null);
        }

        private void TryShutdown(IAsyncResult ar)
        {
            try
            {
                Socket.EndDisconnect(ar);
                ShutdownCompleted?.Invoke(this, new TcpCompletedEventArgs());
            }
            catch (Exception ex)
            {
                var eventArgs = new TcpCompletedEventArgs(ex);
                eventArgs.Error = true;

                ShutdownCompleted?.Invoke(this, eventArgs);
            }
        }

        public IPEndPoint LocalEndPoint => (IPEndPoint)Socket.LocalEndPoint;

        public IPEndPoint RemoteEndPoint => (IPEndPoint)Socket.RemoteEndPoint;
    }
}
