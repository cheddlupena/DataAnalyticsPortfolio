using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrotherConnection
{
    class Request
    {
        public String Command { get; set; }
        public String Arguments { get; set; }

        public String Send()
        {
            using (var client = new TcpClient())
            {
                var connected = false;
                var attempts = 0;
                while (!connected)
                {
                    try
                    {
                        Console.WriteLine("Conneting to CNC Machine - 192.168.0.164:10000");
                        client.Connect("192.168.0.164", 10000);
                        client.NoDelay = true;
                        connected = true;
                        Console.WriteLine(attempts);
                    }
                    catch (SocketException)
                    {
                        attempts++;
                        if (attempts >= 1000)
                        {
                            Console.WriteLine("*********************************************");
                            Console.WriteLine("Failed connecting to Socket 192.168.0.164:10000");
                        }
                        Thread.Sleep(20);
                    }
                }

                client.SendTimeout = 2000;

                                  // Command="LOD"         Arguments = "PDS3"
                var command = "C" + Command.PadRight(7) + Arguments.PadRight(8) + "  \r\n";
                Int32 checksum = 0;
                for (int i = 0; i < command.Length; i++)
                {
                    checksum += command[i];
                }
                checksum = checksum % 16;

                command = String.Format("%{0}\r\n{1:00}%\r\n", command, checksum);

                var result = "";
                using (NetworkStream stream = client.GetStream())
                {
                    var cmdBytes = Encoding.ASCII.GetBytes(command);
                    stream.Write(cmdBytes, 0, cmdBytes.Length);
                    
                    var bytesRead = 0;
                    do
                    {
                        var buffer = new Byte[client.ReceiveBufferSize];
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        result += Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        Console.WriteLine(result);
                    } while (!(result.StartsWith("%") && result.EndsWith("%")));
                    stream.Close();
                }
                client.Close();
                return result; //result = output
            }
        }
    }
}
