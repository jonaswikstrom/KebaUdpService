using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace KebaUdpService
{
    public class KebaConnector : IKebaConnector, IDisposable
    {
        private const string Report1Command = "report 1";
        private const string Report2Command = "report 2";
        private const string Report3Command = "report 3";
        private const string InformationCommand = "i";

        private readonly string ip;
        private readonly int port;

        private readonly UdpClient sender;
        private readonly UdpClient listener;
        private readonly ILogger logger;
        private bool connected;
        private readonly Timer report3Timer;
        private readonly Timer report2Timer;

        public event EventHandler<string> ValueReceived;

        public KebaConnector(ILoggerFactory loggerFactory, string ip, int port)
        {
            logger = loggerFactory.CreateLogger(GetType());

            report3Timer = new Timer(30000);
            report3Timer.Elapsed += (o, args) =>
            {
                Send(Report3Command);
            };

            report2Timer = new Timer(60000);
            report2Timer.Elapsed += (o, args) =>
            {
                Send(Report2Command);
            };

            this.ip = ip;
            this.port = port;

            sender = new UdpClient();
            listener = new UdpClient(port);
        }

        public void Connect()
        {
            logger.LogInformation($"Connecting KEBA on {ip}:{port}");
            sender.Connect(IPAddress.Parse(ip), port);
            logger.LogInformation("Connected");

            connected = true;

            Task.Run(async () =>
            {
                while (connected)
                {
                    var receivedResults = await listener.ReceiveAsync();
                    var message = Encoding.ASCII.GetString(receivedResults.Buffer);

                    logger.LogTrace($"Message received: {message}");

                    ValueReceived?.Invoke(this, message);

                }

                logger.LogInformation("Connection closed.");
            });

            Send(InformationCommand);

            Send(Report2Command);
            Send(Report3Command);

            report3Timer.Start();
            report2Timer.Start();
        }

        private int Send(string message)
        {
            if (!connected)
                return 0;

            logger.LogTrace($"Sending message: {message}");

            var byteData = Encoding.ASCII.GetBytes(message);
            return sender.Send(byteData, byteData.Length);
        }

        public void Close()
        {
            report2Timer.Stop();
            report3Timer.Stop();

            connected = false;
            logger.LogInformation("Closing connection...");
        }

        public void Dispose()
        {
            Close();

            listener.Close();
            listener.Dispose();
            sender.Close();
            sender.Dispose();
        }
    }


}