using GameShared;
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

namespace GameClient
{
    public partial class GameForm : Form
    {
        private World LocalWorld { get; set; }
        private PlayerSP LocalPlayer { get; set; }

        private Client Client { get; set; }
        public GameForm()
        {
            InitializeComponent();

            LocalWorld = World.GetInstance();
            Client = new Client("127.0.0.1", 23333);
            LocalPlayer = new PlayerSP("Test", 100, 0, 0);
        }

        

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_nickTBox.Text))
            {
                try
                {
                    LocalPlayer.SetName(_nickTBox.Text);

                    _connectBtn.Enabled = false;
                    Client.Connect();

                    LocalWorld.AddPlayer(LocalPlayer);

                    using (var ms = new MemoryStream())
                    {

                        var bf = new BinaryFormatter();
                        bf.Serialize(ms, new Packet{
                            Type = PacketType.PLAYER,
                            Content = LocalPlayer });

                        //GZipStream gz = new GZipStream(ms, CompressionLevel.Optimal);

                        //var buff = new byte[1024];
                        //gz.Write(buff, 0, buff.Length);

                        Client.Send(ms.ToArray());

                        Client.SendPacket(new Packet
                        {
                            Type = PacketType.MESSAGE,
                            Content = "Hello, World!"
                        });
                    }
                
                }
                catch(Client.ConnectionException ex)
                {
                    MessageBox.Show($"Не могу присоединится :(\n{ex.Message}",
                        "Ошибка!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    _connectBtn.Enabled = true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Неожиданная ошибка :(\n{ex.Message}",
                        "Ошибка!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                
            }
            else
            {
                MessageBox.Show(
                    "Пожалуйста, введите имя :)",
                    "Ошибка!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
