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
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.ObjectModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


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

            disconnect_button.Enabled = false;
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
                disconnect_button.Enabled = true;
                const string connectionUri = "mongodb+srv://digdeci:6HJ!H$d5vhsMDCp@cluster0.uftj1g6.mongodb.net/?retryWrites=true&w=majority";
                var settings = MongoClientSettings.FromConnectionString(connectionUri);
                // Set the ServerApi field of the settings object to Stable API version 1
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);
                // Create a new client and connect to the server
                var client = new MongoClient(settings);
                // Send a ping to confirm a successful connection
                try
                {
                    var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                    Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                acceptThread = new Thread(() => Accept(client));
                acceptThread.Start();

                logs.AppendText("Started listening on port: " + Port + "\n");
            }
            else
            {
                logs.AppendText("Please check port number \n");
            }

        }

        private void Accept(MongoClient client)
        {
            while (listening)
            {
                try
                {
                    Socket newClient = serverSocket.Accept();
                    clients.Add(newClient);
                    logs.AppendText("A client is connected. \n");

                    Thread recieveThread = new Thread(() => ReceiveMessage(newClient,client));
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

        public static byte[] GenerateRandomNumber()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            int byteCount = 16;
            byte[] randomNumber = new byte[byteCount];
            rng.GetBytes(randomNumber);

            return randomNumber;
        }


        private string SearchUsernameInDB(IMongoCollection<BsonDocument> credentials, string username_recieved) //islogin = true auth, islogin = false signup
        {
            //This fucntion is for login
            var documents = credentials.Find(new BsonDocument()).ToEnumerable();
            var count = credentials.CountDocuments(new BsonDocument());
            if (count == 0)
            {
                //database boş
                //login fail mesajı gönder
                string msg = "No such username: " + username_recieved + "\n";
                logs.AppendText(msg);
                return msg;
            }
            else
            {
                foreach (var document in documents)
                {
                    string username;
                    string password;
                    string channel;
                    if (document != null)
                    {
                        username = document.GetValue("username").ToString();
                        password = document.GetValue("password").ToString();
                        channel = document.GetValue("channel").ToString();
                        if (username == username_recieved)
                        {
                            //user found return credentials
                            string msggg = "username: " + username_recieved + "password:" + password + "channel:" + channel + "\n";
                            logs.AppendText(msggg);
                            return msggg;
                        }
                    }
                }
                //user not found
                string msg = "No such username found\n";
                logs.AppendText(msg);
                return msg;
            }               
            
        }

        private string CreateUserForDB(IMongoCollection<BsonDocument> credentials, string username,string password,string channel)
        {
            //This function is for signup
            var documents = credentials.Find(new BsonDocument()).ToEnumerable();
            var count = credentials.CountDocuments(new BsonDocument());
            if (count == 0)
            {
                //database boş
                //yeni bir document aç ve database'e pushla
                var document = new BsonDocument
                {
                    {"username",username },
                    {"password",password },
                    {"channel",channel }
                };
                credentials.InsertOne(document);
                string msg = "Signup successful: " + username + "\n";
                logs.AppendText(msg);
                return msg;
            }
            else
            {
                foreach (var document in documents)
                {
                    string _username;
                    string _password;
                    string _channel;
                    if (document != null)
                    {
                        _username = document.GetValue("username").ToString();
                        _password = document.GetValue("password").ToString();
                        _channel = document.GetValue("channel").ToString();
                        if (_username == username)
                        {
                            //same username found
                            string msgg = "Signup fail username already exists: " + _username + "\n";
                            logs.AppendText(msgg);
                            return msgg;
                        }
                    }
                }
                // add the user to database
                var document2 = new BsonDocument
                {
                    {"username",username },
                    {"password",password },
                    {"channel",channel }
                };
                credentials.InsertOne(document2);
                string msg = "Signup successful: " + username + "\n";
                logs.AppendText(msg);
                return msg;
            }
        }
        private void ReceiveMessage(Socket newClient,MongoClient client)
        {

            bool connected = true;
            int username_f_index;
            int username_length;
            int password_f_index;
            int password_length;
            int channel_f_index;
            int channel_length;
            string _username;
            string _password="";
            string _channel ="";
            Byte[] randomNumber = new Byte[16];
            while (!terminating && connected)
            {
                try
                {
                    var database = client.GetDatabase("CS432_Database");
                    var credentials = database.GetCollection<BsonDocument>("Credentials");

        
                    byte[] buffer = new byte[384];
                    newClient.Receive(buffer);
                    string message = Encoding.Default.GetString(buffer);
                    message = message.Trim('\0');
                    string privString = Encoding.Default.GetString(privateKey);
                    string server_signature = Encoding.Default.GetString(signature);
                    if (message.Substring(0, 4) == "EXIT")
                    {
                        newClient.Send(Encoding.Default.GetBytes("EXIT"));
                        newClient.Close();
                        clients.Remove(newClient);
                    }
                    else if (message.Substring(0, 6) == "AUTH1:")//Login kısmı
                    {
                        string login_p1 = SearchUsernameInDB(credentials, message.Substring(6));
                        if (login_p1.Substring(0, 2) == "No") //doesnt exists the username
                        {
                            string response = "AUTH1:No username";
                            Byte[] no_username_resp = Encoding.Default.GetBytes(response);
                            newClient.Send(no_username_resp);
                        }
                        else
                        {
                            username_f_index = login_p1.IndexOf("username:");
                            username_length = "username:".Length;
                            password_f_index = login_p1.IndexOf("password:");
                            password_length = "password:".Length;
                            channel_f_index = login_p1.IndexOf("channel:");
                            channel_length = "channel:".Length;
                            _username = login_p1.Substring(username_f_index + username_length, (password_f_index - (username_f_index + username_length)));
                            _password = login_p1.Substring(password_f_index + password_length, (channel_f_index - (password_f_index + password_length)));
                            _channel = login_p1.Substring(channel_f_index + channel_length, (login_p1.Length - (channel_f_index + channel_length)));

                            randomNumber = GenerateRandomNumber();
                            string auth1_random_s = "AUTH1:" + Encoding.Default.GetString(randomNumber);
                            Byte[] auth1_random_b = Encoding.Default.GetBytes(auth1_random_s);
                            newClient.Send(auth1_random_b);
                            logs.AppendText(Encoding.Default.GetString(randomNumber));
                        }
                    }
                    else if (message.Substring(0, 6) == "AUTH2:") {
                        Byte[] hashed_pass = Encoding.Default.GetBytes(_password);
                        Byte[] hashed_pass_quarter = new Byte[16];
                        Buffer.BlockCopy(hashed_pass, 0, hashed_pass_quarter, 0, 16);
                        string rand_num = Encoding.Default.GetString(randomNumber);
                        Byte[] hmac_result = applyHMACwithSHA512(rand_num, hashed_pass_quarter);
                        string hmac_string_r = "AUTH2:" + Encoding.Default.GetString(hmac_result);
                        string hmac_buffer_r = Encoding.Default.GetString(buffer);
                        logs.AppendText("buffer hmac recieved \n");
                        if (hmac_string_r == hmac_buffer_r)
                        {
                            //ok
                            string auth_result = "Authentication Successful \n";
                            Byte[] hashed_key_aes = new Byte[16];
                            Byte[] hashed_4 = new Byte[16];
                            Buffer.BlockCopy(hashed_pass, 0, hashed_key_aes, 0, 16);
                            Buffer.BlockCopy(hashed_pass, 16, hashed_4, 0, 16);
                            Byte[] encrpyt_aes128 = encryptWithAES128(auth_result, hashed_key_aes, hashed_4);
                            logs.AppendText(auth_result);
                            string auth2_result = "AUTH2:" + Encoding.Default.GetString(encrpyt_aes128);
                            Byte[] auth2_result_b = Encoding.Default.GetBytes(auth2_result);
                            newClient.Send(auth2_result_b);
                        }
                        else
                        {
                            //no 
                            string auth_result = "Authentication Unsuccessful \n";
                            Byte[] hashed_key_aes = new Byte[16];
                            Byte[] hashed_4 = new Byte[16];
                            Buffer.BlockCopy(hashed_pass, 0, hashed_key_aes, 0, 16);
                            Buffer.BlockCopy(hashed_pass, 16, hashed_4, 0, 16);
                            Byte[] encrpyt_aes128 = encryptWithAES128(auth_result, hashed_key_aes, hashed_4);
                            logs.AppendText(auth_result);
                            string auth2_result = "AUTH2:" + Encoding.Default.GetString(encrpyt_aes128);
                            Byte[] auth2_result_b = Encoding.Default.GetBytes(auth2_result);
                            newClient.Send(auth2_result_b);
                        }

                    }
                    else
                    {
                        // Enrollment side - Signup
                        Byte[] encrypted = decryptWithRSA(message, 3072, privString);
                        string credentials_recieved = Encoding.Default.GetString(encrypted);

                         username_f_index = credentials_recieved.IndexOf("username:");
                         username_length = "username:".Length;
                         password_f_index = credentials_recieved.IndexOf("password:");
                         password_length = "password:".Length;
                         channel_f_index = credentials_recieved.IndexOf("channel:");
                         channel_length = "channel:".Length;
                         _username = credentials_recieved.Substring(username_f_index + username_length, (password_f_index - (username_f_index + username_length)));
                         _password = credentials_recieved.Substring(password_f_index + password_length, (channel_f_index - (password_f_index + password_length)));
                         _channel = credentials_recieved.Substring(channel_f_index + channel_length, (credentials_recieved.Length - (channel_f_index + channel_length)));

                        string messageToSend = CreateUserForDB(credentials,_username,_password,_channel);
                        if (messageToSend.Substring(0, 18) == "Signup successful:")
                        {
                            messageToSend = messageToSend.Substring(0, 17);
                        }
                        else
                        {
                            messageToSend = messageToSend.Substring(0, 11);
                        }
                        buffer = Encoding.Default.GetBytes(messageToSend);
                        Byte[] buffer_sign = signWithRSA(messageToSend, 3072, server_signature);
                        newClient.Send(buffer);
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

                // encryption with AES-128
        static byte[] encryptWithAES128(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            // since we want to use AES-128
            aesObject.KeySize = 128;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CFB;
            // feedback size should be equal to block size
            aesObject.FeedbackSize = 128;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;
            // create an encryptor with the settings provided
            ICryptoTransform encryptor = aesObject.CreateEncryptor();
            byte[] result = null;

            try
            {
                result = encryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {
                Console.WriteLine(e.Message); // display the cause
            }

            return result;
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
         foreach (Socket i in clients)
            {
                i.Send(Encoding.Default.GetBytes("EXIT"));
                i.Close(); //closing all clients before stoping
            }
         clients.Clear();

            serverSocket.Close();
            listening = false;
            terminating = true;
            listenButton.Enabled = true;

            Environment.Exit(0);
        }

        private void disconnect_button_Click(object sender, EventArgs e)
        {            
            
            foreach (Socket i in clients)
            {
                i.Send(Encoding.Default.GetBytes("EXIT"));
                i.Close(); //closing all clients before stoping
            }
            clients.Clear();

            serverSocket.Close();
            listening = false;
            terminating = true;
            listenButton.Enabled = true;
            disconnect_button.Enabled = false;
            logs.AppendText("Server Disconnected \n");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Environment.Exit(0);
        }
    }
}
