using System;
using System.Net;

namespace Game.Networking
{
    public interface IAsyncSocket : IDisposable
    {
        event EventHandler<TcpCompletedEventArgs> ReadCompleted;
        event EventHandler<TcpCompletedEventArgs> SendCompleted;

        IPEndPoint LocalEndPoint { get; }
        IPEndPoint RemoteEndPoint { get; }
        void ReadAsync(byte[] buffer, int offset, int size);
        void SendAsync(byte[] buffer, int offset, int size);
        void SendAsync(byte[] buffer);
        void Close();
        void AbortiveClose();
    }
}