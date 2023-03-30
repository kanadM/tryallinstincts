using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace PipeClient
{
    class Program
    { 
        static void Main(string[] args)
        {
            if (args != null)
            {
                Console.WriteLine("args not null");
            }
            if (args.Length != 0)
            {
                Console.WriteLine("args Length not zero");
            }
            if (args.Length == 0)
            {
                Console.WriteLine("args Length is zero");
            }
            Main2();
            Main2(null);
            //ConversationWithTheServer();
            Console.ReadLine();
        }
        static void Main2(params string[] args)
        {
            if (args != null)
            {
                Console.WriteLine("args not null");
            }
            if (args?.Length > 0)
            {
                Console.WriteLine("args Length not zero");
            }
            if (args?.Length == 0)
            {
                Console.WriteLine("args Length is zero");
            }
        }
        private static void ConversationWithTheServer()
        {
            using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream(".", "test-pipe", PipeDirection.InOut))
            {
                namedPipeClient.Connect();
                Console.WriteLine("Client connected to the named pipe server. Waiting for server to send the first message...");
                namedPipeClient.ReadMode = PipeTransmissionMode.Message;
                string messageFromServer = ProcessSingleReceivedMessage(namedPipeClient);
                Console.WriteLine("The server is saying {0}", messageFromServer);
                Console.Write("Write a response: ");
                string response = Console.ReadLine();
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                namedPipeClient.Write(responseBytes, 0, responseBytes.Length);
                while (response != "x")
                {
                    messageFromServer = ProcessSingleReceivedMessage(namedPipeClient);
                    Console.WriteLine("The server is saying {0}", messageFromServer);
                    Console.Write("Write a response: ");
                    response = Console.ReadLine();
                    responseBytes = Encoding.UTF8.GetBytes(response);
                    namedPipeClient.Write(responseBytes, 0, responseBytes.Length);
                }
            }
        }

        private static string ProcessSingleReceivedMessage(NamedPipeClientStream namedPipeClient)
        {
            StringBuilder messageBuilder = new StringBuilder();
            string messageChunk = string.Empty;
            byte[] messageBuffer = new byte[5];
            do
            {
                namedPipeClient.Read(messageBuffer, 0, messageBuffer.Length);
                messageChunk = Encoding.UTF8.GetString(messageBuffer);
                messageBuilder.Append(messageChunk);
                messageBuffer = new byte[messageBuffer.Length];
            }
            while (!namedPipeClient.IsMessageComplete);
            return messageBuilder.ToString();
        }
    }
}
