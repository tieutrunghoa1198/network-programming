using System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace prn221_network // Note: actual namespace depends on the project name.
{
    class App
    {
        TcpListener tcpListener;
        Socket socketForServer;
        NetworkStream networkStream;
        StreamWriter streamWriter;
        StreamReader streamReader;
        StringBuilder strInput;
        Thread th_StartListen, th_RunClient;
        public App()
        {
            th_StartListen = new Thread(new ThreadStart(StartListen));
            th_StartListen.Start();
        }
        private void StartListen()
        {
            tcpListener = new TcpListener(System.Net.IPAddress.Any, 6666);
            tcpListener.Start();
            for (; ; )
            {
                
                socketForServer = tcpListener.AcceptSocket();
                Console.WriteLine(socketForServer.RemoteEndPoint);
                IPEndPoint ipend = (IPEndPoint)socketForServer.RemoteEndPoint;
                th_RunClient = new Thread(new ThreadStart(RunClient));
                th_RunClient.Start();
            }
        }

        private void RunClient()
        {
            networkStream = new NetworkStream(socketForServer);
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);
            strInput = new StringBuilder();

            while (true)
            {
                try
                {
                    string line;
                while (true)
                {
                    line = "";
                    line = streamReader.ReadLine();
                    // get prefix:['th'] -> send message back to client
                    if (line.LastIndexOf("th") >= 0) {
                        Console.WriteLine(line);
                        String input = Console.ReadLine();
                        strInput.Append(input);
                        streamWriter.WriteLine(strInput);
                        streamWriter.Flush();
                        strInput.Clear();
                    }
                    // detech go command from client side
                    if (line.LastIndexOf("go") >= 0)
                        Console.WriteLine(line);
                    // quit command
                    if (line.LastIndexOf("q") >= 0)
                        Cleanup();
                }//end while
                }
                catch (Exception err)
                {
                    Console.WriteLine(err);
                    Cleanup();
                    break;
                }
                // DisplayMessage(strInput.ToString());
                strInput.Remove(0, strInput.Length);
            }
        }

        private void Cleanup()
        {
            try
            {
                streamReader.Close();
                streamWriter.Close();
                networkStream.Close();
                socketForServer.Close();
            }
            catch (Exception err) { }
           
        }

        private delegate void DisplayDelegate(string message);
    }
}