using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Splendor
{
    public partial class GameStart : Form
    {
        string path_snd;

        public GameStart()
        {
            InitializeComponent();
            path_snd = Environment.CurrentDirectory;
            path_snd = Path.GetFullPath(Path.Combine(path_snd, @"..\..\")) + @"\Resources\Sounds\";
        }

        private void GameStart_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(path_snd + "2_start.wav");
            sp.Play();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void GameStart_Load(object sender, EventArgs e)
        {
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(path_snd + "1_title.wav");
            sp.Play();
        }
    }
}
