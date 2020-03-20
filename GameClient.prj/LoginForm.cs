using Game.Networking;
using Game.Networking.Entity;
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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.Client
{
    public partial class LoginForm : Form
    {
        private World LocalWorld { get; set; }
        private PlayerSp LocalPlayer { get; set; }

        private GameTcpClient ClientSocket;
        private PacketProtocol PacketProtocol;

        public LoginForm()
        {
            InitializeComponent();

            _nickTBox.KeyDown += new KeyEventHandler(NickTBox_KeyDown);
            _nickTBox.Select();

            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _nickTBox.Text = LocalPlayer.Name;
        }

        private void NickTBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ConnectBtn_Click(sender, e);

                e.SuppressKeyPress  = true;
                e.Handled           = true;
            }
        }

        private void EnableButtons()
        {
            if (_connectBtn.InvokeRequired)
            {
                _connectBtn.Invoke(new Action(() => {
                    _connectBtn.Enabled     = true;
                    _ipTbox.Enabled         = true;
                    _nickTBox.Enabled       = true;
                    _disconnectBtn.Enabled  = false;
                }));
            }
            else
            {
                _connectBtn.Enabled     = true;
                _ipTbox.Enabled         = true;
                _nickTBox.Enabled       = true;
                _disconnectBtn.Enabled  = false;
            }
        }

        private void ClientSocket_ShutdownCompleted(object sender, TcpCompletedEventArgs e)
        {
            Console.WriteLine("Disconnected from server.");

            PacketProtocol.PacketArrived -= PacketProtocol_PacketArrived;

            ClientSocket.ConnectCompleted -= ClientSocket_ConnectCompleted;
            ClientSocket.ShutdownCompleted -= ClientSocket_ShutdownCompleted;

            ClientSocket.Close();
            ClientSocket = null;
            PacketProtocol = null;

            EnableButtons();
        }

        private void ClientSocket_ConnectCompleted(object sender, TcpCompletedEventArgs e)
        {
            if (!e.Error)
            {
                Console.WriteLine($"Connected to {(IPEndPoint)e.Data}");
                PacketProtocol = new PacketProtocol(ClientSocket);
                PacketProtocol.PacketArrived += PacketProtocol_PacketArrived;
                PacketProtocol.Start();

                Console.WriteLine("Sending client info...");

                PacketProtocol.SendPacket(ClientSocket, new Packet
                {
                    Content = LocalPlayer,
                    Type = PacketType.PLAYER_INFO
                });

                if (_disconnectBtn.InvokeRequired)
                {
                    _disconnectBtn.Invoke(new Action(() => {
                        _disconnectBtn.Enabled = true;
                    }));
                }
                else
                {
                    _disconnectBtn.Enabled = true;
                }

                var timer = new Timer();
                timer.Interval = 500;
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            else
            {
                MessageBox.Show($"Удалённый сервер не отвечает!",
                    "Ошибка соединения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Console.WriteLine(e.Data);

                EnableButtons();
            }
        }

        private void PacketProtocol_PacketArrived(object sender, TcpCompletedEventArgs e)
        {
            if (!e.Error)
            {
                var bytePacket = Packet.Deserialize((byte[])e.Data);
                var packet = bytePacket as Packet;
                if (packet != null)
                {
                    switch (packet.Type)
                    {
                        case PacketType.PLAYER_INFO:
                            LocalPlayer.UpdatePlayer((PlayerShared)packet.Content);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine($"Server send corrupted packet!");
                }
            }
            else
            {
                MessageBox.Show($"Удалённый сервер принудительно разовал соединение!",
                    "Ошибка соединения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                EnableButtons();
            }
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(_ipTbox.Text))
            {
                MessageBox.Show("Введите IP:Порт адрес сервера!",
                    "Ошибка соединения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_nickTBox.Text)) { 
                MessageBox.Show("Введите имя персонажа!",
                    "Ошибка соединения",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            LocalPlayer = new PlayerSp(_nickTBox.Text, 100, 0, 0);

            var splited = _ipTbox.Text.Split(':');

            ClientSocket = new GameTcpClient();
            ClientSocket.ConnectCompleted += ClientSocket_ConnectCompleted;
            ClientSocket.ShutdownCompleted += ClientSocket_ShutdownCompleted;

            ClientSocket.ConnectAsync(
                new IPEndPoint(
                    IPAddress.Parse(splited[0]),
                    int.Parse(splited[1])));

            _connectBtn.Enabled     = false;
            _ipTbox.Enabled         = false;
            _nickTBox.Enabled       = false;
            
        }

        private void DisconnectBtn_Click(object sender, EventArgs e)
        {
            ClientSocket?.ShutdownAsync();
        }
    }
}
