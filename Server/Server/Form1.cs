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

//SERVER LAST VERSION

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

                    Thread recieveThread = new Thread(() => ReceiveMessage(newClient));
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

        private string SearchUsernameInDB(string username_recieved, bool isLogin) //islogin = true auth, islogin = false signup
        {
            string[] lines = File.ReadAllLines(filePath);
            string msg = "";
            if (isLogin == false) //signup 
            {
                if (lines.Length == 0 ||lines[0] == "" ){
                    msg = "Signup successful: " + username_recieved + "\n";
                    logs.AppendText(msg);
                    return msg; 
                 }
                foreach (string line in lines)
                {
                    String[] delimiters = { "username:", " password:", " channel:" };
                    string[] parts = line.Split(delimiters, StringSplitOptions.None);
                    string username = parts[1];
                    string password = parts[2];
                    string channel = parts[3];

                    if (username == username_recieved)
                    {
                        msg = "Signup fail username already exists: " + username_recieved + "\n";
                        logs.AppendText(msg);
                        return msg;
                    }                       
                }
                msg = "Signup successful: " + username_recieved + "\n";
                logs.AppendText(msg);
                return msg;
            
            }
            else //login
            {
                if (lines.Length == 0 ||lines[0] == "" ){
                    msg = "No such username: " + username_recieved + "\n";
                    logs.AppendText(msg);
                    return msg; 
                }
                foreach (string line in lines)
                {
                    String[] delimiters = { "username:", " password:", " channel:" };
                    string[] parts = line.Split(delimiters, StringSplitOptions.None);
                    string username = parts[1];
                    string password = parts[2];
                    string channel = parts[3];

                    if (username == username_recieved)
                    {
                        msg = "username: " + username_recieved + "passsword:" + password "channel:" + channel+ "\n";
                        logs.AppendText(msg);
                        return msg;
                    }                       
                }
                msg = "No such username: " + username_recieved + "\n";
                logs.AppendText(msg);
                return msg; 
            }
            return msg;
        }

        private void ReceiveMessage(Socket newClient)
        {

            bool connected = true;

            while (!terminating && connected)
            {
                try
                {
                    byte[] buffer = new byte[384];
                    newClient.Receive(buffer);

                    string message = Encoding.Default.GetString(buffer);
                    string privString = Encoding.Default.GetString(privateKey);
                    string server_signature = Encoding.Default.GetString(signature);

                    if (message.Substring(0,5) == "AUTH:")
                    {
                        string login_p1 = SearchUsernameInDB(message.Substring(6), true);
                        if (login_p1.Substring(0,2) == "No") //doesnt exists the username
                        {

                        }
                        else
                        {
                            byte[] randomNumber = new byte[16];
                            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                            {
                                rng.GetBytes(randomNumber);
                            }
                            newClient.Send(randomNumber);

                            //Byte[] hashed_pass = hashWithSHA512(password);
                            Byte[] hashed_pass_quarter = new Byte[16];
                            //Buffer.BlockCopy(hashed_pass, 0, hashed_pass_quarter, 0, 16);

                            byte[] buffer_hmac = new byte[384];
                            newClient.Receive(buffer_hmac);
                            string buffer_hmac_s = Encoding.Default.GetString(buffer_hmac);
                            //hmac verify
                            //encrypt in aes 128 
                            //newClient.Send(null); //send ok or no auth
                            }
                    }
                    else
                    {
                        byte[] encrypted = decryptWithRSA(message, 3072, privString);
                        string credentials = Encoding.Default.GetString(encrypted);
           
                        int username_f_index = credentials.IndexOf("username:");
                        int username_length = "username:".Length;
                        int password_f_index = credentials.IndexOf("password:");
                        int password_length = "password:".Length;
                        int channel_f_index = credentials.IndexOf("channel:");
                        int channel_length = "channel:".Length;
                        string _username = credentials.Substring(username_f_index + username_length, (password_f_index - (username_f_index + username_length)));
                        string _password = credentials.Substring(password_f_index + password_length, (channel_f_index - (password_f_index + password_length)));
                        string _channel = credentials.Substring(channel_f_index + channel_length, (credentials.Length - (channel_f_index + channel_length)));

                        string messageToSend = SearchUsernameInDB(_username, false);
                        if (messageToSend.Substring(0, 18) == "Signup successful:")
                            WriteCredentialsToFile(credentials);

                        buffer = Encoding.Default.GetBytes(messageToSend);
                        newClient.Send(buffer);
                        Byte[] buffer_sign = signWithRSA(messageToSend, 3072, server_signature);
                        newClient.Send(buffer_sign);
                        logs.AppendText("Sent the success message signed\n");  
                    }
                }
                catch
                {
                    if (!terminating)
                    {
                        logs.AppendText("A client is disconnected. \n");
                    }

                    newClient.Close();
                    clients.Remove(newClient);
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

                static byte[] applyHMACwithSHA512(string input, byte[] key)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create HMAC applier object from System.Security.Cryptography
            HMACSHA512 hmacSHA512 = new HMACSHA512(key);
            // get the result of HMAC operation
            byte[] result = hmacSHA512.ComputeHash(byteInput);

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
                result = rsaObject.SignData(byteInput, "SHA512");
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
