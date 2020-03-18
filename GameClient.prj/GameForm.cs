using Game.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.Client
{
    public partial class GameForm : Form
    {
        private World LocalWorld { get; set; }
        private PlayerSP LocalPlayer { get; set; }

        private GameTcpClient ClientSocket;
        public GameForm()
        {
            InitializeComponent();
            ClientSocket = new GameTcpClient();
            ClientSocket.ConnectCompleted += ClientSocket_ConnectCompleted;
            ClientSocket.SendCompleted += ClientSocket_SendCompleted;
            ClientSocket.ReadCompleted += ClientSocket_ReadCompleted;
        }

        private void ClientSocket_ReadCompleted(object sender, TcpCompletedEventArgs e)
        {
            if (!e.Error)
            {
                Console.WriteLine($"Read event, bytes: {(int)e.Data}");

                var buffer = new byte[1024];
                ClientSocket.ReadAsync(buffer, 0, buffer.Length);
            }
            else
            {
                ClientSocket.AbortiveClose();
                Console.WriteLine("Server was closed!");
            }
        }

        private void ClientSocket_SendCompleted(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine("Send event");
        }

        private void ClientSocket_ConnectCompleted(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine("Connected :3");
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            ClientSocket.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 23333));
            var buffer = new byte[1024];
            ClientSocket.ReadAsync(buffer, 0, buffer.Length);
        }
    }
}
