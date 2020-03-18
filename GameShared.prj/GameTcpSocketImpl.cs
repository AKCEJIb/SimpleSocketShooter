using System;
using System.Net.Sockets;

namespace Game.Networking
{
    internal class GameTcpSocketImpl : IDisposable
    {
        public event EventHandler<TcpCompletedEventArgs> ReadCompleted;
        public event EventHandler<TcpCompletedEventArgs> SendCompleted;
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
            Socket.EndSend(ar);
            SendCompleted?.Invoke(this, new TcpCompletedEventArgs());
        }

        public void SetAbortive()
        {
            Socket.LingerState = new LingerOption(true, 0);
        }

        public void Dispose()
        {
            Socket.Close();
        }
    }
}
