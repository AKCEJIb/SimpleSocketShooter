using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Game.Networking
{
    internal class GameTcpClientImpl : GameTcpSocketImpl
    {
        public GameTcpClientImpl()
            : base(new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp)) { }

        public event EventHandler<TcpCompletedEventArgs> ConnectCompleted;

        public void ConnectAsync(IPEndPoint server)
        {
            Socket.BeginConnect(server, new AsyncCallback(TryConnect), server);
        }

        private void TryConnect(IAsyncResult ar)
        {
            try
            {
                Socket.EndConnect(ar);
                ConnectCompleted?.Invoke(this, new TcpCompletedEventArgs(ar.AsyncState));
            }
            catch (Exception ex)
            {
                var eventArgs = new TcpCompletedEventArgs(ex);
                eventArgs.Error = true;

                ConnectCompleted?.Invoke(this, eventArgs);
            }
        }
    }

    public class GameTcpClient : IAsyncSocket
    {
        public event EventHandler<TcpCompletedEventArgs> ReadCompleted;
        public event EventHandler<TcpCompletedEventArgs> SendCompleted;
        public event EventHandler<TcpCompletedEventArgs> ConnectCompleted;
        public event EventHandler<TcpCompletedEventArgs> ShutdownCompleted;

        private GameTcpClientImpl Socket_;

        private GameTcpClientImpl Socket
        {
            get
            {
                if (Socket_ != null)
                    return Socket_;

                Socket_ = new GameTcpClientImpl();
                Socket_.ConnectCompleted += Socket__ConnectCompleted;
                Socket_.ReadCompleted += Socket__ReadCompleted;
                Socket_.SendCompleted += Socket__SendCompleted;
                Socket_.ShutdownCompleted += Socket__ShutdownCompleted;

                return Socket_;
            }
        }

        private void Socket__ShutdownCompleted(object sender, TcpCompletedEventArgs e)
        {
            ShutdownCompleted?.Invoke(sender, e);
        }

        private void Socket__SendCompleted(object sender, TcpCompletedEventArgs e)
        {
            SendCompleted?.Invoke(sender, e);
        }

        private void Socket__ReadCompleted(object sender, TcpCompletedEventArgs e)
        {
            ReadCompleted?.Invoke(sender, e);
        }

        private void Socket__ConnectCompleted(object sender, TcpCompletedEventArgs e)
        {
            ConnectCompleted?.Invoke(sender, e);
        }

        private void EnsureOpen()
        {
            if (Socket_ == null)
                throw new InvalidOperationException("Socket is not open.");
        }

        public IPEndPoint LocalEndPoint
        { 
            get
            {
                EnsureOpen();
                return Socket.LocalEndPoint;
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                EnsureOpen();
                return Socket.RemoteEndPoint;
            }
        }
        public void ConnectAsync(IPEndPoint server)
        {
            Socket.ConnectAsync(server);
        }
        private void FreeEvents()
        {
            FreeEventsExceptShutdown();
            Socket_.ShutdownCompleted -= Socket__ShutdownCompleted;
        }
        private void FreeEventsExceptShutdown()
        {
            Socket_.ConnectCompleted -= Socket__ConnectCompleted;
            Socket_.ReadCompleted -= Socket__ReadCompleted;
            Socket_.SendCompleted -= Socket__SendCompleted;
        }
        public void AbortiveClose()
        {
            FreeEvents();

            Socket_.SetAbortive();
            Socket_.Dispose();

            Socket_ = null;
        }

        public void Close()
        {
            FreeEvents();

            Socket_.Dispose();
            Socket_ = null;
        }

        public void Dispose()
        {
            Close();
        }

        public void ReadAsync(byte[] buffer, int offset, int size)
        {
            Socket.ReadAsync(buffer, offset, size);
        }

        public void SendAsync(byte[] buffer, int offset, int size)
        {
            Socket.SendAsync(buffer, offset, size);
        }

        public void SendAsync(byte[] buffer)
        {
            Socket.SendAsync(buffer, 0, buffer.Length);
        }

        public void ShutdownAsync()
        {
            EnsureOpen();

            FreeEventsExceptShutdown();

            Socket.ShutdownAsync();
        }
    }
}
