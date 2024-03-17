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
    public partial class FriendRoom : Form
    {
        Home home;
        string path_snd;
        System.Media.SoundPlayer sp;

        public FriendRoom(Home h)
        {
            InitializeComponent();
            home = h;
            path_snd = Environment.CurrentDirectory;
            path_snd = Path.GetFullPath(Path.Combine(path_snd, @"..\..\")) + @"\Resources\Sounds\8.wav";
            sp = new System.Media.SoundPlayer(path_snd);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sp.Play();

            if (textBox2.Visible == true || textBox3.Visible == true)
            {
                textBox2.Visible = false;
                textBox3.Visible = false;
            }
            else
            {
                bool same = false;
                for (int i = 0; i < home.dbroomcode.Length; i++)
                {
                    if (home.dbcount[i] == "4")
                    {
                        same = true;
                        textBox3.Visible = true;
                    }
                    else if (home.dbroomcode[i] == textBox1.Text)
                    {
                        same = true;
                        DialogResult = DialogResult.OK;
                        this.Close();
                        home.roomcode = textBox1.Text;
                    }
                }
                if (!same)
                    textBox2.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sp.Play();

            if (textBox2.Visible || true && textBox3.Visible == true)
            {
                textBox2.Visible = false;
                textBox3.Visible = false;
            }
            else
                this.Close();
        }

        private void FriendRoom_Click(object sender, EventArgs e)
        {
            sp.Play();

            if (textBox2.Visible || true && textBox3.Visible == true)
            {
                textBox2.Visible = false;
                textBox3.Visible = false;
            }
        }
    }
}
