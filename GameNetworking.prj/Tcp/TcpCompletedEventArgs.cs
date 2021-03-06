﻿namespace Game.Networking
{
    public class TcpCompletedEventArgs
    {
        public object Data { get; set; }
        public bool Error { get; set; }

        public TcpCompletedEventArgs()
        {
            Data = null;
        }
        public TcpCompletedEventArgs(object data)
        {
            Data = data;
        }
    }
}