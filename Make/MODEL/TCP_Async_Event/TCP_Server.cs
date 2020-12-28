using Make.MODEL;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Make.MODEL.TCP_Async_Event
{
    public class TCP_Server
    {
        private const Int32 DEFAULT_PORT = 28015, DEFAULT_NUM_CONNECTIONS = 1000, DEFAULT_BUFFER_SIZE = 1024;

        public void Init(String[] args)
        {
            try
            {
                Int32 port = GetValue(args, 0, DEFAULT_PORT);
                Int32 numConnections = GetValue(args, 1, DEFAULT_NUM_CONNECTIONS);
                Int32 bufferSize = GetValue(args, 2, DEFAULT_BUFFER_SIZE);

                SocketListener sl = new SocketListener(numConnections, bufferSize);
                sl.Start(port);

                Console.WriteLine("Server listening on port {0}. Press any key to terminate the server process...", port);
                Console.Read();


            }
            catch (IndexOutOfRangeException)
            {
                PrintUsage();
            }
            catch (FormatException)
            {
                PrintUsage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static Int32 GetValue(String[] args, Int32 index, Int32 defaultValue)
        {
            Int32 value = 0;

            if (args.Length <= index || !Int32.TryParse(args[index], out value))
            {
                return defaultValue;
            }

            return value;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: SocketAsyncServer <port> [numConnections] [bufferSize].");
            Console.WriteLine("\t<port> Numeric value for the listening TCP port.");
            Console.WriteLine("\t[numConnections] Numeric value for the maximum number of incoming connections.");
            Console.WriteLine("\t[bufferSize] Numeric value for the buffer size of incoming connections.");
        }

    }
}
