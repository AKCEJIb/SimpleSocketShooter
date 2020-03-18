using System;

namespace Game.Networking
{
    public interface IAsyncSocket : IDisposable
    {
        event EventHandler<TcpCompletedEventArgs> ReadCompleted;
        event EventHandler<TcpCompletedEventArgs> SendCompleted;

        void ReadAsync(byte[] buffer, int offset, int size);
        void SendAsync(byte[] buffer, int offset, int size);
        void SendAsync(byte[] buffer);
        void Close();
        void AbortiveClose();
    }
}