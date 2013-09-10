using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text;

namespace SwitchLight
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            s.Connect("power", 7070);
            string buf = "AVOISYS0";
            Byte[] b = Encoding.ASCII.GetBytes(buf);

            s.Send(b, SocketFlags.None);

            byte[] rcv = new byte[128];
            s.Receive(rcv);

            string r = Encoding.ASCII.GetString(rcv);

            const Byte POS = (1 << 5);
            Byte state = 0;
            System.Byte.TryParse(r.Substring(0, 3), out state);
            state = (byte)(255 & ~state);

            bool light = (0 != (state & POS));
            Byte n = (byte)(light ? (state & ~POS) : (state | POS));

            String cmd = "AVOISYS1" + n.ToString("D3") + "\0\0\0";

            Byte[] c = Encoding.ASCII.GetBytes(cmd);

            s.Send(c);
        }
    }
}
