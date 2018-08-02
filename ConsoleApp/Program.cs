using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using ADONetHelper.SqlServer;

namespace ConsoleApp
{
    class Program
    {
        static bool IsDebug = false;
        static bool IsWaitForClose = false;
        static bool IsWaitCompleted = false;
        static int nIntervelTime = 3600000;
        static SqlServerClient cDB;

        [DllImport("kernel32")]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        private delegate bool ConsoleEventDelegate(CtrlType eventType);

        static void onStart()
        {
            // "User ID=" + sUser + ";Initial Catalog=" + sName + ";Data Source=" + sDataSource + ";Password=" + sPassword;
            //cDB = new SqlServerClient("");
        }
        static void onUpdate()
        {

            // top 1  sZIPFileName,nTransID", "sStatus='3' or sStatus='5' Order by nTransID
        }
        static void onStop()
        {

        }
        static void Main(string[] args)
        {
            string sConfigFile = null;
            FileINI cConfig = null;

            try
            {
                for (var i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--debug": IsDebug = true; break;
                        case "--config":
                            if (i + 1 > args.Length) throw new Exception("configuration not found path.");
                            sConfigFile = args[i + 1].Replace("\"", "");
                            break;
                    }
                }
                if (!File.Exists(sConfigFile)) throw new Exception("configuration not extsts path.");

                cConfig = new FileINI(sConfigFile);

                Log("sConfig: {0}", sConfigFile);
                Log("IsDebug: {0}", IsDebug);

                ConsoleEventDelegate handler = new ConsoleEventDelegate(ConsoleCloseEventCallback);
                SetConsoleCtrlHandler(handler, true);

                Log("DNS Resolve...");
                var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect("10.0.80.59", 21);

                Log("Program Starting...");
                Program.onStart();
                do
                {
                    Program.onUpdate();
                    Thread.Sleep(Program.nIntervelTime);
                    if (!IsWaitCompleted) continue;

                    Log("Program Stoping...");
                    Program.onStop();
                    if (IsWaitCompleted) break;
                } while (true);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
        static void Log(string msg, object arg)
        {
            Log(msg, new object[] { arg });
        }
        static void Log(string msg = null, object[] arg = null)
        {
            if (IsDebug)
            {
                if (msg != null)
                {
                    if (arg == null) Console.WriteLine(" " + msg); else Console.WriteLine(" " + msg, arg);
                }
                else
                {
                    Console.WriteLine("");
                }
            }
        }
        static void Error(Exception ex)
        {
            if (IsDebug)
            {
                Log("---------------------------------------------------------------------");
                if (ex.Source != null) Log(ex.Source);
                Log(ex.Message);
                Log("---------------------------------------------------------------------");
                Log("--> Next, please any key.");
            }
        }

        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        static bool ConsoleCloseEventCallback(CtrlType eventType)
        {
            Console.WriteLine("Interrup Services closing, death imminent [" + eventType.ToString() + "]");
            IsWaitCompleted = true;
            return true;
        }


    }
}
