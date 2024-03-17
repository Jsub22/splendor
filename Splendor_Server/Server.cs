using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace Splendor_Server
{
    public partial class Server : Form
    {
        delegate void AppendTextDelegate(Control ctrl, string s);
        AppendTextDelegate _textAppender;
        Socket mainSock;
        IPAddress thisAddress;

        Dictionary<string, Dictionary<string,Socket>> room = new Dictionary<string, Dictionary<string, Socket>>(); // 방 정보

        #region DB_idpwd
        string _server = "155.230.235.248"; //DB 서버 주소, 로컬일 경우 localhost
        int _port = 34056; //DB 서버 포트
        string _database = "swdb479"; //DB 이름
        string _id = "2019111479"; //계정 아이디
        string _pw = "user@111479"; //계정 비밀번호
        string _connectionAddress = "";
        #endregion

        public Server()
        {
            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _textAppender = new AppendTextDelegate(AppendText);
            _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);
        }

        void AppendText(Control ctrl, string s)
        {
            if (ctrl.InvokeRequired) ctrl.Invoke(_textAppender, ctrl, s);
            else
            {
                string source = ctrl.Text;
                ctrl.Text = source + Environment.NewLine + s;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());

            // 처음으로 발견되는 ipv4 주소를 사용한다.
            foreach (IPAddress addr in he.AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    thisAddress = addr;
                    break;
                }
            }

            // 주소가 없다면..
            if (thisAddress == null)
                // 로컬호스트 주소를 사용한다.
                thisAddress = IPAddress.Loopback;

            try
            {                
                //바인딩 되어있는지 확인
                if (mainSock.IsBound)
                {
                    MessageBox.Show("이미 서버에 연결되어 있습니다!");
                    return;
                }

                // 서버에서 클라이언트의 연결 요청을 대기하기 위해
                // 소켓을 열어둔다.

                IPEndPoint serverEP = new IPEndPoint(thisAddress, 9999);
                mainSock.Bind(serverEP);
                mainSock.Listen(10);

                // 비동기적으로 클라이언트의 연결 요청을 받는다.
                mainSock.BeginAccept(AcceptCallback, null);
                MessageBox.Show("서버가구동 되었습니다!");
            }
            catch
            {
                MessageBox.Show("서버 시작시 오류가 발생하였습니다.");
                return;
            }

            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();

                    string insertQuery = string.Format("INSERT INTO splendor_server(address) VALUES('{0}'); ", thisAddress);
                    //서버 주소 db에 저장
                    MySqlCommand command = new MySqlCommand(insertQuery, mysql);
                    command.ExecuteNonQuery();
                    mysql.Close();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        List<Socket> connectedClients = new List<Socket>();
        void AcceptCallback(IAsyncResult ar)
        {

            try
            {
                // 클라이언트의 연결 요청을 수락한다.
                Socket client = mainSock.EndAccept(ar);

                // 또 다른 클라이언트의 연결을 대기한다.
                mainSock.BeginAccept(AcceptCallback, null);

                AsyncObject obj = new AsyncObject(4096);
                obj.WorkingSocket = client;

                // 연결된 클라이언트 리스트에 추가해준다.
                connectedClients.Add(client);

                // 텍스트박스에 클라이언트가 연결되었다고 써준다.
                listBox1.Items.Add("클라이언트 (@ {" + client.RemoteEndPoint.ToString() + "})가 연결되었습니다.");

                Socket socket = client;
                byte[] bDts = Encoding.UTF8.GetBytes("client" + '\x01' + client.RemoteEndPoint.ToString() + '\x01');
                socket.Send(bDts);
                //연결된 클라이언트에게 본인 클라이언트 번호 넘겨주기

                // 클라이언트의 데이터를 받는다.
                client.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
            }
            catch
            {
                //
                return;
            }
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

                if (ip == "enter")//"enter" + '\x01' + nickname + '\x01' + roomcode + '\x01'
                {
                    Dictionary<string, Socket> clients = new Dictionary<string, Socket>();

                    try
                    {
                        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                        {
                            string[] dbuser = new string[4];
                            mysql.Open();
                            string selectQuery = string.Format("SELECT nickname, client FROM splendor_info WHERE roomcode = '{0}'", tokens[2]);
                            //받은 룸코드에 들어있는 사람들의 클라이언트 번호 찾아서
                            MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                            MySqlDataReader table = command.ExecuteReader();

                            while (table.Read())
                            {
                                for (int k = connectedClients.Count - 1; k >= 0; k--)
                                {
                                    if (connectedClients[k].RemoteEndPoint.ToString() == table[1].ToString())
                                    {//찾은 클라이언트에 00이가 들어왔다 알려주기
                                        //listBox1.Items.Add(connectedClients[k].RemoteEndPoint.ToString());
                                        //listBox1.Items.Add("동일 client 찾기 성공");

                                        clients.Add(table[0].ToString(), connectedClients[k]);

                                        byte[] bDts = Encoding.UTF8.GetBytes("enter" + '\x01' + tokens[1] + '\x01');
                                        clients[table[0].ToString()].Send(bDts);
                                    }
                                }
                            }
                            table.Close();
                            mysql.Close();
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                    // 방 코드 목록 추가함
                    if (room.ContainsKey(tokens[2]) == false)
                    {
                        room.Add(tokens[2], clients);
                    }
                    else
                    {
                        room[tokens[2]] = clients;
                    }
                }
                //"state" + '\x01' + "ready/wait/repost" + '\x01' + roomcode + '\x01' + nickname 
                //+ '\x01' + state.ToString() + '\x01' + tokens[1] + '\x01'
                else if (ip == "state")
                {
                    //listBox1.Items.Add(text);
                    //이전플레이어상태 해당 플레이어에게 전달함
                    if (tokens[1] == "repost")
                    {
                        byte[] bDts = Encoding.UTF8.GetBytes("state" + '\x01' + tokens[1] + '\x01' + tokens[3] + '\x01' + tokens[4] + '\x01');
                        room[tokens[2]][tokens[5]].Send(bDts);
                        //listBox1.Items.Add("state1" + '\x01' + tokens[1] + '\x01' + tokens[3] + '\x01' + tokens[4] + '\x01');
                    }
                    // 대기상태 & 준비상태 전체 전달함
                    else
                    {
                        foreach (Socket socket in room[tokens[2]].Values)
                        {
                            byte[] bDts = Encoding.UTF8.GetBytes("state" + '\x01' + tokens[1] + '\x01' + tokens[3] + '\x01');
                            socket.Send(bDts);
                            listBox1.Items.Add("state2" + '\x01' + tokens[1] + '\x01' + tokens[3] + '\x01');
                        }
                    }
                }

                // 시작함
                else if (ip == "start") // start + '\x01' + roomcode + '\x01' + count.ToString() + '\x01'
                {                       //  + list_kards1 + '\x01' + list_kards2 + '\x01' + list_kards3 + '\x01'
                    listBox1.Items.Add("Server Start Start");

                    int count = int.Parse(tokens[2]);
                    int kards1 = int.Parse(tokens[3]);
                    int kards2 = int.Parse(tokens[4]);
                    int kards3 = int.Parse(tokens[5]);

                    int int1 = 0, int2 = 0, int3 = 0, int4 = 0;
                    string str1="", str2="", str3="", str4 = "";

                    Random random = new Random();

                    int1 = random.Next(0, kards1-3);
                    int2 = random.Next(0, kards2-3);
                    int3 = random.Next(0, kards3-3);

                    for (int i = 0; i < 4; i++)
                    {
                        str1 += int1++.ToString() + ":";
                        str2 += int2++.ToString() + ":";
                        str3 += int3++.ToString() + ":";
                    }

                    int turn = random.Next(0, count);

                    foreach (Socket socket in room[tokens[1]].Values)
                    {
                        byte[] bDts = Encoding.UTF8.GetBytes("start" + '\x01' + "게임을 시작합니다." + '\x01' + turn.ToString() 
                            + '\x01' + str1 + '\x01' + str2 + '\x01' + str3 + '\x01');
                        socket.Send(bDts);
                    }

                    try
                    {
                        using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                        {
                            mysql.Open();
                            string updateQuery = string.Format("UPDATE splendor_roomcode SET gaming='True' WHERE roomcode='{0}';", tokens[1]);
                            MySqlCommand command = new MySqlCommand(updateQuery, mysql);
                            command.ExecuteReader();
                            mysql.Close();
                        }
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                    listBox1.Items.Add("Server Start End, text:" + text);
                }

                // 토큰 가져옴
                // "take" + '\x01' + turn.ToString() + '\x01' + selectTokens.Text.ToString() + '\x01' + roomcode + '\x01' + nickname + '\x01'
                else if (ip == "take") 
                {
                    listBox1.Items.Add("Server Take Start");

                    int turn = int.Parse(tokens[1]) + 1;

                    foreach (Socket socket in room[tokens[3]].Values)
                    {
                        byte[] bDts = Encoding.UTF8.GetBytes("take" + '\x01' + "토큰을 가져갔습니다." + '\x01' + turn.ToString()
                            + '\x01' + tokens[2] + '\x01' + tokens[4] + '\x01');
                        socket.Send(bDts);
                    }

                    listBox1.Items.Add("Server Take End, text:" + text);
                }
                //"buy" + '\x01' + turn.ToString() + '\x01'
                //+ selectpos + '\x01' + selectinfo + '\x01' + roomcode + '\x01' + nickname + '\x01' + str
                else if (ip == "buy")
                {
                    listBox1.Items.Add("Server Buy Start");

                    int turn = int.Parse(tokens[1]) + 1;
                    byte[] bDts = Encoding.UTF8.GetBytes("");

                    if (tokens[6] == "keep")
                    {
                        // Keep 카드는 선택 없이 소모함
                        bDts = Encoding.UTF8.GetBytes("buy" + '\x01' + "카드를 구매하였습니다." + '\x01' + turn.ToString()
                           + '\x01' + tokens[2] + '\x01' + tokens[3] +'\x01' + tokens[5] + '\x01');
                    }
                    else
                    {
                        // 카드 선택함
                        int kards = int.Parse(tokens[6]);

                        Random random = new Random();
                        int pick = random.Next(0, kards);

                        bDts = Encoding.UTF8.GetBytes("buy" + '\x01' + "카드를 구매하였습니다." + '\x01' + turn.ToString()
                           + '\x01' + tokens[2] + '\x01' + tokens[3] +'\x01' + tokens[5] + '\x01' + pick.ToString() + '\x01');
                    }
                    foreach (Socket socket in room[tokens[4]].Values)
                    {
                        socket.Send(bDts);
                    }

                    listBox1.Items.Add("Server Buy End, tokens[6]:" + tokens[6]);
                }
                // "keep" + '\x01' + turn.ToString() + '\x01' + selectpos + '\x01' + selectinfo
                // + '\x01' + roomcode + '\x01' + nickname + '\x01' + str
                else if (ip == "keep")
                {
                    listBox1.Items.Add("Server Keep Start");

                    int turn = int.Parse(tokens[1]) + 1;

                    // 카드 선택함
                    int kards = int.Parse(tokens[6]);

                    Random random = new Random();
                    int pick = random.Next(0, kards);

                    foreach (Socket socket in room[tokens[4]].Values)
                    {
                        byte[] bDts = Encoding.UTF8.GetBytes("keep" + '\x01' + "카드를 보관하였습니다." + '\x01' + turn.ToString()
                            + '\x01' + tokens[2] + '\x01' + tokens[3] + '\x01' + tokens[5] + '\x01' + pick.ToString() + '\x01');
                        socket.Send(bDts);
                    }

                    listBox1.Items.Add("Server Keep End, text:" + text);
                }

                // "victory" + '\x01' + turn.ToString() + '\x01' + roomcode + '\x01' + nickname + '\x01'
                else if (ip == "victory")
                {
                    listBox1.Items.Add("Server Victory Start");

                    int turn = int.Parse(tokens[1]) + 1;

                    byte[] bDts = Encoding.UTF8.GetBytes("victory" + '\x01' + "승리하였습니다" + '\x01' + turn.ToString()
                          + '\x01' + tokens[3] + '\x01');

                    foreach (Socket socket in room[tokens[2]].Values)
                    {
                        socket.Send(bDts);
                    }

                    listBox1.Items.Add("Server Victory End, text:" + text);
                }

                // "hostchange" + '\x01' + roomcode + '\x01' + list_nick.Count.ToString() + '\x01'
                else if (ip == "hostchange")
                {
                    int nexthost = int.Parse(tokens[2]);
                    Random random = new Random();
                    int nt = random.Next(0, nexthost);

                    foreach (Socket socket in room[tokens[1]].Values)
                    {
                        byte[] bDts = Encoding.UTF8.GetBytes("hostchange" + '\x01' + nt.ToString() + '\x01');
                        socket.Send(bDts);
                    }
                }

                // "out" + '\x01' + roomcode + '\x01' + starting.ToString() + '\x01' + nickname + '\x01'
                else if (ip == "out")
                {
                    // 게임 전
                    if (tokens[2] == "True")
                    {
                        foreach (Socket socket in room[tokens[1]].Values)
                        {
                            byte[] bDts = Encoding.UTF8.GetBytes("out" + '\x01' + tokens[3] + '\x01');
                            socket.Send(bDts);
                        }
                    }
                    else // False
                    {
                        // 게임 중
                        foreach (Socket socket in room[tokens[1]].Values)
                        {
                            byte[] bDts = Encoding.UTF8.GetBytes("comeout" + '\x01' + tokens[3] + '\x01');
                            socket.Send(bDts);
                        }
                    }

                    room[tokens[1]].Remove(tokens[3]);
                }
                
                else
                {
                    for (int i = connectedClients.Count - 1; i >= 0; i--)
                    {
                        Socket socket = connectedClients[i];
                        if (socket != obj.WorkingSocket)
                        {
                            try { socket.Send(obj.Buffer); }
                            catch
                            {
                                try { socket.Dispose(); } catch { }
                                connectedClients.RemoveAt(i);
                            }
                        }
                    }
                }

                // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.
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


        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    string selectQuery = string.Format("DELETE FROM splendor_server WHERE address='{0}'", thisAddress);

                    MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                    command.ExecuteNonQuery();
                    mysql.Close();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            mainSock.Close();
        }
    }
}
