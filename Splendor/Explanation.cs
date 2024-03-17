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
    public partial class Explanation : Form
    {
        public string path_snd;
        System.Media.SoundPlayer sp;

        public Explanation()
        {
            InitializeComponent();
            path_snd = Environment.CurrentDirectory;
            path_snd = Path.GetFullPath(Path.Combine(path_snd, @"..\..\")) + @"\Resources\Sounds\8.wav";
            sp = new System.Media.SoundPlayer(path_snd);

            textBox1.Text = "스플렌더는 보석으로 카드를 사서 15점을 먼저 모으는 사람이 이기는 게임이다.";
            textBox1.Text += "\r\n";
            textBox1.Text += "보석은 파란색, 흰색, 초록색, 빨간색, 검은색, 황금색이 있는데 황금색 보석은 그 어떤 보석이든 대체가능하다.";
            textBox1.Text += "\r\n";
            textBox1.Text += "\r\n";
            textBox1.Text += "자신의 차례에 할 수 있는 일";
            textBox1.Text += "\r\n";
            textBox1.Text += "-보석 들고오기(소지 보석이 10개를 초과하면 안된다)";
            textBox1.Text += "\r\n";
            textBox1.Text += "=> 같은 색 2개(해당 보석이 4개 이상일 때 가능)";
            textBox1.Text += "\r\n";
            textBox1.Text += "=> 다른 색 3개";
            textBox1.Text += "\r\n";
            textBox1.Text += "=> 황금색 1개와 카드 1장 가져오기(뒷장 추가설명)";
            textBox1.Text += "\r\n";
            textBox1.Text += "\r\n";
            textBox1.Text += "영구보석을 귀족카드에 표시된 만큼 가지고 있다면 귀족을 방문을 받을 수 있다.";
            textBox1.Text += "\r\n";
            textBox1.Text += "이때 귀족의 방문은 액션으로 취급하지 않는다.";
            textBox1.Text += "\r\n";

            textBox2.Text = "카드 가져오기";
            textBox2.Text += "\r\n";
            textBox2.Text += "황금색 보석을 가지고 올 때 펼쳐져있거나 뒤집혀져 있는 카드 1장을 가져온다.";
            textBox2.Text += "\r\n";
            textBox2.Text += "한 번 가져온 카드는 다시 내려놓을 수 없다.";
            textBox2.Text += "\r\n";
            textBox2.Text += "카드는 총 3개를 가지고 있을 수 있는데 카드는 사지 않는 한 줄어들지 않는다.";
            textBox2.Text += "\r\n";
            textBox2.Text += "\r\n";
            textBox2.Text += "게임 승리";
            textBox2.Text += "\r\n";
            textBox2.Text += "15점을 먼저 얻은 사람을 기준으로 차례를 모두 마친 뒤 가장 높은 점수를 가진 사람이 승리한다.";
            textBox2.Text += "\r\n";
            textBox2.Text += "가장 높은 점수가 동점이라면 카드를 더 적게 가진 사람의 승리이다.";

            button2.Focus();
        }

        private void button1_Click(object sender, EventArgs e)//확인
        {
            sp.Play();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)//페이지 넘기기
        {
            sp.Play();

            if (textBox1.Visible)
            {
                textBox1.Visible = false;
                textBox2.Visible = true;
                pictureBox1.Visible = false;
                pictureBox2.Visible = true;
                label1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                label4.Visible = true;
                label5.Visible = true;
                button2.Text = "이전";
            }
            else
            {
                textBox1.Visible = true;
                textBox2.Visible = false;
                pictureBox1.Visible = true;
                pictureBox2.Visible = false;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = false;
                label5.Visible = false;
                button2.Text = "다음";
            }
        }
    }
}
