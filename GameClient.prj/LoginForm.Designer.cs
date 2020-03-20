namespace Game.Client
{
    partial class LoginForm
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
            ClientSocket?.ShutdownAsync();
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
            this._ipTbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._disconnectBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _connectBtn
            // 
            this._connectBtn.Location = new System.Drawing.Point(120, 202);
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
            this._nickTBox.Location = new System.Drawing.Point(120, 179);
            this._nickTBox.Margin = new System.Windows.Forms.Padding(2);
            this._nickTBox.Name = "_nickTBox";
            this._nickTBox.Size = new System.Drawing.Size(241, 20);
            this._nickTBox.TabIndex = 1;
            // 
            // _enterNameLbl
            // 
            this._enterNameLbl.Location = new System.Drawing.Point(120, 158);
            this._enterNameLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._enterNameLbl.Name = "_enterNameLbl";
            this._enterNameLbl.Size = new System.Drawing.Size(240, 19);
            this._enterNameLbl.TabIndex = 2;
            this._enterNameLbl.Text = "ВВЕДИТЕ ИМЯ";
            this._enterNameLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _ipTbox
            // 
            this._ipTbox.Location = new System.Drawing.Point(323, 324);
            this._ipTbox.Margin = new System.Windows.Forms.Padding(2);
            this._ipTbox.Name = "_ipTbox";
            this._ipTbox.Size = new System.Drawing.Size(135, 20);
            this._ipTbox.TabIndex = 3;
            this._ipTbox.Text = "127.0.0.1:23333";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(301, 327);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "IP:";
            // 
            // _disconnectBtn
            // 
            this._disconnectBtn.Enabled = false;
            this._disconnectBtn.Location = new System.Drawing.Point(12, 322);
            this._disconnectBtn.Name = "_disconnectBtn";
            this._disconnectBtn.Size = new System.Drawing.Size(87, 23);
            this._disconnectBtn.TabIndex = 5;
            this._disconnectBtn.Text = "Отключится";
            this._disconnectBtn.UseVisualStyleBackColor = true;
            this._disconnectBtn.Click += new System.EventHandler(this.DisconnectBtn_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 352);
            this.Controls.Add(this._disconnectBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._ipTbox);
            this.Controls.Add(this._enterNameLbl);
            this.Controls.Add(this._nickTBox);
            this.Controls.Add(this._connectBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IT\'S A GAME!!!1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _connectBtn;
        private System.Windows.Forms.TextBox _nickTBox;
        private System.Windows.Forms.Label _enterNameLbl;
        private System.Windows.Forms.TextBox _ipTbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _disconnectBtn;
    }
}

