using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Splendor
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            GameStart gamestart = new GameStart();
            if (gamestart.ShowDialog() == DialogResult.OK)
            {
                Nickname nickname = new Nickname();
                if (nickname.ShowDialog() == DialogResult.OK)
                {
                    Home home = new Home(nickname.textBox1.Text);
                    if (home.ShowDialog() == DialogResult.OK)
                        Application.Run(new Main(home.nickname, home.roomcode, home.roomstate, home.who));
                }
            }
        }
    }
}
