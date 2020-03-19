using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    static class Program
    {
        static void Main(string[] args)
        {
            //args = new string[]
            //{
            //    "-server",
            //    "-port",
            //    "23333"
            //};
            if (args.Length > 0)
            {
                if (args[0] == "-server")
                    if (args[1] == "-port")
                        if (!string.IsNullOrEmpty(args[2])){
                            var port = int.Parse(args[2]);

                            var server = new Server.GameServer(port);
                            server.Start();
                        }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Client.LoginForm());
            }
        }
    }
}
