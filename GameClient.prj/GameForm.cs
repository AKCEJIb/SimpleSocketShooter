using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameClient
{
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();


        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_nickTBox.Text))
            {

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
