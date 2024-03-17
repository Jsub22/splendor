 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using System.IO;

//여기 서버 연결 안함!
//sql만 사용했음
namespace Splendor
{
    public partial class Home : Form
    {
        #region DB_idpwd
        string _server = "155.230.235.248"; //DB 서버 주소, 로컬일 경우 localhost
        int _port = 34056; //DB 서버 포트
        string _database = "swdb479"; //DB 이름
        string _id = "2019111479"; //계정 아이디
        string _pw = "user@111479"; //계정 비밀번호
        string _connectionAddress = "";
        #endregion

        public string nickname;
        public string roomcode;
        public bool roomstate;
        public int who;
        public string back_nick;
        public string path_snd;
        System.Media.SoundPlayer sp;

        public Home(string nick)
        {
            InitializeComponent();
            nickname = nick;
            button1.Text = "<" + nickname + "> 닉네임 변경하기";

            _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);
            path_snd = Environment.CurrentDirectory;
            path_snd = Path.GetFullPath(Path.Combine(path_snd, @"..\..\")) + @"\Resources\Sounds\8.wav";
            sp = new System.Media.SoundPlayer(path_snd);
        }

        public string[] dbnickname = new string[100];
        private void button1_Click(object sender, EventArgs e)//닉네임 바꾸기
        {
            sp.Play();

            if (textBox2.Visible == true)
            {
                textBox2.Visible = false;
            }
            else
            {
                try
                {
                    using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                    {
                        mysql.Open();
                        string selectQuery = string.Format("SELECT nickname FROM splendor_info");//만들어진 룸코드들 불러오기

                        MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                        MySqlDataReader table = command.ExecuteReader();

                        int i = 0;
                        while (table.Read())
                        {
                            dbnickname[i] = table[0].ToString();
                            i++;
                        }

                        table.Close();
                        mysql.Close();
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

                NewNickname newnickname = new NewNickname(this);
                if (newnickname.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                        {
                            mysql.Open();
                            string updateQuery = string.Format("UPDATE splendor_info SET nickname='{0}' WHERE nickname='{1}'", nickname, back_nick);

                            MySqlCommand command = new MySqlCommand(updateQuery, mysql);
                            command.ExecuteReader();
                            mysql.Close();
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                    button1.Text = "<" + nickname + "> 닉네임 변경하기";
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)//방만들기
        {
            sp.Play();

            if (textBox2.Visible == true)//모든 버튼에 이게 붙어있는데 신경 안써도 되는 부분
            {
                textBox2.Visible = false;
            }
            else
            {
                string[] db_roomcode = new string[100];
                try
                {
                    using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                    {
                        mysql.Open();
                        string selectQuery = string.Format("SELECT roomcode FROM splendor_roomcode");//만들어진 룸코드들 불러오기

                        MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                        MySqlDataReader table = command.ExecuteReader();

                        int i = 0;
                        while (table.Read())
                        {
                            db_roomcode[i] = table[0].ToString();
                            i++;
                        }

                        table.Close();
                        mysql.Close();
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

                Random rand = new Random();//랜덤으로 룸코드 배정해서
                roomcode = rand.Next(0, 10).ToString();
                roomcode += rand.Next(0, 10).ToString();
                roomcode += rand.Next(0, 10).ToString();
                roomcode += rand.Next(0, 10).ToString();
                roomcode += rand.Next(0, 10).ToString();

                for (int i = 0; i < roomcode.Length; i++)
                    if (db_roomcode[i] == roomcode)//이미 존재하는 룸코드인지 확인
                    {
                        i = 0;
                        roomcode = rand.Next(0, 10).ToString();
                        roomcode += rand.Next(0, 10).ToString();
                        roomcode += rand.Next(0, 10).ToString();
                        roomcode += rand.Next(0, 10).ToString();
                        roomcode += rand.Next(0, 10).ToString();
                    }

                RoomMaker roommaker = new RoomMaker(this);//룸만드는 폼 열어서 거기에 있는 텍스트박스에 룸코드 넣어줌
                if (roommaker.ShowDialog() == DialogResult.OK)//만든다고 하면
                {
                    who = 0;//방장이라는 표시 해주고
                    this.DialogResult = DialogResult.OK;
                    this.Close();//폼 닫기
                }//main안열고 닫아도 되냐 싶겠지만 이거 관련 코드는 program가면 한눈에 볼 수 있다.
            }
        }

        public string[] dbroomcode = new string[100];
        public string[] dbcount = new string[100];
        private void button3_Click(object sender, EventArgs e)//들어가고자 하는 룸의 코드를 알고 있을 때
        {
            sp.Play();

            if (textBox2.Visible == true)
            {
                textBox2.Visible = false;
            }
            else
            {
                try
                {
                    using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                    {
                        mysql.Open();
                        string selectQuery = string.Format("SELECT * FROM splendor_roomcode");//존재하는 룸코드 다 불러와서

                        MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                        MySqlDataReader table = command.ExecuteReader();

                        int i = 0;
                        while (table.Read())
                        {
                            dbroomcode[i] = table[0].ToString();
                            dbcount[i] = table[2].ToString();
                            i++;
                        }

                        table.Close();
                        mysql.Close();
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

                FriendRoom friendroom = new FriendRoom(this);//이 폼의 텍스트박스에 넣어주고
                if (friendroom.ShowDialog() == DialogResult.OK)
                {
                    who = 1;//들어갈 수 있으면 방장이 아니라는 표시 해주고
                    this.DialogResult = DialogResult.OK;
                    this.Close();//폼 닫기
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)//랜덤 방 들어가기
        {
            sp.Play();

            if (textBox2.Visible == true) 
            {
                textBox2.Visible = false;
            }
            else
            {
                string[] true_roomcode = new string[100];
                int i = 0;
                try
                {
                    using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                    {
                        mysql.Open();
                        string selectQuery = string.Format("SELECT roomcode FROM splendor_roomcode WHERE count<4 AND state='True'");
                        //방이 공개이고, 방에 들어있는 인원이 4명 이하인 룸코드 불러오기
                        MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                        MySqlDataReader table = command.ExecuteReader();

                        while (table.Read())
                        {
                            true_roomcode[i] = table[0].ToString();
                            i++;
                        }
                        table.Close();
                        mysql.Close();
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                if (i != 0)//들어갈 수 있는 방이 존재하면
                {
                    Random rand = new Random();
                    int a = rand.Next(0, i);//랜덤 돌려서
                    roomcode = true_roomcode[a];//방 코드 넣고
                    who = 1;//방장 아니라 표시한 뒤
                    this.DialogResult = DialogResult.OK;
                    this.Close();//폼 닫기
                }
                else//들어갈 수 있는 방이 없으면
                    textBox2.Visible = true;
            }  
        }

        private void button5_Click(object sender, EventArgs e)//게임설명
        {
            sp.Play();

            if (textBox2.Visible == true)
            {
                textBox2.Visible = false;
            }
            else
            {
                new Explanation().Show();
            }
        }

        private void button6_Click(object sender, EventArgs e)//닫기
        {
            sp.Play();

            if (textBox2.Visible == true)
            {
                textBox2.Visible = false;
            }
            else
            {
                DialogResult = DialogResult.None;
                this.Close();
            }
        }

        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    string deleteQuery = string.Format("DELETE FROM splendor_info WHERE nickname='{0}'", nickname);

                    MySqlCommand command = new MySqlCommand(deleteQuery, mysql);
                    command.ExecuteReader();
                    mysql.Close();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }


    }
}
