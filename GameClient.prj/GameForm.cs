using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameClient
{
    public partial class GameForm : Form
    {
        private Socket ClientSocket { get; set; }
        private byte[] _buffer = new byte[1024];
        public GameForm()
        {
            InitializeComponent();

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23333);
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.Connect(ipPoint);

            ClientSocket.BeginReceive(_buffer, 0, 1024, SocketFlags.None, new AsyncCallback(OnReceivePacket), ClientSocket);

        }

        private void OnReceivePacket(IAsyncResult ar)
        {
            Socket server = (Socket)ar.AsyncState;

            int bytesRead = server.EndReceive(ar);
            var sb = new StringBuilder();

            if (bytesRead > 0)
            {
                sb.Append(Encoding.Unicode.GetString(_buffer, 0, bytesRead));

                server.BeginReceive(_buffer, 0, 1024, 0,
                    new AsyncCallback(OnReceivePacket), server);

                if (sb.Length > 1)
                {
                    Console.WriteLine(sb.ToString());
                }
            }
            else
            {
                if (sb.Length > 1)
                {
                    Console.WriteLine(sb.ToString());
                }
              
            }
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_nickTBox.Text))
            {
                byte[] data = Encoding.Unicode.GetBytes(_nickTBox.Text);
                ClientSocket.Send(data);
            }
            else
            {
                MessageBox.Show(
                    "Пожалуйста, введите имя :)",
                    "Опшивка!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
