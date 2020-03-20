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
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.Client
{
    public partial class ClientForm : Form
    {
        private GameClient Client { get; set; }
        private const int MAX_FPS = 60;
        private Timer timer = new Timer();
        public ClientForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, _gameField, new object[] { true });

            _gameField.PreviewKeyDown += PlayerControl;

            _nickTBox.KeyDown += new KeyEventHandler(NickTBox_KeyDown);
            _nickTBox.Select();

            Client = GameClient.GetInstance();

            Client.CriticalErrorOccured     += Client_CriticalErrorOccured;
            Client.ConnectedToServer        += Client_ConnectedToServer;
            Client.DisconnectedFromServer   += Client_DisconnectedFromServer;

            _gameField.Paint += new PaintEventHandler(Client.World.DrawWorld);

            
            timer.Interval = 1000 / MAX_FPS;
            timer.Tick += FPS_Tick;
            timer.Start();
        }

        // Контролим игрока
        private void PlayerControl(object sender, PreviewKeyDownEventArgs e)
        {

            e.IsInputKey = true;

            switch (e.KeyData)
            {
                case Keys.W:
                    Client.MovePlayer(new Vector2(0, -5));
                    break;
                case Keys.S:
                    Client.MovePlayer(new Vector2(0, 5));
                    break;
                case Keys.A:
                    Client.MovePlayer(new Vector2(-5, 0));
                    break;
                case Keys.D:
                    Client.MovePlayer(new Vector2(5, 0));
                    break;
            }
        }

        // Запускаем отправку пакетов движений и отрисовку
        private void FPS_Tick(object sender, EventArgs e)
        {
            _gameField.Invalidate();
            Client.SendMoveQueue();
        }

        private void Client_DisconnectedFromServer()
        {
            _loginPanel.BeginInvoke(new Action(() => {
                _loginPanel.Enabled = true;
                _loginPanel.Visible = true;
            }));

            _gameField.BeginInvoke(new Action(() => {
                _gameField.Enabled = false;
                _gameField.Visible = false;
            }));
            EnableButtons();
        }

        private void Client_ConnectedToServer()
        {

            _loginPanel.BeginInvoke(new Action(() => {
                _loginPanel.Enabled = false;
                _loginPanel.Visible = false;
            }));


            _gameField.BeginInvoke(new Action(() => {
                _gameField.Enabled = true;
                _gameField.Visible = true;
                _gameField.Focus();
            }));

            DisableButtons();
        }

        private void Client_CriticalErrorOccured(string obj)
        {
            MessageBox.Show(obj, "Критическая ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            EnableButtons();
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
                    _ipPortTbox.Enabled     = true;
                    _nickTBox.Enabled       = true;
                    _disconnectBtn.Enabled  = false;
                }));
            }
            else
            {
                _connectBtn.Enabled     = true;
                _ipPortTbox.Enabled     = true;
                _nickTBox.Enabled       = true;
                _disconnectBtn.Enabled  = false;
            }
        }

        private void DisableButtons()
        {
            if (_connectBtn.InvokeRequired)
            {
                _connectBtn.Invoke(new Action(() => {
                    _connectBtn.Enabled     = false;
                    _ipPortTbox.Enabled     = false;
                    _nickTBox.Enabled       = false;
                    _disconnectBtn.Enabled  = true;
                }));
            }
            else
            {
                _connectBtn.Enabled     = false;
                _ipPortTbox.Enabled     = false;
                _nickTBox.Enabled       = false;
                _disconnectBtn.Enabled  = true;
            }
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(_ipPortTbox.Text))
            {
                MessageBox.Show("Введите IP:Порт сервера!",
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

            Client.Connect(_ipPortTbox.Text, _nickTBox.Text);

            _connectBtn.Enabled     = false;
            _ipPortTbox.Enabled     = false;
            _nickTBox.Enabled       = false;
            
        }

        private void DisconnectBtn_Click(object sender, EventArgs e)
        {
            Client.Disconnect();
        }
    }
}
