using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using UnityEngine;
using System.Diagnostics;

namespace Haptikos.Exoskeleton.CommunicationLayer
{
    /// <summary>
    /// UDP Reciever Class
    /// 
    /// This class is responsible for recieveing the device data from a specific port in the local network utilizing the UDP protocol.
    /// </summary>
    public class UDPReceiver
    {
        int port;

        UdpClient udpClient;

        IPEndPoint IPEndPoint;

        public Stopwatch stopwatch;

        public void InitializeConnection()
        {
            udpClient = new UdpClient(port + 2);

            const int SIO_UDP_CONNRESET = -1744830452;
            udpClient.Client.IOControl((IOControlCode)SIO_UDP_CONNRESET, new byte[] { 0, 0, 0, 0 }, null);

            IPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            udpClient.Connect(IPEndPoint);
            stopwatch = new();
            stopwatch.Restart();
        }

        public void StopConnection()
        {

            udpClient.Close();
            udpClient.Dispose();
        }

        public string GetData()
        {
            string finalMessage = string.Empty;

            //blocks the execution
            byte[] message = udpClient.Receive(ref IPEndPoint);
            stopwatch.Restart();

            if (message != null)
                finalMessage = Encoding.ASCII.GetString(message);

            return finalMessage;
        }

        public void SendHapticData(string jointName)
        {
            byte[] message = Encoding.Default.GetBytes(jointName);
            try
            {
                udpClient.Send(message, message.Length);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
            }
        }

        public UDPReceiver(int port)
        {
            this.port = port;
        }
    }
}