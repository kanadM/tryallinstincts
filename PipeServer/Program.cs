using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace PipeServer
{
    class Program
    {
        static void Main()
        {
            ConversationWithTheClient();
        }
        private static void ConversationWithTheClient()
        {
            using (NamedPipeServerStream namedPipeServer = new NamedPipeServerStream("test-pipe", PipeDirection.InOut,
                1, PipeTransmissionMode.Message))
            {
                Console.WriteLine("Server waiting for a connection...");
                namedPipeServer.WaitForConnection();
                Console.Write("A client has connected, send a greeting from the server: ");
                string message = Console.ReadLine();
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                namedPipeServer.Write(messageBytes, 0, messageBytes.Length);

                string response = ProcessSingleReceivedMessage(namedPipeServer);
                Console.WriteLine("The client has responded: {0}", response);
                while (response != "x")
                {
                    Console.Write("Send a response from the server: ");
                    message = Console.ReadLine();
                    messageBytes = Encoding.UTF8.GetBytes(message);
                    namedPipeServer.Write(messageBytes, 0, messageBytes.Length);
                    response = ProcessSingleReceivedMessage(namedPipeServer);
                    Console.WriteLine("The client is saying {0}", response);
                }

                Console.WriteLine("The client has ended the conversation.");
            }
        }

        private static string ProcessSingleReceivedMessage(NamedPipeServerStream namedPipeServer)
        {
            StringBuilder messageBuilder = new StringBuilder();
            string messageChunk = string.Empty;
            byte[] messageBuffer = new byte[5];
            do
            {
                namedPipeServer.Read(messageBuffer, 0, messageBuffer.Length);
                messageChunk = Encoding.UTF8.GetString(messageBuffer);
                messageBuilder.Append(messageChunk);
                messageBuffer = new byte[messageBuffer.Length];
            }
            while (!namedPipeServer.IsMessageComplete);
            return messageBuilder.ToString();
        }
    }
}
