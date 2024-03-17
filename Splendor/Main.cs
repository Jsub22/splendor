using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using System.IO;


namespace Splendor
{
    public partial class Main : Form
    {
        delegate void AppendTextDelegate(Control ctrl, string s);
        AppendTextDelegate _textAppender;
        Socket mainSock;

        #region DB_idpwd
        string _server = "155.230.235.248"; //DB 서버 주소, 로컬일 경우 localhost
        int _port = 34056; //DB 서버 포트
        string _database = "swdb479"; //DB 이름
        string _id = "2019111479"; //계정 아이디
        string _pw = "user@111479"; //계정 비밀번호
        string _connectionAddress = "";
        #endregion

        string nickname;
        string roomcode;
        string server_add;
        string myclient;

        int turn;
        int turn_for = 0;
        int who; // 방장이면 0

        Boolean starting;
        Boolean state;

        int selectpos; // 선택 위치
        string selectinfo; // 선택 정보

        int[] pub_jewel; // 공용 보석 정보

        List<string> list_Cards1; // 공용 카드 정보 레벨1
        List<string> list_Cards2; // 공용 카드 정보 레벨2
        List<string> list_Cards3; // 공용 카드 정보 레벨3
        List<string> list_Cards4; // 공용 카드 정보 귀족
        string[] field_Cards;

        List<string> list_nick;

        Dictionary<string, Player> players = new Dictionary<string, Player>(); // 클라이언트 게임 정보
        bool roomstate;

       // Label[] player = new Label[4];
       // PictureBox[] readyornot = new PictureBox[4];

        string path_rsc;
        int[] arr_img;
        int pos_k;

        public Main(string nick, string room, bool state, int w)
        {
            InitializeComponent();
            nickname = nick;
            roomcode = room;
            roomstate = state;
            starting = false;
            this.state = false;
            turn = -1;
            who = w;

            pub_jewel = new int[6] { 5, 5, 5, 5, 5, 5 };

            //레벨1,레벨2,레벨3,귀족4):가치: 필요보석(red: green:blue: white:black):영구보석번호
            list_Cards1 = new List<string>();
            list_Cards2 = new List<string>();
            list_Cards3 = new List<string>();
            list_Cards4 = new List<string>();
            field_Cards = new string[16];

            list_nick = new List<string>();

            label1.Text = roomcode;
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _textAppender = new AppendTextDelegate(AppendText);
            _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);

            path_rsc = Environment.CurrentDirectory;
            path_rsc = Path.GetFullPath(Path.Combine(path_rsc, @"..\..\")) + @"\Resources";

            arr_img = new int[16] { 100, 100, 100, 100, 101, 101, 101, 101, 102, 102, 102, 102, 103, 103, 103, 103 };
            st0.SizeMode = PictureBoxSizeMode.Zoom;
            st1.SizeMode = PictureBoxSizeMode.Zoom;
            st2.SizeMode = PictureBoxSizeMode.Zoom;
            st3.SizeMode = PictureBoxSizeMode.Zoom;
            st4.SizeMode = PictureBoxSizeMode.Zoom;
            st5.SizeMode = PictureBoxSizeMode.Zoom;
            st6.SizeMode = PictureBoxSizeMode.Zoom;
            st7.SizeMode = PictureBoxSizeMode.Zoom;
            st8.SizeMode = PictureBoxSizeMode.Zoom;
            st9.SizeMode = PictureBoxSizeMode.Zoom;
            st10.SizeMode = PictureBoxSizeMode.Zoom;
            st11.SizeMode = PictureBoxSizeMode.Zoom;
            st12.SizeMode = PictureBoxSizeMode.Zoom;
            st13.SizeMode = PictureBoxSizeMode.Zoom;
            st14.SizeMode = PictureBoxSizeMode.Zoom;
            st15.SizeMode = PictureBoxSizeMode.Zoom;
            emoticon1.SizeMode = PictureBoxSizeMode.Zoom;
            emoticon2.SizeMode = PictureBoxSizeMode.Zoom;
            emoticon3.SizeMode = PictureBoxSizeMode.Zoom;
            emoticon4.SizeMode = PictureBoxSizeMode.Zoom;
            emoticon5.SizeMode = PictureBoxSizeMode.Zoom;
            emoticon6.SizeMode = PictureBoxSizeMode.Zoom;
            emoticon7.SizeMode = PictureBoxSizeMode.Zoom;
            emoticon8.SizeMode = PictureBoxSizeMode.Zoom;
            emoticon9.SizeMode = PictureBoxSizeMode.Zoom;

            if (who == 0)
                start.Text = "게임 시작";
        }

        public void Display_Emoticon(string nick, string idx)
        {
            Image img = Image.FromFile(path_rsc + @"\Img_Emoticons\" + idx + ".png");
            if (nick == player1.Text)
            {
                p1pic.SizeMode = PictureBoxSizeMode.Zoom;
                p1pic.Image = img;
            }
            else if (nick == player2.Text)
            {
                p2pic.SizeMode = PictureBoxSizeMode.Zoom;
                p2pic.Image = img;
            }
            else if (nick == player3.Text)
            {
                p3pic.SizeMode = PictureBoxSizeMode.Zoom;
                p3pic.Image = img;
            }
            else
            {
                p4pic.SizeMode = PictureBoxSizeMode.Zoom;
                p4pic.Image = img;
            }
        }

        public void Play_Sound(int path)
        {
            System.Media.SoundPlayer sp = new System.Media.SoundPlayer(path_rsc + @"\Sounds\" + path.ToString() + ".wav");
            sp.Play();
        }

        public void Fill_Card(int pos)
        {
            Image img = Image.FromFile(path_rsc + @"\Image_Cards\c" + arr_img[pos].ToString() + ".png");
            
            switch (pos)
            {
                default:
                    //MessageBox.Show("default");
                    break;
                case 0:
                    st0.Image = img;
                    break;
                case 1:
                    st1.Image = img;
                    break;
                case 2:
                    st2.Image = img;
                    break;
                case 3:
                    st3.Image = img;
                    break;
                case 4:
                    st4.Image = img;
                    break;
                case 5:
                    st5.Image = img;
                    break;
                case 6:
                    st6.Image = img;
                    break;
                case 7:
                    st7.Image = img;
                    break;
                case 8:
                    st8.Image = img;
                    break;
                case 9:
                    st9.Image = img;
                    break;
                case 10:
                    st10.Image = img;
                    break;
                case 11:
                    st11.Image = img;
                    break;
                case 12:
                    st12.Image = img;
                    break;
                case 13:
                    st13.Image = img;
                    break;
                case 14:
                    st14.Image = img;
                    break;
                case 15:
                    st15.Image = img;
                    break;
            }
            //MessageBox.Show("refill : " + pos.ToString());
        }

        public void Move_Card(int pos)
        {
            string str = @"\Image_Cards\c";
            //Image img = img = Image.FromFile(path_rsc + @"\Image_Cards\c" + arr_img[pos].ToString() + ".jpg");
            Image img = img = Image.FromFile(path_rsc + str + arr_img[pos].ToString() + ".png");
            if (pos_k == 0)
            {
                keep0.SizeMode = PictureBoxSizeMode.Zoom;
                keep0.Image = img;
            }
            else if (pos_k == 1)
            {
                keep1.SizeMode = PictureBoxSizeMode.Zoom;
                keep1.Image = img;
            }
            else if (pos_k == 2)
            {
                keep2.SizeMode = PictureBoxSizeMode.Zoom;
                keep2.Image = img;
            }
            else
                ;
        }

        public void BuyCards(string Cards, string nick)
        {
            int[] result = players[nick].BuyCards(Cards); // 0~4:변경 이전의 값, 5:총 사용 gold 개수
            string[] info = Cards.Split(';');

            if (info[1] == 4.ToString())
            {
                ;
            }
            else
            {
                int[] jew = new int[5] { int.Parse(info[3]), int.Parse(info[4]), int.Parse(info[5]), int.Parse(info[6]), int.Parse(info[7]) };

                for (int i = 0; i < 5; i++)
                {
                    int temp = result[i] - jew[i];
                    if (temp < 0)
                        pub_jewel[i] += result[i];
                    else
                        pub_jewel[i] += jew[i];
                }
                pub_jewel[5] += result[5];
            }
        }

        public void TakeTokens(string jeww, string nick)
        {
            players[nick].TakeTokens(jeww);

            string[] selecttokens = jeww.Split(':'); // 필요보석(red:green:blue:white:black)
           
            for (int i = 0; i < selecttokens.Length; i++)
            {
                if (selecttokens[i] == "Red")
                    pub_jewel[0] -= 1;
                else if (selecttokens[i] == "Green")
                    pub_jewel[1] -= 1;
                else if (selecttokens[i] == "Blue")
                    pub_jewel[2] -= 1;
                else if (selecttokens[i] == "White")
                    pub_jewel[3] -= 1;
                else if (selecttokens[i] == "Black")
                    pub_jewel[4] -= 1;
            }
        }
        public void KeepCards(string Cards, string nick)
        {
            players[nick].KeepCards(Cards);

            pub_jewel[5] -= 1;
        }

        public void SetTokens()
        {
            red.Text = pub_jewel[0].ToString(); green.Text = pub_jewel[1].ToString();
            blue.Text = pub_jewel[2].ToString(); white.Text = pub_jewel[3].ToString();
            black.Text = pub_jewel[4].ToString(); gold.Text = pub_jewel[5].ToString();

            per_rd.Text = players[nickname].per_jewel[0].ToString(); rd.Text = players[nickname].jewel[0].ToString();
            per_gn.Text = players[nickname].per_jewel[1].ToString(); gn.Text = players[nickname].jewel[1].ToString();
            per_be.Text = players[nickname].per_jewel[2].ToString(); be.Text = players[nickname].jewel[2].ToString();
            per_wt.Text = players[nickname].per_jewel[3].ToString(); wt.Text = players[nickname].jewel[3].ToString();
            per_bk.Text = players[nickname].per_jewel[4].ToString(); bk.Text = players[nickname].jewel[4].ToString();
            gd.Text = players[nickname].jewel[5].ToString();
        }

        public void SetOthersTokens(string nick)
        {
            if (nick == nickname)
                return;

            int idx = list_nick.FindIndex(a => a.Contains(nick));

            if (idx == 0)
            {
                pr1tk1.Text = string.Format("{0}    {1}   {2}   {3}    {4}", players[nick].per_jewel[0], players[nick].per_jewel[1], players[nick].per_jewel[2], players[nick].per_jewel[3], players[nick].per_jewel[4]);
                pr1tk2.Text = string.Format("{0}    {1}   {2}   {3}   {4}    {5}", players[nick].jewel[0], players[nick].jewel[1], players[nick].jewel[2], players[nick].jewel[3], players[nick].jewel[4], players[nick].jewel[5]);

                pr1pt.Text = players[nick].point.ToString();
                pr1cards.Text = players[nick].buy_cnt.ToString();
            }
            else if (idx == 1)
            {
                pr2tk1.Text = string.Format("{0}    {1}   {2}   {3}    {4}", players[nick].per_jewel[0], players[nick].per_jewel[1], players[nick].per_jewel[2], players[nick].per_jewel[3], players[nick].per_jewel[4]);
                pr2tk2.Text = string.Format("{0}    {1}   {2}   {3}   {4}    {5}", players[nick].jewel[0], players[nick].jewel[1], players[nick].jewel[2], players[nick].jewel[3], players[nick].jewel[4], players[nick].jewel[5]);

                pr2pt.Text = players[nick].point.ToString();
                pr2cards.Text = players[nick].buy_cnt.ToString();
            }
            else if (idx == 2)
            {
                pr3tk1.Text = string.Format("{0}    {1}   {2}   {3}    {4}", players[nick].per_jewel[0], players[nick].per_jewel[1], players[nick].per_jewel[2], players[nick].per_jewel[3], players[nick].per_jewel[4]);
                pr3tk2.Text = string.Format("{0}    {1}   {2}   {3}   {4}    {5}", players[nick].jewel[0], players[nick].jewel[1], players[nick].jewel[2], players[nick].jewel[3], players[nick].jewel[4], players[nick].jewel[5]);

                pr3pt.Text = players[nick].point.ToString();
                pr3cards.Text = players[nick].buy_cnt.ToString();
            }
            else if (idx == 3)
            {
                pr4tk1.Text = string.Format("{0}    {1}   {2}   {3}    {4}", players[nick].per_jewel[0], players[nick].per_jewel[1], players[nick].per_jewel[2], players[nick].per_jewel[3], players[nick].per_jewel[4]);
                pr4tk2.Text = string.Format("{0}    {1}   {2}   {3}   {4}    {5}", players[nick].jewel[0], players[nick].jewel[1], players[nick].jewel[2], players[nick].jewel[3], players[nick].jewel[4], players[nick].jewel[5]);

                pr4pt.Text = players[nick].point.ToString();
                pr4cards.Text = players[nick].buy_cnt.ToString();
            }
        }

        public void SetTurnColors()
        {
            if (list_nick[turn] == player1.Text)
                pic_player1.BackColor = Color.Violet;
            else if (player1.Text != "Player1")
                pic_player1.BackColor = Color.Lime;

            if (list_nick[turn] == player2.Text)
                pic_player2.BackColor = Color.Violet;
            else if (player2.Text != "Player2")
                pic_player2.BackColor = Color.Lime;

            if (list_nick[turn] == player3.Text)
                pic_player3.BackColor = Color.Violet;
            else if (player3.Text != "Player3")
                pic_player3.BackColor = Color.Lime;

            if (list_nick[turn] == player4.Text)
                pic_player4.BackColor = Color.Violet;
            else if (player4.Text != "Player4")
                pic_player4.BackColor = Color.Lime;
        }

        public class Player
        {
            // 속성 (데이터)
            string nickname;

            public int point;
            public int[] jewel; // red green blue white black gold
            public int[] per_jewel;  // red green blue white black
            public List<string> keep_cards;
            public List<string> buy_cards;
            public int buy_cnt;

            // 기능 (메서드)
            public Player(string nickname)
            {
                this.nickname = nickname;
                this.point = 0;
                jewel = new int[6] { 0, 0, 0, 0, 0, 0 };
                per_jewel = new int[5] { 0, 0, 0, 0, 0 };
                keep_cards = new List<string>();
                buy_cards = new List<string>();
                buy_cnt = 0;
            }

            // 카드마다 string 값 =>
            // 종류(레벨1,레벨2,레벨3,귀족4):필요보석(red:green:blue:white:black):영구보석번호
            public int[] BuyCards(string Cards)
            {
                string[] info = Cards.Split(';');
                int[] jew = new int[5] { int.Parse(info[3]), int.Parse(info[4]), int.Parse(info[5]), int.Parse(info[6]), int.Parse(info[7]) };
                int[] tmpgold = new int[6] { 0, 0, 0, 0, 0, 0 };

                if (info[1] == 4.ToString())
                {
                    buy_cards.Add(Cards);
                }
                else
                {
                    int tmp = jewel[5];

                    for (int i = 0; i < 5; i++)
                    {
                        if (jewel[i] + per_jewel[i] < jew[i])
                        {
                            // gold 를 써야 하는 경우
                            if (jewel[i] + per_jewel[i] + tmp - jew[i] >= 0)
                            {
                                int temp = jew[i] - jewel[i] - per_jewel[i];
                                if (temp >= 0)
                                {
                                    tmp = tmp - tmpgold[i];
                                }
                                else
                                {
                                    tmpgold[i] = temp;
                                    tmp = tmp - tmpgold[i];
                                }
                            }
                        }
                    }

                    tmpgold[5] = tmpgold[0] + tmpgold[1] + tmpgold[2] + tmpgold[3] + tmpgold[4];

                    // 개인 보석
                    for (int i = 0; i < 5; i++)
                    {
                        int temp = per_jewel[i] + tmpgold[i] - jew[i];

                        if (temp >= 0)
                            // 변경 이전의 개수 저장
                            tmpgold[i] = jewel[i];
                        else
                        {
                            // 변경 이전의 개수 저장
                            tmpgold[i] = jewel[i];
                            jewel[i] = jewel[i] + temp;
                        }
                    }
                    jewel[5] = tmp;

                    // 영구 보석
                    per_jewel[int.Parse(info[8])] += 1;
                }

                // 포인트
                point += int.Parse(info[2]);

                buy_cnt += 1;

                return tmpgold;
            }

            public void KeepCards(string Cards)
            {
                keep_cards.Add(Cards);
                jewel[5] += 1;
            }

            public void TakeTokens(string jeww)
            {
                string[] selecttokens = jeww.Split(':'); // 필요보석(red:green:blue:white:black)

                for (int i = 0; i < selecttokens.Length; i++)
                {
                    if (selecttokens[i] == "Red")
                        jewel[0] += 1;
                    else if (selecttokens[i] == "Green")
                        jewel[1] += 1;
                    else if (selecttokens[i] == "Blue")
                        jewel[2] += 1;
                    else if (selecttokens[i] == "White")
                        jewel[3] += 1;
                    else if (selecttokens[i] == "Black")
                        jewel[4] += 1;
                }
            }

            public Boolean ExaminJewels(string Cards)
            {
                string[] info = Cards.Split(';');
                int[] jew = new int[5] { int.Parse(info[3]), int.Parse(info[4]), int.Parse(info[5]), int.Parse(info[6]), int.Parse(info[7]) };

                if (info[1] == 4.ToString())
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (per_jewel[i] <= jew[i])
                        {
                            ;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    int tmp = jewel[5];

                    for (int i = 0; i < 5; i++)
                    {
                        if (jewel[i] + per_jewel[i] < jew[i])
                        {
                            // gold 를 써야 하는 경우
                            if (jewel[i] + per_jewel[i] + tmp - jew[i] >= 0)
                            {
                                int temp = jew[i] - jewel[i] - per_jewel[i];
                                if (temp >= 0)
                                {
                                    tmp = tmp - temp;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }

                return true;
            }

            /*
            public void SetTokens()
            {
                per_rd.Text = per_jewel[0].ToString(); rd.Text = jewel[0].ToString();
                per_gn.Text = per_jewel[1].ToString(); gn.Text = jewel[1].ToString();
                per_be.Text = per_jewel[2].ToString(); be.Text = jewel[2].ToString();
                per_wt.Text = per_jewel[3].ToString(); wt.Text = jewel[3].ToString();
                per_bk.Text = per_jewel[4].ToString(); bk.Text = jewel[4].ToString();
                gd.Text = jewel[5].ToString();

                
                per_rd.Text = players[nickname].per_jewel[0].ToString(); rd.Text = players[nickname].jewel[0].ToString();
                per_gn.Text = players[nickname].per_jewel[1].ToString(); gn.Text = players[nickname].jewel[1].ToString();
                per_be.Text = players[nickname].per_jewel[2].ToString(); be.Text = players[nickname].jewel[2].ToString();
                per_wt.Text = players[nickname].per_jewel[3].ToString(); wt.Text = players[nickname].jewel[3].ToString();
                per_bk.Text = players[nickname].per_jewel[4].ToString(); bk.Text = players[nickname].jewel[4].ToString();
                gd.Text = players[nickname].jewel[5].ToString();
            }

                
                 string[] info = jeww.Split(':'); // 필요보석(red:green:blue:white:black)
                int[] jew = new int[5] { int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[2]), int.Parse(info[3]), int.Parse(info[4]) };

                jewel[0] += jew[0];
                jewel[1] += jew[1];
                jewel[2] += jew[2];
                jewel[3] += jew[3];
                jewel[4] += jew[4];
                */
            }

            #region 코드 너무 길다

            void AppendText(Control ctrl, string s)
        {
            if (ctrl.InvokeRequired) ctrl.Invoke(_textAppender, ctrl, s);
            else
            {
                string source = ctrl.Text;
                ctrl.Text = source + Environment.NewLine + s;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            { 
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    string selectQuery = string.Format("SELECT * FROM splendor_server");

                    MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                    MySqlDataReader table = command.ExecuteReader();

                    while (table.Read())
                    {
                        server_add = table[0].ToString();
                    }
                    table.Close();

                    selectQuery = string.Format("SELECT * FROM splendor_card WHERE level = 1");
                    command = new MySqlCommand(selectQuery, mysql);
                    table = command.ExecuteReader();
                    while (table.Read())
                    {
                        list_Cards1.Add(string.Format("{0};{1};{2};{3}{4}",table[0].ToString(), table[1].ToString(), table[2].ToString(), table[3].ToString(), table[4].ToString()));
                    }
                    table.Close();

                    selectQuery = string.Format("SELECT * FROM splendor_card WHERE level = 2");
                    command = new MySqlCommand(selectQuery, mysql);
                    table = command.ExecuteReader();
                    while (table.Read())
                    {
                        list_Cards2.Add(string.Format("{0};{1};{2};{3}{4}", table[0].ToString(), table[1].ToString(), table[2].ToString(), table[3].ToString(), table[4].ToString()));
                    }
                    table.Close();

                    selectQuery = string.Format("SELECT * FROM splendor_card WHERE level = 3");
                    command = new MySqlCommand(selectQuery, mysql);
                    table = command.ExecuteReader();
                    while (table.Read())
                    {
                        list_Cards3.Add(string.Format("{0};{1};{2};{3}{4}", table[0].ToString(), table[1].ToString(), table[2].ToString(), table[3].ToString(), table[4].ToString()));
                    }
                    table.Close();

                    selectQuery = string.Format("SELECT * FROM splendor_card WHERE level = 4");
                    command = new MySqlCommand(selectQuery, mysql);
                    table = command.ExecuteReader();
                    while (table.Read())
                    {
                        list_Cards4.Add(string.Format("{0};{1};{2};{3}{4}", table[0].ToString(), table[1].ToString(), table[2].ToString(), table[3].ToString(), table[4].ToString()));
                    }
                    table.Close();

                    mysql.Close();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            try { mainSock.Connect(server_add, 9999); }
            catch (Exception ex)
            {
                MessageBox.Show("연결에 실패했습니다!\n오류 내용: {0}", ex.Message);
                return;
            }

            // 연결 완료, 서버에서 데이터가 올 수 있으므로 수신 대기한다.
            AsyncObject obj = new AsyncObject(4096);
            obj.WorkingSocket = mainSock;
            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);

            /*Image img = Image.FromFile(path_rsc + @"\img_jewel1.jpg");
            pictureBox17.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox17.Image = img;
            img = Image.FromFile(path_rsc + @"\img_jewel2.jpg");
            pictureBox18.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox18.Image = img;*/

            for (int i = 0; i < 16; i++)
                Fill_Card(i);
        }

        public class AsyncObject
        {
            public byte[] Buffer;
            public Socket WorkingSocket;
            public readonly int BufferSize;
            public AsyncObject(int bufferSize)
            {
                BufferSize = bufferSize;
                Buffer = new byte[BufferSize];
            }

            public void ClearBuffer()
            {
                Array.Clear(Buffer, 0, BufferSize);
            }
        }
        #endregion

        void DataReceived(IAsyncResult ar)
        {
            try
            {
                // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                // 데이터 수신을 끝낸다.
                int received = obj.WorkingSocket.EndReceive(ar);

                // 받은 데이터가 없으면(연결끊어짐) 끝낸다.
                if (received <= 0)
                {
                    obj.WorkingSocket.Close();
                    return;
                }

                // 텍스트로 변환한다.
                string text = Encoding.UTF8.GetString(obj.Buffer);

                // 0x01 기준으로 짜른다.
                // tokens[0] - 보낸 사람 IP
                // tokens[1] - 보낸 메세지
                string[] tokens = text.Split('\x01');
                string ip = tokens[0];

                if (ip == "client")
                {
                    myclient = tokens[1];

                    try
                    {
                        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                        {
                            string insertQuery = "";
                            MySqlCommand command;
                            if (who == 0)//방장이면
                            {
                                mysql.Open();
                                insertQuery = string.Format("INSERT INTO splendor_roomcode(roomcode, state, count) VALUES ('{0}', '{1}', 1)", roomcode, roomstate.ToString());
                                //룸코드 테이블에 룸코드 추가하기
                                command = new MySqlCommand(insertQuery, mysql);
                                command.ExecuteReader();
                                mysql.Close();
                            }
                            else//방장이 아니면
                            {
                                mysql.Open();
                                insertQuery = string.Format("UPDATE splendor_roomcode SET count=count+1 WHERE roomcode='{0}'", roomcode);
                                //존재하는 룸코드 찾아서 인원만 고치기
                                command = new MySqlCommand(insertQuery, mysql);
                                command.ExecuteReader();
                                mysql.Close();
                            }

                            mysql.Open();
                            insertQuery = string.Format("INSERT INTO splendor_info(nickname, roomcode, client, host) VALUES ('{0}', '{1}', '{2}',{3})", nickname, roomcode, myclient, who);
                            //닉네임 넣기
                            //왜 insert인가 하면 home폼 닫았을 때 해당 닉네임 delete해서 그럼. 안그러면 그냥 폼 닫는거랑 main에 들어와서 폼 닫치는거랑 구별을 못하더라
                            command = new MySqlCommand(insertQuery, mysql);
                            command.ExecuteReader();
                            mysql.Close();
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                    byte[] bDts = Encoding.UTF8.GetBytes("enter" + '\x01' + nickname + '\x01' + roomcode + '\x01');//입장했다고 서버한테 알려줌
                    mainSock.Send(bDts);
                }
                else if (ip == "enter")
                {
                    list_nick.Clear();

                    try
                    {
                        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                        {
                            mysql.Open();
                            string selectQuery = string.Format("SELECT nickname FROM splendor_info WHERE roomcode='{0}' ORDER BY host", roomcode);

                            MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                            MySqlDataReader table = command.ExecuteReader();

                            while (table.Read())
                            {
                                list_nick.Add(table[0].ToString());
                            }

                            table.Close();
                            mysql.Close();
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                    // 프로필 설정
                    int count = list_nick.Count;

                    if (count == 1)
                    {
                        player1.Text = list_nick[0];
                    }
                    if (count == 2)
                    {
                        player1.Text = list_nick[0];
                        player2.Text = list_nick[1];
                    }
                    if (count == 3)
                    {
                        player1.Text = list_nick[0];
                        player2.Text = list_nick[1];
                        player3.Text = list_nick[2];
                    }
                    if (count == 4)
                    {
                        player1.Text = list_nick[0];
                        player2.Text = list_nick[1];
                        player3.Text = list_nick[2];
                        player4.Text = list_nick[3];
                    }

                    /* // 시도했는데 더 귀찮아져서 안 함
                       for (int i = 0; i < list_nick.Count; i++)
                       {
                           player[i] = new Label();
                           player[i].Text = list_nick[i];
                           player[i].Size = new Size(170, 28);
                           player[i].Location = new Point(955, 95 + 135*i);
                           player[i].BackColor = Color.Transparent;
                           player[i].Font = new Font("함초롬바탕 확장", 15, FontStyle.Bold);

                           Controls.Add(player[i]);

                           readyornot[i] = new PictureBox();
                           readyornot[i].Size = new Size(28, 28);
                           readyornot[i].Location = new Point(925, 95 + 135 * i);
                           readyornot[i].BackColor = Color.LightGray;

                           Controls.Add(readyornot[i]);
                       }
                       */

                    // 입장 이전 플레이어 상태까지 알림
                    listBox1.Items.Add(tokens[1] + " 님이 입장하셨습니다.");//입장했다

                    if (who != 0 && tokens[1] != nickname)
                    {
                        byte[] bDts = Encoding.UTF8.GetBytes("state" + '\x01' + "repost" + '\x01' + roomcode + '\x01' + nickname +
                            '\x01' + state.ToString() + '\x01' + tokens[1] + '\x01');
                        mainSock.Send(bDts);
                        //MessageBox.Show(nickname + "이 자신이 준비상태인지 아닌지 보내는 메세지");
                    }
                }

                //"state" + '\x01' + tokens[1] + '\x01' + tokens[3] + '\x01' // + state.ToString() + '\x01'
                else if (ip == "state")
                {
                    if (tokens[1] == "ready")
                    {
                        if (tokens[2] == player1.Text)
                        {
                            pic_player1.BackColor = Color.Lime;
                            listBox1.Items.Add("게임이 시작되었습니다");
                        }
                        else if (tokens[2] == player2.Text)
                        {
                            pic_player2.BackColor = Color.Lime;
                            listBox1.Items.Add(tokens[2] + " 님이 준비하였습니다.");
                        }
                        else if (tokens[2] == player3.Text)
                        {
                            pic_player3.BackColor = Color.Lime;
                            listBox1.Items.Add(tokens[2] + " 님이 준비하였습니다.");
                        }
                        else if (tokens[2] == player4.Text)
                        {
                            pic_player4.BackColor = Color.Lime;
                            listBox1.Items.Add(tokens[2] + " 님이 준비하였습니다.");
                        }
                    }
                    else if (tokens[1] == "wait")
                    {
                        if (tokens[2] == player2.Text)
                        {
                            pic_player2.BackColor = Color.LightGray;
                            listBox1.Items.Add(tokens[2] + " 님이 준비를 취소하였습니다.");
                        }
                        else if (tokens[2] == player3.Text)
                        {
                            pic_player3.BackColor = Color.LightGray;
                            listBox1.Items.Add(tokens[2] + " 님이 준비를 취소하였습니다.");
                        }
                        else if (tokens[2] == player4.Text)
                        {
                            pic_player4.BackColor = Color.LightGray;
                            listBox1.Items.Add(tokens[2] + " 님이 준비를 취소하였습니다.");
                        }
                    }
                    else if (tokens[1] == "repost")
                    {
                        if (tokens[3] == "True")
                        {
                            if (tokens[2] == player1.Text)
                                pic_player1.BackColor = Color.Lime;
                            else if (tokens[2] == player2.Text)
                                pic_player2.BackColor = Color.Lime;
                            else if (tokens[2] == player3.Text)
                                pic_player3.BackColor = Color.Lime;
                            else if (tokens[2] == player4.Text)
                                pic_player4.BackColor = Color.Lime;
                        }
                        else if (tokens[3] == "False")
                        {
                            if (tokens[2] == player1.Text)
                                pic_player1.BackColor = Color.Transparent;
                            else if (tokens[2] == player2.Text)
                                pic_player2.BackColor = Color.Transparent;
                            else if (tokens[2] == player3.Text)
                                pic_player3.BackColor = Color.Transparent;
                            else if (tokens[2] == player4.Text)
                                pic_player4.BackColor = Color.Transparent;
                        }
                    }
                }

                //"start" + '\x01' + "게임을 시작합니다." + '\x01' + turn.ToString()
                //            + '\x01' + str1 + '\x01' + str2 + '\x01' + str3 + '\x01' + str4 + '\x01'
                else if (ip == "start")
                {
                    starting = true;
                    listBox1.Items.Add(tokens[1]);

                    // 플레이어 정보 설정
                    int count = list_nick.Count;

                    if (count == 2)
                    {
                        players.Add(player1.Text, new Player(player1.Text));
                        players.Add(player2.Text, new Player(player2.Text));
                    }
                    else if (count == 3)
                    {
                        players.Add(player1.Text, new Player(player1.Text));
                        players.Add(player2.Text, new Player(player2.Text));
                        players.Add(player3.Text, new Player(player3.Text));
                    }
                    else if (count == 4)
                    {
                        players.Add(player1.Text, new Player(player1.Text));
                        players.Add(player2.Text, new Player(player2.Text));
                        players.Add(player3.Text, new Player(player3.Text));
                        players.Add(player4.Text, new Player(player4.Text));
                    }

                    // 토큰 설정
                    SetTokens();
                    foreach (string nick in list_nick)
                        SetOthersTokens(nick);

                    // 카드 덱 설정
                    string[] kard1 = tokens[3].Split(':');  //cnt:40, 0~39
                    string[] kard2 = tokens[4].Split(':');  //cnt:30, 40~69
                    string[] kard3 = tokens[5].Split(':');  //cnt:20, 70~89

                    int j = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        field_Cards[i] = list_Cards1[int.Parse(kard1[j])];
                        arr_img[i] = int.Parse(kard1[j]);

                        field_Cards[i + 4] = list_Cards2[int.Parse(kard2[j])];
                        arr_img[i + 4] = int.Parse(kard2[j]) + 40;

                        field_Cards[i + 8] = list_Cards3[int.Parse(kard3[j])];
                        arr_img[i + 8] = int.Parse(kard3[j]) + 70;

                        field_Cards[i + 12] = list_Cards4[j];
                        arr_img[i + 12] = j + 90;

                        j += 1;
                    }

                    Play_Sound(6);
                    for (int i = 0; i < 15; i++)
                        Fill_Card(i);

                    list_Cards1.RemoveRange(int.Parse(kard1[0]), 4);
                    list_Cards2.RemoveRange(int.Parse(kard2[0]), 4);
                    list_Cards3.RemoveRange(int.Parse(kard3[0]), 4);
                    list_Cards4.RemoveRange(0, 4);

                    // 턴 설정
                    selectpos = -1;
                    turn = int.Parse(tokens[2]) % list_nick.Count;
                    listBox1.Items.Add(String.Format("--{0} 님의 차례--", list_nick[turn]));
                    SetTurnColors();
                }

                // "take" + '\x01' + "토큰을 가져갔습니다." + '\x01' + turn.ToString()
                // +'\x01' + tokens[2] + '\x01' + tokens[4] + '\x01'
                else if (ip == "take")
                {
                    TakeTokens(tokens[3], tokens[4]);
                    SetTokens();

                    listBox1.Items.Add(tokens[4] + " 님이 " + tokens[3] + " " + tokens[1]);
                    SetOthersTokens(tokens[4]);


                    selectpos = -1;
                    turn = int.Parse(tokens[2]) % list_nick.Count;
                    listBox1.Items.Add(String.Format("--{0} 님의 차례--", list_nick[turn]));
                    SetTurnColors();
                }

                //"buy" + '\x01' + "카드를 구매하였습니다." + '\x01' + turn.ToString()
                // selectpos + '\x01' + selectinfo + '\x01' + nickname + '\x01' + pick.ToString() + '\x01');
                else if (ip == "buy")
                {
                    BuyCards(tokens[4], tokens[5]);
                    SetTokens();

                    listBox1.Items.Add(tokens[5] + " 님이 " + tokens[4] + " " + tokens[1]);
                    SetOthersTokens(tokens[5]);

                    // 선택 위치 카드 재설정
                    int pos = int.Parse(tokens[3]);
                    if (pos < 4)
                    {
                        field_Cards[pos] = list_Cards1[int.Parse(tokens[6])];
                        list_Cards1.RemoveAt(int.Parse(tokens[6]));
                        arr_img[pos] = int.Parse(tokens[6]);
                    }
                    else if (pos < 8)
                    {
                        field_Cards[pos] = list_Cards2[int.Parse(tokens[6])];
                        list_Cards2.RemoveAt(int.Parse(tokens[6]));
                        arr_img[pos] = int.Parse(tokens[6]) + 40;
                    }
                    else if (pos < 12)
                    {
                        field_Cards[pos] = list_Cards3[int.Parse(tokens[6])];
                        list_Cards3.RemoveAt(int.Parse(tokens[6]));
                        arr_img[pos] = int.Parse(tokens[6]) + 70;
                    }
                    else if (pos < 16)
                    {
                        field_Cards[pos] = null;
                        arr_img[pos] = -1;
                    }
                    else if (pos < 19)
                    {
                        players[nickname].keep_cards.RemoveAt(pos - 16);
                    }

                    Fill_Card(pos);

                    // 포인트 확인
                    if (players[tokens[5]].point >= 2/*15*/)
                    {
                        if (nickname == tokens[5])
                        {
                            byte[] bDts = Encoding.UTF8.GetBytes("victory" + '\x01' + turn.ToString() + '\x01' + roomcode + '\x01' + nickname + '\x01');
                            mainSock.Send(bDts);
                        }

                        obj.ClearBuffer();
                        obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
                        return;
                    }

                    // 턴 설정
                    selectpos = -1;
                    turn = int.Parse(tokens[2]) % list_nick.Count;
                    listBox1.Items.Add(String.Format("--{0} 님의 차례--", list_nick[turn]));
                    SetTurnColors();
                }

                //"keep" + '\x01' + "카드를 보관하였습니다." + '\x01' + turn.ToString() +'\x01' 
                //+ tokens[2] + '\x01' + tokens[3] + '\x01' + tokens[5] + '\x01' + pick.ToString() + '\x01'
                else if (ip == "keep")
                {
                    KeepCards(tokens[4], tokens[5]);
                    gold.Text = pub_jewel[5].ToString();
                    gd.Text = players[nickname].jewel[5].ToString();

                    listBox1.Items.Add(tokens[5] + " 님이 " + tokens[4] + " " + tokens[1]);
                    SetOthersTokens(tokens[5]);

                    // 선택 위치 카드 재설정
                    int pos = int.Parse(tokens[3]);

                    if (nickname == tokens[5])
                    {
                        Move_Card(pos);  //카드 보관
                        pos_k++;
                    }

                    if (pos < 4)
                    {
                        field_Cards[pos] = list_Cards1[int.Parse(tokens[6])];
                        list_Cards1.RemoveAt(int.Parse(tokens[6]));
                        arr_img[pos] = int.Parse(tokens[6]);
                    }
                    else if (pos < 8)
                    {
                        field_Cards[pos] = list_Cards2[int.Parse(tokens[6])];
                        list_Cards2.RemoveAt(int.Parse(tokens[6]));
                        arr_img[pos] = int.Parse(tokens[6]) + 40;
                    }
                    else if (pos < 12)
                    {
                        field_Cards[pos] = list_Cards3[int.Parse(tokens[6])];
                        list_Cards3.RemoveAt(int.Parse(tokens[6]));
                        arr_img[pos] = int.Parse(tokens[6]) + 70;
                    }

                    Fill_Card(pos);

                    selectpos = -1;
                    turn = int.Parse(tokens[2]) % list_nick.Count;
                    listBox1.Items.Add(String.Format("--{0} 님의 차례--", list_nick[turn]));
                    SetTurnColors();
                }

                //"victory" + '\x01' + "승리하였습니다." + '\x01' + turn.ToString()
                //+'\x01' + tokens[3] + '\x01'
                else if (ip == "victory")
                {
                    listBox1.Items.Add(string.Format("{0} 님이 {1}. (소모 턴: {2})", tokens[3], tokens[1], tokens[2]));
                    selectTokens.Text = string.Format("{0} 님이 {1}!", tokens[3], tokens[1]);
                    selectTokens.Visible = true;
                }
                else if (ip == "out")
                {
                    if (tokens[1] == player1.Text)
                    {
                        player1.Text = "Player1";
                        listBox1.Items.Add(tokens[1] + " 님이 퇴장하셨습니다.");
                    }
                    else if (tokens[1] == player2.Text)
                    {
                        player1.Text = "Player2";
                        listBox1.Items.Add(tokens[1] + " 님이 퇴장하셨습니다.");
                    }
                    else if (tokens[1] == player3.Text)
                    {
                        player1.Text = "Player3";
                        listBox1.Items.Add(tokens[1] + " 님이 퇴장하셨습니다.");
                    }
                    else if (tokens[1] == player4.Text)
                    {
                        player1.Text = "Player4";
                        listBox1.Items.Add(tokens[1] + " 님이 퇴장하셨습니다.");
                    }

                    players.Remove(tokens[1]);
                    list_nick.Remove(tokens[1]);

                    // 방장 바꿈
                    byte[] bDts = Encoding.UTF8.GetBytes("hostchange" + '\x01' + roomcode + '\x01' + list_nick.Count.ToString() + '\x01');
                    mainSock.Send(bDts);
                }

                // "hostchange" + '\x01' + nt.ToString() + '\x01'
                else if (ip == "hostchange")
                {
                    int nexthost = int.Parse(tokens[1]);
                    if (nickname == list_nick[nexthost])
                        who = 0;

                    try
                    {
                        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                        {
                            mysql.Open();
                            string insertQuery = string.Format("UPDATE splendor_info SET host=0 WHERE roomcode='{0}' AND nickname='{1}'", roomcode, list_nick[nexthost]);

                            MySqlCommand command = new MySqlCommand(insertQuery, mysql);
                            command.ExecuteReader();
                            mysql.Close();
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                    // 프로필 설정
                    int count = list_nick.Count;

                    if (count == 1)
                    {
                        player1.Text = list_nick[0];
                    }
                    if (count == 2)
                    {
                        player1.Text = list_nick[0];
                        player2.Text = list_nick[1];
                    }
                    if (count == 3)
                    {
                        player1.Text = list_nick[0];
                        player2.Text = list_nick[1];
                        player3.Text = list_nick[2];
                    }
                    if (count == 4)
                    {
                        player1.Text = list_nick[0];
                        player2.Text = list_nick[1];
                        player3.Text = list_nick[2];
                        player4.Text = list_nick[3];
                    }

                    // 재설정
                    byte[] bDts = Encoding.UTF8.GetBytes("state" + '\x01' + "repost" + '\x01' + roomcode + '\x01' + nickname +
                        '\x01' + state.ToString() + '\x01' + tokens[1] + '\x01');
                    mainSock.Send(bDts);
                }

                else if (ip == "comeout")
                {
                    if (tokens[1] == player1.Text)
                    {
                        player1.Text = "Player1";
                        listBox1.Items.Add(tokens[1] + " 님이 퇴장하셨습니다.");
                    }
                    else if (tokens[1] == player2.Text)
                    {
                        player1.Text = "Player2";
                        listBox1.Items.Add(tokens[1] + " 님이 퇴장하셨습니다.");
                    }
                    else if (tokens[1] == player3.Text)
                    {
                        player1.Text = "Player3";
                        listBox1.Items.Add(tokens[1] + " 님이 퇴장하셨습니다.");
                    }
                    else if (tokens[1] == player4.Text)
                    {
                        player1.Text = "Player4";
                        listBox1.Items.Add(tokens[1] + " 님이 퇴장하셨습니다.");
                    }

                    players.Remove(tokens[1]);
                    list_nick.Remove(tokens[1]);

                    // 프로필 설정
                    int count = list_nick.Count;

                    if (count == 1)
                    {
                        player1.Text = list_nick[0];
                    }
                    if (count == 2)
                    {
                        player1.Text = list_nick[0];
                        player2.Text = list_nick[1];
                    }
                    if (count == 3)
                    {
                        player1.Text = list_nick[0];
                        player2.Text = list_nick[1];
                        player3.Text = list_nick[2];
                    }
                    if (count == 4)
                    {
                        player1.Text = list_nick[0];
                        player2.Text = list_nick[1];
                        player3.Text = list_nick[2];
                        player4.Text = list_nick[3];
                    }

                    foreach (string nick in list_nick)
                        SetOthersTokens(nick);
                    SetTurnColors();
                }
                
                else if (ip == "emot")
                {
                    //MessageBox.Show("emot : " + tokens[2] + "," + tokens[3]);
                    Display_Emoticon(tokens[2], tokens[3]);
                }

                obj.ClearBuffer();

                // 수신 대기
                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
            }
            catch //(Exception ex)
            {
                //
                return;
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mainSock.IsBound)
            {
                mainSock.Close();
                return;
            }

            if (MessageBox.Show("정말 퇴장하시겠습니까?", "게임 퇴장",
                MessageBoxButtons.YesNo) == DialogResult.Yes)

            {
                byte[] bDts = Encoding.UTF8.GetBytes("out" + '\x01' + roomcode + '\x01' + starting.ToString() + '\x01' + nickname + '\x01');
                mainSock.Send(bDts);
                try
                {
                    using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                    {
                        mysql.Open();
                        string deleteQuery = string.Format("DELETE FROM splendor_info WHERE nickname='{0}'", nickname);
                        MySqlCommand command = new MySqlCommand(deleteQuery, mysql);
                        command.ExecuteReader();
                        mysql.Close();

                        mysql.Open();
                        string updateQuery = string.Format("UPDATE splendor_roomcode SET count=count-1 WHERE roomcode='{0}'", roomcode);
                        command = new MySqlCommand(updateQuery, mysql);
                        command.ExecuteReader();
                        mysql.Close();

                        mysql.Open();
                        deleteQuery = string.Format("DELETE FROM splendor_roomcode WHERE count='0'");
                        command = new MySqlCommand(deleteQuery, mysql);
                        command.ExecuteReader();
                        mysql.Close();
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

                mainSock.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void gameout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void start_Click_1(object sender, EventArgs e)
        {
            if (who == 0)
            {
                int count = list_nick.Count;
                if (count == 4 && pic_player2.BackColor == Color.Lime && pic_player3.BackColor == Color.Lime && pic_player4.BackColor == Color.Lime)
                {
                    byte[] bDts = Encoding.UTF8.GetBytes("state" + '\x01' + "ready" + '\x01' + roomcode + '\x01' + nickname + '\x01');
                    mainSock.Send(bDts);
                    bDts = Encoding.UTF8.GetBytes("start" + '\x01' + roomcode + '\x01' + count.ToString() + '\x01'
                                + list_Cards1.Count.ToString() + '\x01' + list_Cards2.Count.ToString() + '\x01' + list_Cards3.Count.ToString() + '\x01' + list_Cards4.Count.ToString() + '\x01');
                    mainSock.Send(bDts);
                }
                else if (count == 3 && pic_player2.BackColor == Color.Lime && pic_player3.BackColor == Color.Lime)
                {
                    byte[] bDts = Encoding.UTF8.GetBytes("state" + '\x01' + "ready" + '\x01' + roomcode + '\x01' + nickname + '\x01');
                    mainSock.Send(bDts);
                    bDts = Encoding.UTF8.GetBytes("start" + '\x01' + roomcode + '\x01' + count.ToString() + '\x01'
                                + list_Cards1.Count.ToString() + '\x01' + list_Cards2.Count.ToString() + '\x01' + list_Cards3.Count.ToString() + '\x01' + list_Cards4.Count.ToString() + '\x01');
                    mainSock.Send(bDts);
                }
                else if (count == 2 && pic_player2.BackColor == Color.Lime)
                {
                    byte[] bDts = Encoding.UTF8.GetBytes("state" + '\x01' + "ready" + '\x01' + roomcode + '\x01' + nickname + '\x01');
                    mainSock.Send(bDts);
                    bDts = Encoding.UTF8.GetBytes("start" + '\x01' + roomcode + '\x01' + count.ToString() + '\x01'
                                + list_Cards1.Count.ToString() + '\x01' + list_Cards2.Count.ToString() + '\x01' + list_Cards3.Count.ToString() + '\x01' + list_Cards4.Count.ToString() + '\x01');
                    mainSock.Send(bDts);
                }
                else
                    MessageBox.Show("참가자 전원이 준비상태일 때 게임을 시작할 수 있습니다.");
            }
            else if (!starting)
            {
                state = !state;

                if (state)
                {
                    start.Text = "준비취소";
                    byte[] bDts = Encoding.UTF8.GetBytes("state" + '\x01' + "ready" + '\x01' + roomcode + '\x01' + nickname + '\x01');
                    mainSock.Send(bDts);
                }
                else
                {
                    start.Text = "준비하기";
                    byte[] bDts = Encoding.UTF8.GetBytes("state" + '\x01' + "wait" + '\x01' + roomcode + '\x01' + nickname + '\x01');
                    mainSock.Send(bDts);
                }
            }
        }

        private void SetVisible(Boolean BuynKeep, Boolean OknBack)
        {
            buy.Visible = BuynKeep;
            stick.Visible = BuynKeep;
            keep.Visible = BuynKeep;

            ok.Visible = OknBack;
            stick2.Visible = OknBack;
            back.Visible = OknBack;

            buy.Enabled = BuynKeep;
            keep.Enabled = BuynKeep;

            ok.Enabled = OknBack;
            back.Enabled = OknBack;

            selectTokens.Visible = OknBack;
        }

        private void st0_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[0];
            selectpos = 0;
        }

        private void st1_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[1];
            selectpos = 1;
        }

        private void st2_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[2];
            selectpos = 2;
        }

        private void st3_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[3];
            selectpos = 3;
        }

        private void st4_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[4];
            selectpos = 4;
        }

        private void st5_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[5];
            selectpos = 5;
        }

        private void st6_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[6];
            selectpos = 6;
        }

        private void st7_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[7];
            selectpos = 7;
        }

        private void st8_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[8];
            selectpos = 8;
        }

        private void st9_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[9];
            selectpos = 9;
        }

        private void st10_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[10];
            selectpos = 10;
        }

        private void st11_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);

            selectinfo = field_Cards[11];
            selectpos = 11;
        }

        private void st12_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);
            keep.Enabled = false;

            selectinfo = field_Cards[12];
            selectpos = 12;
        }

        private void st13_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);
            keep.Enabled = false;

            selectinfo = field_Cards[13];
            selectpos = 13;
        }

        private void st14_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);
            keep.Enabled = false;

            selectinfo = field_Cards[14];
            selectpos = 14;
        }

        private void st15_Click(object sender, EventArgs e)
        {
            Play_Sound(4);

            if (list_nick[turn] != nickname)
                return;

            SetVisible(true, false);
            keep.Enabled = false;

            selectinfo = field_Cards[15];
            selectpos = 15;
        }

        private void keep0_Click(object sender, EventArgs e)
        {
            if (list_nick[turn] != nickname)
                return;
            else if (players[nickname].keep_cards.Count < 1)
                return;

            SetVisible(true, false);
            keep.Enabled = false;

            selectinfo = players[nickname].keep_cards[0];
            selectpos = 16;
        }

        private void keep1_Click(object sender, EventArgs e)
        {
            if (list_nick[turn] != nickname)
                return;
            else if (players[nickname].keep_cards.Count < 2)
                return;

            SetVisible(true, false);
            keep.Enabled = false;

            selectinfo = players[nickname].keep_cards[1];
            selectpos = 17;
        }

        private void keep2_Click(object sender, EventArgs e)
        {
            if (list_nick[turn] != nickname)
                return;
            else if (players[nickname].keep_cards.Count < 3)
                return;

            SetVisible(true, false);
            keep.Enabled = false;

            selectinfo = players[nickname].keep_cards[2];
            selectpos = 18;
        }

        private void buy_Click(object sender, EventArgs e)
        {
            if (list_nick[turn] != nickname)
                return;
            else if (!players[nickname].ExaminJewels(selectinfo))
            {
                listBox1.Items.Add("[!] 토큰이 부족하여 해당 카드를 구매할 수 없습니다.");
                return;
            }

            Play_Sound(5);

            if (selectpos < 4)
            {
                byte[] bDts = Encoding.UTF8.GetBytes("buy" + '\x01' + turn.ToString() + '\x01'
                    + selectpos + '\x01' + selectinfo + '\x01' + roomcode + '\x01' + nickname + '\x01' + list_Cards1.Count.ToString() + '\x01');
                mainSock.Send(bDts);
            }
            else if (selectpos < 8)
            {
                byte[] bDts = Encoding.UTF8.GetBytes("buy" + '\x01' + turn.ToString() + '\x01'
                    + selectpos + '\x01' + selectinfo + '\x01' + roomcode + '\x01' + nickname + '\x01' + list_Cards2.Count.ToString() + '\x01');
                mainSock.Send(bDts);
            }
            else if (selectpos < 12)
            {
                byte[] bDts = Encoding.UTF8.GetBytes("buy" + '\x01' + turn.ToString() + '\x01'
                    + selectpos + '\x01' + selectinfo + '\x01' + roomcode + '\x01' + nickname + '\x01' + list_Cards3.Count.ToString() + '\x01');
                mainSock.Send(bDts);
            }
            else if (selectpos < 19) // 귀족 카드도 keep 카드 처럼 소모시킴
            {
                byte[] bDts = Encoding.UTF8.GetBytes("buy" + '\x01' + turn.ToString() + '\x01'
                    + selectpos + '\x01' + selectinfo + '\x01' + roomcode + '\x01' + nickname + '\x01' + "keep" + '\x01');
                mainSock.Send(bDts);
            }

            SetVisible(false, false);
        }

        private void keep_Click(object sender, EventArgs e)
        {
            if (list_nick[turn] != nickname)
                return;
            else if (selectpos >= 16)
            {
                listBox1.Items.Add("[!] 해당 카드는 보관할 수 없습니다.");
                return;
            }
            else if (players[nickname].keep_cards.Count == 3)
            {
                listBox1.Items.Add("[!] 카드 보관함이 꽉 찼습니다. 카드를 구매하여 비울 수 있습니다.");
                return;
            }

            Play_Sound(6);

            string str = "";
            if (selectpos < 4)
                str = list_Cards1.Count.ToString() + '\x01';
            else if (selectpos < 8)
                str = list_Cards2.Count.ToString() + '\x01';
            else if (selectpos < 12)
                str = list_Cards3.Count.ToString() + '\x01';

            byte[] bDts = Encoding.UTF8.GetBytes("keep" + '\x01' + turn.ToString() + '\x01' + selectpos + '\x01' + selectinfo
                + '\x01' + roomcode + '\x01' + nickname + '\x01' + str);
            mainSock.Send(bDts);

            SetVisible(false, false);
        }

        private void label2_Click(object sender, EventArgs e) // ok
        {
            if (list_nick[turn] != nickname)
                return;

            Play_Sound(5);

            byte[] bDts = Encoding.UTF8.GetBytes("take" + '\x01' + turn.ToString() + '\x01' + selectTokens.Text.ToString() + '\x01' + roomcode + '\x01' + nickname + '\x01');
            mainSock.Send(bDts);

            SetVisible(false, false);

            list_tokens.Clear();
        }

        private void label3_Click(object sender, EventArgs e) // back
        {
            if (list_nick[turn] != nickname)
                return;

            SetVisible(false, false);

            list_tokens.Clear();
        }

        List<string> list_tokens = new List<string>();

        private void red_Click(object sender, EventArgs e)
        {
            if (list_nick[turn] != nickname)
                return;

            selectpos = -1;

            if (list_tokens.Count == 3)
            {
                listBox1.Items.Add("[!] 토큰은 3개까지 선택 가능합니다.");
                return;
            }
            else if (pub_jewel[0] <= 0)
            {
                listBox1.Items.Add("[!] 공용 토큰이 부족합니다.");
                return;
            }
            if (list_tokens.Count == 1 && list_tokens[0] == "Red")
            {
                if (pub_jewel[0] >= 4)
                {
                    ;
                }
                else
                {
                    listBox1.Items.Add("[!] 공용 토큰이 4개 이상 있어야만 해당 토큰 2개를 가져갈 수 있습니다.");
                    return;
                }
            }

            // 중복값이 있음
            else if (list_tokens.Count == 2 && list_tokens.Count != list_tokens.Distinct().Count())
            {
                listBox1.Items.Add("[!] 같은 색의 토큰을 선택할 경우 총 2개의 같은 색의 토큰만 가져갈 수 있습니다.");
                return;
            }
            // 중복값이 없음
            else if (list_tokens.Count == 2 && list_tokens.Count == list_tokens.Distinct().Count())
            {
                list_tokens.Add("Red");
                if (list_tokens.Count == 3 && list_tokens.Count == list_tokens.Distinct().Count())
                {
                    list_tokens.RemoveAt(list_tokens.Count - 1);
                }
                else
                {
                    list_tokens.RemoveAt(list_tokens.Count - 1);
                    listBox1.Items.Add("[!] 총 3개의 토큰을 가져가려면 각각 다른 색의 토큰을 선택해야 합니다.");
                    return;
                }
            }

            list_tokens.Add("Red");

            string str = "";
            foreach (string tokens in list_tokens)
                str += tokens + ":";
            selectTokens.Text = str;

            SetVisible(false, true);
        }

        private void green_Click(object sender, EventArgs e)
        {
            if (list_nick[turn] != nickname)
                return;

            selectpos = -1;

            if (list_tokens.Count == 3)
            {
                listBox1.Items.Add("[!] 토큰은 3개까지 선택 가능합니다.");
                return;
            }
            else if (pub_jewel[1] <= 0)
            {
                listBox1.Items.Add("[!] 공용 토큰이 부족합니다.");
                return;
            }

            if (list_tokens.Count == 1 && list_tokens[0] == "Green")
            {
                if (pub_jewel[1] >= 4)
                {
                    ;
                }
                else
                {
                    listBox1.Items.Add("[!] 공용 토큰이 4개 이상 있어야만 해당 토큰 2개를 가져갈 수 있습니다.");
                    return;
                }
            }

            // 중복값이 있음
            else if (list_tokens.Count == 2 && list_tokens.Count != list_tokens.Distinct().Count())
            {
                listBox1.Items.Add("[!] 같은 색의 토큰을 선택할 경우 총 2개의 같은 색의 토큰만 가져갈 수 있습니다.");
                return;
            }
            // 중복값이 없음
            else if (list_tokens.Count == 2 && list_tokens.Count == list_tokens.Distinct().Count())
            {
                list_tokens.Add("Green");
                if (list_tokens.Count == 3 && list_tokens.Count == list_tokens.Distinct().Count())
                {
                    list_tokens.RemoveAt(list_tokens.Count - 1); ;
                }
                else
                {
                    list_tokens.RemoveAt(list_tokens.Count - 1);
                    listBox1.Items.Add("[!] 총 3개의 토큰을 가져가려면 각각 다른 색의 토큰을 선택해야 합니다.");
                    return;
                }
            }

            list_tokens.Add("Green");

            string str = "";
            foreach (string tokens in list_tokens)
                str += tokens + ":";
            selectTokens.Text = str;

            SetVisible(false, true);
        }

        private void blue_Click(object sender, EventArgs e)
        {
            if (list_nick[turn] != nickname)
                return;

            selectpos = -1;

            if (list_tokens.Count == 3)
            {
                listBox1.Items.Add("[!] 토큰은 3개까지 선택 가능합니다.");
                return;
            }
            else if (pub_jewel[2] <= 0)
            {
                listBox1.Items.Add("[!] 공용 토큰이 부족합니다.");
                return;
            }

            if (list_tokens.Count == 1 && list_tokens[0] == "Blue")
            {
                if (pub_jewel[2] >= 4)
                {
                    ;
                }
                else
                {
                    listBox1.Items.Add("[!] 공용 토큰이 4개 이상 있어야만 해당 토큰 2개를 가져갈 수 있습니다.");
                    return;
                }
            }
            // 중복값이 있음
            else if (list_tokens.Count == 2 && list_tokens.Count != list_tokens.Distinct().Count())
            {
                listBox1.Items.Add("[!] 같은 색의 토큰을 선택할 경우 총 2개의 같은 색의 토큰만 가져갈 수 있습니다.");
                return;
            }
            // 중복값이 없음
            else if (list_tokens.Count == 2 && list_tokens.Count == list_tokens.Distinct().Count())
            {
                list_tokens.Add("Blue");
                if (list_tokens.Count == 3 && list_tokens.Count == list_tokens.Distinct().Count())
                {
                    list_tokens.RemoveAt(list_tokens.Count - 1);
                }
                else
                {
                    list_tokens.RemoveAt(list_tokens.Count - 1);
                    listBox1.Items.Add("[!] 총 3개의 토큰을 가져가려면 각각 다른 색의 토큰을 선택해야 합니다.");
                    return;
                }
            }

            list_tokens.Add("Blue");

            string str = "";
            foreach (string tokens in list_tokens)
                str += tokens + ":";
            selectTokens.Text = str;

            SetVisible(false, true);
        }

        private void white_Click(object sender, EventArgs e)
        {
            if (list_nick[turn] != nickname)
                return;

            selectpos = -1;

            if (list_tokens.Count == 3)
            {
                listBox1.Items.Add("[!] 토큰은 3개까지 선택 가능합니다.");
                return;
            }
            else if (pub_jewel[3] <= 0)
            {
                listBox1.Items.Add("[!] 공용 토큰이 부족합니다.");
                return;
            }

            if (list_tokens.Count == 1 && list_tokens[0] == "White")
            {
                if (pub_jewel[3] >= 4)
                {
                    ;
                }
                else
                {
                    listBox1.Items.Add("[!] 공용 토큰이 4개 이상 있어야만 해당 토큰 2개를 가져갈 수 있습니다.");
                    return;
                }
            }
            // 중복값이 있음
            else if (list_tokens.Count == 2 && list_tokens.Count != list_tokens.Distinct().Count())
            {
                listBox1.Items.Add("[!] 같은 색의 토큰을 선택할 경우 총 2개의 같은 색의 토큰만 가져갈 수 있습니다.");
                return;
            }
            // 중복값이 없음
            else if (list_tokens.Count == 2 && list_tokens.Count == list_tokens.Distinct().Count())
            {
                list_tokens.Add("White");
                if (list_tokens.Count == 3 && list_tokens.Count == list_tokens.Distinct().Count())
                {
                    list_tokens.RemoveAt(list_tokens.Count - 1);
                }
                else
                {
                    list_tokens.RemoveAt(list_tokens.Count - 1);
                    listBox1.Items.Add("[!] 총 3개의 토큰을 가져가려면 각각 다른 색의 토큰을 선택해야 합니다.");
                    return;
                }
            }

            list_tokens.Add("White");

            string str = "";
            foreach (string tokens in list_tokens)
                str += tokens + ":";
            selectTokens.Text = str;

            SetVisible(false, true);
        }

        private void black_Click(object sender, EventArgs e)
        {
            if (list_nick[turn] != nickname)
                return;

            selectpos = -1;

            if (list_tokens.Count == 3)
            {
                listBox1.Items.Add("[!] 토큰은 3개까지 선택 가능합니다.");
                return;
            }
            else if (pub_jewel[4] <= 0)
            {
                listBox1.Items.Add("[!] 공용 토큰이 부족합니다.");
                return;
            }

            if (list_tokens.Count == 1 && list_tokens[0] == "Black")
            {
                if (pub_jewel[4] >= 4)
                {
                    ;
                }
                else
                {
                    listBox1.Items.Add("[!] 공용 토큰이 4개 이상 있어야만 해당 토큰 2개를 가져갈 수 있습니다.");
                    return;
                }
            }
            // 중복값이 있음
            if (list_tokens.Count == 2 && list_tokens.Count != list_tokens.Distinct().Count())
            {
                listBox1.Items.Add("[!] 같은 색의 토큰을 선택할 경우 총 2개의 같은 색의 토큰만 가져갈 수 있습니다.");
                return;
            }
            // 중복값이 없음
            else if (list_tokens.Count == 2 && list_tokens.Count == list_tokens.Distinct().Count())
            {
                list_tokens.Add("Black");
                if (list_tokens.Count == 3 && list_tokens.Count == list_tokens.Distinct().Count())
                {
                    list_tokens.RemoveAt(list_tokens.Count - 1);
                }
                else
                {
                    list_tokens.RemoveAt(list_tokens.Count - 1);
                    listBox1.Items.Add("[!] 총 3개의 토큰을 가져가려면 각각 다른 색의 토큰을 선택해야 합니다.");
                    return;
                }
            }

            list_tokens.Add("Black");

            string str = "";
            foreach (string tokens in list_tokens)
                str += tokens + ":";
            selectTokens.Text = str;

            SetVisible(false, true);
        }

        private void gold_Click(object sender, EventArgs e)
        {

        }

        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (list_nick[turn] != nickname)
                return;

            selectpos = -1;
            selectTokens.Visible = false;
            SetVisible(false, false);

            list_tokens.Clear();
        }

        private void selectTokens_Click(object sender, EventArgs e)
        {

        }

        private void stick2_Click(object sender, EventArgs e)
        {

        }

        private void stick_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox17_Click(object sender, EventArgs e)
        {

        }

        private void buy1_Click(object sender, EventArgs e)
        {

        }

        private void gd_Click(object sender, EventArgs e)
        {

        }

        private void bk_Click(object sender, EventArgs e)
        {

        }

        private void per_bk_Click(object sender, EventArgs e)
        {

        }

        private void wt_Click(object sender, EventArgs e)
        {

        }

        private void per_wt_Click(object sender, EventArgs e)
        {

        }

        private void be_Click(object sender, EventArgs e)
        {

        }

        private void per_be_Click(object sender, EventArgs e)
        {

        }

        private void gn_Click(object sender, EventArgs e)
        {

        }

        private void per_gn_Click(object sender, EventArgs e)
        {

        }

        private void rd_Click(object sender, EventArgs e)
        {

        }

        private void per_rd_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox18_Click(object sender, EventArgs e)
        {

        }

        private void buy3_Click(object sender, EventArgs e)
        {

        }

        private void buy2_Click(object sender, EventArgs e)
        {

        }

        private void buy0_Click(object sender, EventArgs e)
        {

        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {
            Pen myPen = new Pen(Color.LightGray, 2);
            Rectangle myRectangle;

            switch (selectpos)
            {
                case 0:
                    Graphics g = e.Graphics;
                    myRectangle = new Rectangle(st0.Location.X, st0.Location.Y, st0.Width, st0.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 1:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st1.Location.X, st1.Location.Y, st1.Width, st1.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 2:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st2.Location.X, st2.Location.Y, st2.Width, st2.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 3:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st3.Location.X, st3.Location.Y, st3.Width, st3.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 4:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st4.Location.X, st4.Location.Y, st4.Width, st4.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 5:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st5.Location.X, st5.Location.Y, st5.Width, st5.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 6:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st6.Location.X, st6.Location.Y, st6.Width, st6.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 7:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st7.Location.X, st7.Location.Y, st7.Width, st7.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 8:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st8.Location.X, st8.Location.Y, st8.Width, st8.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 9:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st9.Location.X, st9.Location.Y, st9.Width, st9.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 10:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st10.Location.X, st10.Location.Y, st10.Width, st10.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 11:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st11.Location.X, st11.Location.Y, st11.Width, st11.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 12:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st12.Location.X, st12.Location.Y, st12.Width, st12.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 13:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st13.Location.X, st13.Location.Y, st13.Width, st13.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 14:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st14.Location.X, st14.Location.Y, st14.Width, st14.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 15:
                    g = e.Graphics;
                    myRectangle = new Rectangle(st15.Location.X, st15.Location.Y, st15.Width, st15.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 16:
                    g = e.Graphics;
                    myRectangle = new Rectangle(keep0.Location.X, keep0.Location.Y, keep0.Width, keep0.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 17:
                    g = e.Graphics;
                    myRectangle = new Rectangle(keep1.Location.X, keep1.Location.Y, keep1.Width, keep1.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
                case 18:
                    g = e.Graphics;
                    myRectangle = new Rectangle(keep2.Location.X, keep2.Location.Y, keep2.Width, keep2.Height);
                    g.DrawRectangle(myPen, myRectangle);
                    break;
            }
        }

        private void pr1pt_Click(object sender, EventArgs e)
        {

        }

        private void emoticon1_Click(object sender, EventArgs e)
        {
            Play_Sound(7);
            Display_Emoticon(nickname, "1");

            byte[] bDts = Encoding.UTF8.GetBytes("emot" + '\x01' + roomcode + '\x01' + nickname + '\x01' + "1" + '\x01');
            mainSock.Send(bDts);
        }

        private void emoticon2_Click(object sender, EventArgs e)
        {
            Play_Sound(7);
            Display_Emoticon(nickname, "2");

            byte[] bDts = Encoding.UTF8.GetBytes("emot" + '\x01' + roomcode + '\x01' + nickname + '\x01' + "2" + '\x01');
            mainSock.Send(bDts);
        }

        private void emoticon3_Click(object sender, EventArgs e)
        {
            Play_Sound(7);
            Display_Emoticon(nickname, "3");

            byte[] bDts = Encoding.UTF8.GetBytes("emot" + '\x01' + roomcode + '\x01' + nickname + '\x01' + "3" + '\x01');
            mainSock.Send(bDts);
        }

        private void emoticon4_Click(object sender, EventArgs e)
        {
            Play_Sound(7);
            Display_Emoticon(nickname, "4");

            byte[] bDts = Encoding.UTF8.GetBytes("emot" + '\x01' + roomcode + '\x01' + nickname + '\x01' + "4" + '\x01');
            mainSock.Send(bDts);
        }

        private void emoticon5_Click(object sender, EventArgs e)
        {
            Play_Sound(7);
            Display_Emoticon(nickname, "5");

            byte[] bDts = Encoding.UTF8.GetBytes("emot" + '\x01' + roomcode + '\x01' + nickname + '\x01' + "5" + '\x01');
            mainSock.Send(bDts);
        }

        private void emoticon6_Click(object sender, EventArgs e)
        {
            Play_Sound(7);
            Display_Emoticon(nickname, "6");

            byte[] bDts = Encoding.UTF8.GetBytes("emot" + '\x01' + roomcode + '\x01' + nickname + '\x01' + "6" + '\x01');
            mainSock.Send(bDts);
        }

        private void emoticon7_Click(object sender, EventArgs e)
        {
            Play_Sound(7);
            Display_Emoticon(nickname, "7");

            byte[] bDts = Encoding.UTF8.GetBytes("emot" + '\x01' + roomcode + '\x01' + nickname + '\x01' + "7" + '\x01');
            mainSock.Send(bDts);
        }

        private void emoticon8_Click(object sender, EventArgs e)
        {
            Play_Sound(7);
            Display_Emoticon(nickname, "8");

            byte[] bDts = Encoding.UTF8.GetBytes("emot" + '\x01' + roomcode + '\x01' + nickname + '\x01' + "8" + '\x01');
            mainSock.Send(bDts);
        }

        private void emoticon9_Click(object sender, EventArgs e)
        {
            Play_Sound(7);
            Display_Emoticon(nickname, "9");

            byte[] bDts = Encoding.UTF8.GetBytes("emot" + '\x01' + roomcode + '\x01' + nickname + '\x01' + "9" + '\x01');
            mainSock.Send(bDts);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            byte[] bDts = Encoding.UTF8.GetBytes("victory" + '\x01' + (turn - 1).ToString() + '\x01' + roomcode + '\x01' + nickname + '\x01');
            mainSock.Send(bDts);
        }
    }
}
