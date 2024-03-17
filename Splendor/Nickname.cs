using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;

namespace Splendor
{
    public partial class Nickname : Form
    {
        #region DB_idpwd
        string _server = "155.230.235.248"; //DB 서버 주소, 로컬일 경우 localhost
        int _port = 34056; //DB 서버 포트
        string _database = "swdb479"; //DB 이름
        string _id = "2019111479"; //계정 아이디
        string _pw = "user@111479"; //계정 비밀번호
        string _connectionAddress = "";
        #endregion
        string path_snd;

        public Nickname()
        {
            InitializeComponent();
            _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);
            path_snd = Environment.CurrentDirectory;
            path_snd = Path.GetFullPath(Path.Combine(path_snd, @"..\..\")) + @"\Resources\Sounds\";
        }

        private void Nickname_Click(object sender, EventArgs e)
        {
            if (textBox2.Visible == true)//닉네임 입력하세요
            {
                textBox2.Visible = false;
            }
            else if (textBox3.Visible == true)//이미 존재하는 닉네임
            {
                textBox3.Visible = false;
            }
            else
            {
                bool same = false;
                try
                {
                    using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                    {
                        mysql.Open();
                        string selectQuery = string.Format("SELECT * FROM splendor_info");

                        MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                        MySqlDataReader table = command.ExecuteReader();

                        while (table.Read())
                        {
                            if (textBox1.Text == table[0].ToString())
                                same = true;
                        }

                        table.Close();
                        mysql.Close();
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                if (textBox1.Text == "")
                {
                    textBox2.Visible = true;
                }
                else if (same)
                {
                    textBox3.Visible = true;
                }
                else
                {
                    try
                    {
                        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                        {
                            mysql.Open();
                            string selectQuery = string.Format("INSERT INTO splendor_info(nickname) VALUES ('{0}');", textBox1.Text);

                            MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                            command.ExecuteReader();
                            mysql.Close();
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(path_snd + "2_start.wav");
            sp.Play();

            if (textBox2.Visible == true)//닉네임 입력하세요
            {
                textBox2.Visible = false;
            }
            else if (textBox3.Visible == true)//이미 존재하는 닉네임
            {
                textBox3.Visible = false;
            }
            else
            {
                bool same = false;
                try
                {
                    using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                    {
                        mysql.Open();
                        string selectQuery = string.Format("SELECT * FROM splendor_info");

                        MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                        MySqlDataReader table = command.ExecuteReader();

                        while (table.Read())
                        {
                            if (textBox1.Text == table[0].ToString())
                                same = true;
                        }

                        table.Close();
                        mysql.Close();
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
                if (textBox1.Text == "")
                {
                    textBox2.Visible = true;
                }
                else if(same)
                {
                    textBox3.Visible = true;
                }
                else
                {
                    try
                    {
                        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                        {
                            mysql.Open();
                            string selectQuery = string.Format("INSERT INTO splendor_info(nickname) VALUES ('{0}');", textBox1.Text);

                            MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                            command.ExecuteReader();
                            mysql.Close();
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}
