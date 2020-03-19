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
            Socket.EndConnect(ar);
            ConnectCompleted?.Invoke(this, new TcpCompletedEventArgs(ar.AsyncState));
        }
    }

    public class GameTcpClient : IAsyncSocket
    {
        public event EventHandler<TcpCompletedEventArgs> ReadCompleted;
        public event EventHandler<TcpCompletedEventArgs> SendCompleted;
        public event EventHandler<TcpCompletedEventArgs> ConnectCompleted;

        private GameTcpClientImpl Socket_;

        private GameTcpClientImpl Socket
        {
            get
            {
                if (Socket_ != null)
                    return Socket_;

                Socket_ = new GameTcpClientImpl();
                Socket_.ConnectCompleted += (s, e) => ConnectCompleted?.Invoke(s, e);
                Socket_.ReadCompleted += (s, e) => ReadCompleted?.Invoke(s, e);
                Socket_.SendCompleted += (s, e) => SendCompleted?.Invoke(s, e);

                return Socket_;
            }
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

        public void AbortiveClose()
        {
            Socket_.SetAbortive();
            Socket_.Dispose();

            Socket_ = null;
        }

        public void Close()
        {
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
    }
}
