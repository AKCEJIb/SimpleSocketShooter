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
    public partial class LoginForm : Form
    {
        private World LocalWorld { get; set; }
        private PlayerSP LocalPlayer { get; set; }

        private GameTcpClient ClientSocket;
        private PacketProtocol PacketProtocol;

        public LoginForm()
        {
            InitializeComponent();
            ClientSocket = new GameTcpClient();
            ClientSocket.ConnectCompleted += ClientSocket_ConnectCompleted;
            ClientSocket.SendCompleted += ClientSocket_SendCompleted;
        }

        private void ClientSocket_SendCompleted(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine($"Send event, bytes sent {(int)e.Data}");
        }

        private void ClientSocket_ConnectCompleted(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine($"Connected to {(IPEndPoint)e.Data}");
            PacketProtocol = new PacketProtocol(ClientSocket);
            PacketProtocol.PacketArrived += PacketProtocol_PacketArrived;
            PacketProtocol.Start();
        }

        private void PacketProtocol_PacketArrived(object sender, TcpCompletedEventArgs e)
        {
            var packet = Packet.Deserialize((byte[])e.Data);
            Console.WriteLine(packet.Content);
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            var splited = _ipTbox.Text.Split(':');
            ClientSocket.ConnectAsync(
                new IPEndPoint(
                    IPAddress.Parse(splited[0]),
                    int.Parse(splited[1])));
  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PacketProtocol.SendPacket(ClientSocket, new Packet
            {
                Content = "From client to server :3",
                Type = PacketType.SYSTEM_MESSAGE
            });
        }
    }
}
