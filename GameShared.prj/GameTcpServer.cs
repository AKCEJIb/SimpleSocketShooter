﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Game.Networking
{
    /// <summary>
    /// Класс представлющий соединение сервера с клиентом.
    /// </summary>
    public sealed class GameTcpServerConnection : IAsyncSocket
    {
        private readonly GameTcpSocketImpl Socket;

        public event EventHandler<TcpCompletedEventArgs> ReadCompleted;
        public event EventHandler<TcpCompletedEventArgs> SendCompleted;

        internal GameTcpServerConnection(Socket socket)
        {
            Socket = new GameTcpSocketImpl(socket);
            Socket.ReadCompleted += (s, e) => ReadCompleted.Invoke(s, e);
            Socket.SendCompleted += (s, e) => SendCompleted.Invoke(s, e);
        }

        public void AbortiveClose()
        {
            Socket.SetAbortive();
            Socket.Dispose();
        }

        public void Close()
        {
            Socket.Dispose();
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

    internal sealed class GameTcpServerImpl : IDisposable
    {
        public event EventHandler<TcpCompletedEventArgs> ClientAccepted;
        private Socket Socket { get; set; }

        public GameTcpServerImpl()
        {
            Socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
        }

        public void Bind(IPEndPoint bind, int backlog)
        {
            Socket.Bind(bind);
            Socket.Listen(backlog);
        }

        public void ClientAcceptAsync()
        {
            Socket.BeginAccept(new AsyncCallback(TryAccept), null);
        }

        private void TryAccept(IAsyncResult ar)
        {
            ClientAccepted?.Invoke(this, 
                new TcpCompletedEventArgs(
                    new GameTcpServerConnection(Socket.EndAccept(ar))));
        }

        public void Dispose()
        {
            Socket.Close();
        }
    }

    public class GameTcpServer : IDisposable
    {
        public event EventHandler<TcpCompletedEventArgs> AcceptCompleted;
        private GameTcpServerImpl Socket_;

        private GameTcpServerImpl Socket
        {
            get
            {
                if (Socket_ != null)
                    return Socket_;

                // Create a new socket connection and subscribe to its events
                Socket_ = new GameTcpServerImpl();
                Socket_.ClientAccepted += (s, e) => AcceptCompleted(s, e);

                return Socket_;
            }
        }

        public void Bind(IPEndPoint bindTo, int backlog)
        {
            Socket.Bind(bindTo, backlog);
        }

        public void Bind(int port, int backlog)
        {
            Bind(new IPEndPoint(IPAddress.Any, port), backlog);
        }
        public void Bind(IPAddress address, int port, int backlog)
        {
            Bind(new IPEndPoint(address, port), backlog);
        }

        public void ClientAcceptAsync()
        {
            Socket.ClientAcceptAsync();
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (Socket_ == null)
                return;

            Socket_.Dispose();
            Socket_ = null;
        }
    }
}
