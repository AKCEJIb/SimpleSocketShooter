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
        private Vector2 _playerDirection = new Vector2(0, -1);
        private const int MAX_FPS = 60;
        private Timer timer = new Timer();
        private bool _isChatUsing = false;
        private bool _shootReady = true;
        public ClientForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, _renderZone, new object[] { true });

            this.Text = $"SimpleSocketShooter v{typeof(GameClient).Assembly.GetName().Version}";

            _gameField.PreviewKeyDown += PlayerControl;

            _chatEnterField.KeyDown += new KeyEventHandler(ChatMessageSend);

            _nickTBox.KeyDown += new KeyEventHandler(NickTBox_KeyDown);
            _nickTBox.Select();

            Client = GameClient.GetInstance();

            Client.CriticalErrorOccured     += Client_CriticalErrorOccured;
            Client.ConnectedToServer        += Client_ConnectedToServer;
            Client.DisconnectedFromServer   += Client_DisconnectedFromServer;
            Client.ChatMessageArrived       += Client_ChatMessageArrived;

            _renderZone.Paint += new PaintEventHandler(Client.World.DrawWorld);

            
            timer.Interval = 1000 / MAX_FPS;
            timer.Tick += FPS_Tick;
            timer.Start();
        }

        private void Client_ChatMessageArrived(string obj)
        {
            _chatWindow.BeginInvoke(new Action(() => _chatWindow.Text += obj));
        }

        private void ChatMessageSend(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Отправка сообщения...
                if(!string.IsNullOrEmpty(_chatEnterField.Text) && !string.IsNullOrWhiteSpace(_chatEnterField.Text))
                    Client.EnqueueChatMessage(_chatEnterField.Text.Trim());

                _chatEnterField.Clear();
                _chatEnterField.Enabled = false;
                _gameField.Focus();
                _isChatUsing = false;

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
            
        }

        // Контролим игрока
        private void PlayerControl(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;

            if (!Client.World.LocalPlayer.Alive || _isChatUsing || IsInRenderZone(e.KeyData))
                return;

            switch (e.KeyData)
            {
                case Keys.W:
                    Client.EnqueueMove(new Vector2(0, -1));
                    _playerDirection = new Vector2(0, -1);
                    break;
                case Keys.S:
                    Client.EnqueueMove(new Vector2(0, 1));
                    _playerDirection = new Vector2(0, 1);
                    break;
                case Keys.A:
                    Client.EnqueueMove(new Vector2(-1, 0));
                    _playerDirection = new Vector2(-1, 0);
                    break;
                case Keys.D:
                    Client.EnqueueMove(new Vector2(1, 0));
                    _playerDirection = new Vector2(1, 0);
                    break;
                case Keys.Enter:
                    _isChatUsing = true;
                    _chatEnterField.Enabled = true;
                    _chatEnterField.Select();
                    break;
                case Keys.C:
                    if (_shootReady)
                    {
                        Client.EnqueueShoot(_playerDirection);
                        GunReload();
                    }
                    break;
            }
        }

        private void GunReload()
        {
            _shootReady = false;
            var timer = new System.Timers.Timer();
            timer.Interval = 400;
            timer.AutoReset = false;
            timer.Elapsed += (s,e) => _shootReady = true;
            timer.Start();
        }

        private bool IsInRenderZone(Keys k)
        {
            var ply = Client.World.LocalPlayer;
            var posX = ply.Position.X;
            var posY = ply.Position.Y;

            switch (k)
            {
                case Keys.W:
                    return !(posY - 10 > -_renderZone.Height / 2);
                case Keys.S:
                    return !(posY + 10 < _renderZone.Height / 2);
                case Keys.A:
                    return !(posX - 10 > -_renderZone.Width / 2);
                case Keys.D:
                    return !(posX + 10 < _renderZone.Width / 2);
            }

            return false;
        }

        // Запускаем отправку пакетов движений и отрисовку
        private void FPS_Tick(object sender, EventArgs e)
        {
            _renderZone.Invalidate();
            Client.SendPacketQueue();
            if(Client.World.LocalPlayer != null)
                if (!Client.World.LocalPlayer.Alive)
                {
                    if(!_resurectBtn.Enabled)
                    {
                        _resurectBtn.Enabled = true;
                        _resurectBtn.Visible = true;
                    }
                }
                else
                {
                    if (_resurectBtn.Enabled)
                    {
                        _resurectBtn.Enabled = false;
                        _resurectBtn.Visible = false;
                    }
                }
        }

        private void Client_DisconnectedFromServer()
        {
            _loginPanel.BeginInvoke(new Action(() => {
                _loginPanel.Enabled = true;
                _loginPanel.Visible = true;

                this.Text = $"SimpleSocketShooter v{typeof(string).Assembly.GetName().Version}";
            }));

            _gameField.BeginInvoke(new Action(() => {
                _gameField.Enabled = false;
                _gameField.Visible = false;

                _chatEnterField.Text = string.Empty;
                _chatWindow.Text     = string.Empty;
            }));
            EnableButtons();
        }

        private void Client_ConnectedToServer(string serverip, string plyName)
        {

            _loginPanel.BeginInvoke(new Action(() => {
                _loginPanel.Enabled = false;
                _loginPanel.Visible = false;
            }));


            _gameField.BeginInvoke(new Action(() => {
                _gameField.Enabled = true;
                _gameField.Visible = true;
                _gameField.Focus();

                this.Text = $"SimpleSocketShooter - Server: {serverip} Player: {plyName}";
            }));

            DisableButtons();
        }

        private void Client_CriticalErrorOccured(string obj)
        {
            MessageBox.Show(obj, "Критическая ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            DisconnectBtn_Click(this, EventArgs.Empty);
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

        private void ResurectBtn_Click(object sender, EventArgs e)
        {
            Client.EnqueueRevive();
            _gameField.Focus();
        }
    }
}
