using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Splendor
{
    public partial class RoomMaker : Form
    {
        Home home;
        string path_snd;
        System.Media.SoundPlayer sp;

        public RoomMaker(Home h)
        {
            InitializeComponent();
            home = h;
            textBox1.Text = home.roomcode;
            path_snd = Environment.CurrentDirectory;
            path_snd = Path.GetFullPath(Path.Combine(path_snd, @"..\..\")) + @"\Resources\Sounds\8.wav";
            sp = new System.Media.SoundPlayer(path_snd);
        }

        private void button1_Click(object sender, EventArgs e)//textBox1 값은 우리가 서버에서 랜덤으로 넣자 그게 더 간단할듯
        {
            sp.Play();

            if (radioButton1.Checked)
                home.roomstate = true;
            else
                home.roomstate = false;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sp.Play();
            this.Close();
        }
    }
}
