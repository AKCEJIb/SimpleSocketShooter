using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    static class Program
    {
        private static bool FindArg(string[] args, string arg)
        {
            if (args.Contains(arg))
                return true;

            return false;
        }

        private static T FindArgValue<T>(string[] args, string arg)
        {
            if (args.Contains(arg))
            {
                var indx = Array.FindIndex(args, x => x.Contains(arg));
                if(args.Length > indx)
                {
                    try
                    {
                        var converter = TypeDescriptor.GetConverter(typeof(T));
                        return (T)converter?.ConvertFromString(args[indx + 1]);
                    }
                    catch
                    {
                        return default(T);
                    }
                }
            }
            return default(T);
        }
        static void Main(string[] args)
        {
            //args = new string[]
            //{
            //    "-server1",
            //    "-port"
            //};

            if (args.Length > 0)
            {
                if (FindArg(args, "-server"))
                {

                    var port = FindArgValue<int>(args, "-port");

                    if (port == 0)
                    {
                        Console.WriteLine("Port not specified, using default 23333 port!");
                        port = 23333;
                    }

                    //var ip = Dns.GetHostAddresses(Dns.GetHostName())
                    //    .Where(x => x.AddressFamily == AddressFamily.InterNetwork).
                    //    FirstOrDefault();

                    var server = Server.GameServer.GetInstance();
                    server.SetPort(port);
                    server.Start();
                }
                else
                {
                    Console.WriteLine("No such arguments available!");
                    Console.WriteLine("Press any key...");
                    Console.ReadKey(true);
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Client.ClientForm());
            }
        }
    }
}
