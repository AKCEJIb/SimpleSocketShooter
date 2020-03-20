namespace Game.Client
{
    partial class ClientForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            Client.Disconnect();
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this._connectBtn = new System.Windows.Forms.Button();
            this._nickTBox = new System.Windows.Forms.TextBox();
            this._enterNameLbl = new System.Windows.Forms.Label();
            this._ipPortTbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._loginPanel = new System.Windows.Forms.Panel();
            this._disconnectBtn = new System.Windows.Forms.Button();
            this._gameField = new System.Windows.Forms.Panel();
            this._loginPanel.SuspendLayout();
            this._gameField.SuspendLayout();
            this.SuspendLayout();
            // 
            // _connectBtn
            // 
            this._connectBtn.Location = new System.Drawing.Point(219, 264);
            this._connectBtn.Margin = new System.Windows.Forms.Padding(2);
            this._connectBtn.Name = "_connectBtn";
            this._connectBtn.Size = new System.Drawing.Size(240, 28);
            this._connectBtn.TabIndex = 0;
            this._connectBtn.Text = "ИГРАТЬ!";
            this._connectBtn.UseVisualStyleBackColor = true;
            this._connectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
            // 
            // _nickTBox
            // 
            this._nickTBox.Location = new System.Drawing.Point(219, 241);
            this._nickTBox.Margin = new System.Windows.Forms.Padding(2);
            this._nickTBox.Name = "_nickTBox";
            this._nickTBox.Size = new System.Drawing.Size(241, 20);
            this._nickTBox.TabIndex = 1;
            // 
            // _enterNameLbl
            // 
            this._enterNameLbl.Location = new System.Drawing.Point(219, 220);
            this._enterNameLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._enterNameLbl.Name = "_enterNameLbl";
            this._enterNameLbl.Size = new System.Drawing.Size(240, 19);
            this._enterNameLbl.TabIndex = 2;
            this._enterNameLbl.Text = "ВВЕДИТЕ ИМЯ";
            this._enterNameLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _ipPortTbox
            // 
            this._ipPortTbox.Location = new System.Drawing.Point(507, 526);
            this._ipPortTbox.Margin = new System.Windows.Forms.Padding(2);
            this._ipPortTbox.Name = "_ipPortTbox";
            this._ipPortTbox.Size = new System.Drawing.Size(135, 20);
            this._ipPortTbox.TabIndex = 3;
            this._ipPortTbox.Text = "127.0.0.1:23333";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(485, 529);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "IP:";
            // 
            // _loginPanel
            // 
            this._loginPanel.Controls.Add(this._connectBtn);
            this._loginPanel.Controls.Add(this._nickTBox);
            this._loginPanel.Controls.Add(this._enterNameLbl);
            this._loginPanel.Controls.Add(this._ipPortTbox);
            this._loginPanel.Controls.Add(this.label1);
            this._loginPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._loginPanel.Location = new System.Drawing.Point(0, 0);
            this._loginPanel.Name = "_loginPanel";
            this._loginPanel.Size = new System.Drawing.Size(653, 557);
            this._loginPanel.TabIndex = 6;
            // 
            // _disconnectBtn
            // 
            this._disconnectBtn.Enabled = false;
            this._disconnectBtn.Location = new System.Drawing.Point(12, 524);
            this._disconnectBtn.Name = "_disconnectBtn";
            this._disconnectBtn.Size = new System.Drawing.Size(87, 23);
            this._disconnectBtn.TabIndex = 5;
            this._disconnectBtn.Text = "Отключится";
            this._disconnectBtn.UseVisualStyleBackColor = true;
            this._disconnectBtn.Click += new System.EventHandler(this.DisconnectBtn_Click);
            // 
            // _gameField
            // 
            this._gameField.Controls.Add(this._disconnectBtn);
            this._gameField.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gameField.Enabled = false;
            this._gameField.Location = new System.Drawing.Point(0, 0);
            this._gameField.Name = "_gameField";
            this._gameField.Size = new System.Drawing.Size(653, 557);
            this._gameField.TabIndex = 6;
            this._gameField.Visible = false;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 557);
            this.Controls.Add(this._loginPanel);
            this.Controls.Add(this._gameField);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "ClientForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IT\'S A GAME!!!1";
            this._loginPanel.ResumeLayout(false);
            this._loginPanel.PerformLayout();
            this._gameField.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _connectBtn;
        private System.Windows.Forms.TextBox _nickTBox;
        private System.Windows.Forms.Label _enterNameLbl;
        private System.Windows.Forms.TextBox _ipPortTbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _disconnectBtn;
        private System.Windows.Forms.Panel _loginPanel;
        private System.Windows.Forms.Panel _gameField;
    }
}

