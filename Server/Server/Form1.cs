using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        bool terminating = false;
        bool listening = false;

        byte[] privateKey = File.ReadAllBytes("server_enc_dec_pub_prv.txt");
        byte[] signature = File.ReadAllBytes("server_sign_verify_pub_prv.txt");

        string filePath = @"data/credentials.txt";

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> clients = new List<Socket>();
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();

            /*if (!File.Exists(filePath)) // Check if the file already exists
            {
                // If the file doesn't exist, create it
                using (FileStream fs = File.Create(filePath))
                {
                    // Leave the stream open to allow further file operations
                }
            }*/
        }


        private void listenButton_Click(object sender, EventArgs e)
        {
            int Port;
            Thread acceptThread;

            if (Int32.TryParse(portText.Text, out Port))
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Any,Port));
                serverSocket.Listen(5);

                listening = true;
                listenButton.Enabled = false;
                acceptThread = new Thread(new ThreadStart(Accept));
                acceptThread.Start();

                logs.AppendText("Started listening on port: " + Port + "\n");
            }
            else
            {
                logs.AppendText("Please check port number \n");
            }

        }

        private void Accept()
        {
            while (listening)
            {
                try
                {
                    Socket newClient = serverSocket.Accept();
                    clients.Add(newClient);
                    logs.AppendText("A client is connected. \n");

                    Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage(newClient));
                    recieveThread.Start();
                }
                catch
                {
                    if (terminating)
                    {
                        listening = false;
                    }
                    else
                    {
                        logs.AppendText("Socket has stopped working. \n");
                    }
                }
            }
        }

        private void ReceiveMessage(Socket newClient)
        {
            Socket s = newClient;
            bool connected = true;

            while (!terminating && connected)
            {
                try
                {
                    byte[] buffer = new byte[600];
                    s.Receive(buffer);

                    string message = Encoding.Default.GetString(buffer);
                    string privString = Encoding.Default.GetString(privateKey);
                    byte[] encrypted = decryptWithRSA(message, 3072, privString);
                    string credentials = Encoding.Default.GetString(encrypted);

                    string[] crArray = credentials.Split(' ');
                    string _username = crArray[0];

                    string messageToSend;
                    string[] lines = File.ReadAllLines(filePath);
                    buffer = null;
                    if (lines.Length == 0)
                    {
                        messageToSend = "success";
                        buffer = Encoding.Default.GetBytes(messageToSend);
                        signWithRSA(messageToSend, 3072, privString);
                        s.Send(buffer);

                        WriteCredentialsToFile(credentials);
                    }
                    else
                    {
                        foreach (string line in lines)
                        {
                            string[] parts = line.Split(' ');
                            string username = parts[0];
                            string password = parts[1];
                            string channel = parts[2];

                            if (username == _username)
                            {
                                messageToSend = "error";
                                buffer = Encoding.Default.GetBytes(messageToSend);
                                signWithRSA(messageToSend, 3072, privString);
                                s.Send(buffer);
                                break;
                            }                       
                        }
                        messageToSend = "success";
                        buffer = Encoding.Default.GetBytes(messageToSend);
                        signWithRSA(messageToSend, 3072, privString);
                        s.Send(buffer);

                        WriteCredentialsToFile(credentials);
                    }
                    
                }
                catch
                {
                    if (!terminating)
                    {
                        logs.AppendText("A client is disconnected. \n");
                    }

                    s.Close();
                    clients.Remove(s);
                    connected = false;
                }
            }
        }

        private void WriteCredentialsToFile(string message)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(message);
            }
        }

        static byte[] decryptWithRSA(string input, int algoLength, string xmlStringKey)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlStringKey);
            rsaObject.KeySize = 3072;
            byte[] result = null;

            try
            {
                result = rsaObject.Decrypt(byteInput, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        static string generateHexStringFromByteArray(byte[] input)
        {
            string hexString = BitConverter.ToString(input);
            return hexString.Replace("-", "");
        }
        // signing with RSA
        static byte[] signWithRSA(string input, int algoLength, string xmlString)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            byte[] result = null;

            try
            {
                result = rsaObject.SignData(byteInput, "SHA256");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }
        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listening = false;
            terminating = true;
            Environment.Exit(0);
        }


    }
}
