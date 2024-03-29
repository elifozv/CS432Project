﻿using System;
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
        struct Client
        {
            public string username;
            public string password;
            public string channel;
        }

        bool terminating = false;
        bool listening = false;
        bool connected = false;

        byte[] privateKey = File.ReadAllBytes("server_enc_dec_pub_prv.txt");
        byte[] signature = File.ReadAllBytes("server_sign_verify_pub_prv.txt");

        string filePath = @"data/credentials.txt";

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        List<Socket> clients = new List<Socket>();
        Dictionary<Socket, Client> clientsToItsSocket = new Dictionary<Socket, Client>();
        Byte[] if_channel_aes_key = new Byte[16];
        Byte[] if_channel_4_key = new Byte[16];
        Byte[] if_channel_hmac_key = new Byte[16];
        Byte[] math_channel_aes_key = new Byte[16];
        Byte[] math_channel_4_key = new Byte[16];
        Byte[] math_channel_hmac_key = new Byte[16];
        Byte[] sps_channel_aes_key = new Byte[16];
        Byte[] sps_channel_4_key = new Byte[16];
        Byte[] sps_channel_hmac_key = new Byte[16];


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
                connected = true;
                terminating = false;
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
                    logs.AppendText("Couldn't connect to the database! Maybe check your network settings on MongoDB");
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
                            logs.AppendText("Username: " + username_recieved + "\n\n");
                            logs.AppendText("Password: " + password + "\n\n");
                            logs.AppendText("Channel: " + channel + "\n\n");
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
                        //newClient.Send(Encoding.Default.GetBytes("EXIT"));
                        clients.Remove(newClient);
                        newClient.Close();
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
                            Client user = new Client();
                            user.username = _username;
                            user.password = _password;
                            _channel = _channel.Replace("\n", "");
                            user.channel = _channel;
                            
                            clientsToItsSocket.Add(newClient,user);

                            randomNumber = GenerateRandomNumber();
                            string auth1_random_s = "AUTH1:" + Encoding.Default.GetString(randomNumber);
                            Byte[] auth1_random_b = Encoding.Default.GetBytes(auth1_random_s);
                            newClient.Send(auth1_random_b);
                            logs.AppendText("Random number: " + Encoding.Default.GetString(randomNumber) + "\n\n");
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
                        hmac_buffer_r = hmac_buffer_r.Trim('\0');

                        logs.AppendText("buffer hmac recieved \n\n");

                        if (hmac_string_r == hmac_buffer_r)
                        {
                            //ok
                            string auth_result = "Authentication Successful";
                            Byte[] hashed_key_aes = new Byte[16];
                            Byte[] hashed_4 = new Byte[16];
                            Buffer.BlockCopy(hashed_pass, 0, hashed_key_aes, 0, 16);
                            Buffer.BlockCopy(hashed_pass, 16, hashed_4, 0, 16);
                            Byte[] encrpyt_aes128 = encryptWithAES128(auth_result, hashed_key_aes, hashed_4);
                            string message_to_sign = Encoding.Default.GetString(encrpyt_aes128);
                            string channeltosend = "";
                            string key = "";
                            string key4 = "";
                            string key_hmac = "";
                            foreach (KeyValuePair<Socket, Client> pair in clientsToItsSocket)
                            {
                                Socket mysocket = pair.Key;      // Access the Socket key
                                Client myclient = pair.Value;    // Access the Client value

                                if (mysocket == newClient)
                                {
                                    channeltosend = myclient.channel;
                                    break;
                                }
                            }
                            if (channeltosend == "IF100")
                            {
                                 key = Encoding.Default.GetString(if_channel_aes_key);
                                 key4 = Encoding.Default.GetString(if_channel_4_key);
                                 key_hmac = Encoding.Default.GetString(if_channel_hmac_key);
                            }
                            else if (channeltosend == "MATH101")
                            {
                                key = Encoding.Default.GetString(math_channel_aes_key);
                                key4 = Encoding.Default.GetString(math_channel_4_key);
                                key_hmac = Encoding.Default.GetString(math_channel_hmac_key);
                            }
                            else if (channeltosend == "SPS101")
                            {
                                key = Encoding.Default.GetString(sps_channel_aes_key);
                                key4 = Encoding.Default.GetString(sps_channel_4_key);
                                key_hmac = Encoding.Default.GetString(sps_channel_hmac_key);
                            }
                            key = key.Trim('\0');
                            key4 = key4.Trim('\0');
                            key_hmac = key_hmac.Trim('\0');
                            if (key == null || key == "")
                            {
                                string no_key = "Channel Unavailable";
                                logs.AppendText(no_key + "\n");
                                Byte[] hashed_key_aes_c = new Byte[16];
                                Byte[] hashed_4_c = new Byte[16];
                                Buffer.BlockCopy(hashed_pass, 0, hashed_key_aes_c, 0, 16);
                                Buffer.BlockCopy(hashed_pass, 16, hashed_4_c, 0, 16);
                                Byte[] encrpyt_aes128_c = encryptWithAES128(no_key, hashed_key_aes_c, hashed_4_c);
                                string auth2_result_c = "AUTH2:" + Encoding.Default.GetString(encrpyt_aes128_c);
                                string message_to_sign_c = Encoding.Default.GetString(encrpyt_aes128_c);
                                Byte[] auth2_result_b_c = Encoding.Default.GetBytes(auth2_result_c);
                                Byte[] sign_buffer_c = signWithRSA(message_to_sign_c, 3072, server_signature);
                                newClient.Send(auth2_result_b_c);
                                newClient.Send(sign_buffer_c);
                                logs.AppendText("Sent channel unavailable\n");
                            }
                            else
                            {
                                logs.AppendText(auth_result + "\n");
                                Byte[] encrypted_step2_key = encryptWithAES128(key, hashed_key_aes, hashed_4);
                                Byte[] encrypted_step2_4 = encryptWithAES128(key4, hashed_key_aes, hashed_4);
                                Byte[] encrypted_step2_hmac = encryptWithAES128(key_hmac, hashed_key_aes, hashed_4);


                                string auth2_result = "AUTH2:" + Encoding.Default.GetString(encrpyt_aes128) + Encoding.Default.GetString(encrypted_step2_key) + Encoding.Default.GetString(encrypted_step2_4) + Encoding.Default.GetString(encrypted_step2_hmac);
                                Byte[] auth2_result_b = Encoding.Default.GetBytes(auth2_result);
                                Byte[] sign_buffer = signWithRSA(auth2_result.Substring(6), 3072, server_signature);
                                newClient.Send(auth2_result_b);
                                newClient.Send(sign_buffer);
                                logs.AppendText("Send the successful message signed\n");
                            }

                        }
                        else
                        { 
                            //no SORU: PASSWORD KULLANIYORUZ ANCAK CLİENT YANLIŞ GİRDİ NASIL KONTROL ETMELİYİZ?
                            string auth_result = "Authentication Unsuccessful";
                            Byte[] hashed_key_aes = new Byte[16];
                            Byte[] hashed_4 = new Byte[16];
                            Buffer.BlockCopy(hashed_pass, 0, hashed_key_aes, 0, 16);
                            Buffer.BlockCopy(hashed_pass, 16, hashed_4, 0, 16);
                            Byte[] encrpyt_aes128 = encryptWithAES128(auth_result, hashed_key_aes, hashed_4);
                            logs.AppendText(auth_result + "\n");
                            string auth2_result = "AUTH2:" + Encoding.Default.GetString(encrpyt_aes128);
                            string message_to_sign = Encoding.Default.GetString(encrpyt_aes128);
                            Byte[] auth2_result_b = Encoding.Default.GetBytes(auth2_result);
                            Byte[] sign_buffer = signWithRSA(message_to_sign, 3072, server_signature);
                            newClient.Send(auth2_result_b);
                            newClient.Send(sign_buffer);
                            logs.AppendText("Send the unsuccessful message signed\n");
                        }

                    }
                    else if (message.Substring(0,4) == "MSG:")
                    {
                        string channeltosend = "";
                        foreach (KeyValuePair<Socket, Client> pair in clientsToItsSocket)
                        {
                            Socket mysocket = pair.Key;      // Access the Socket key
                            Client myclient = pair.Value;    // Access the Client value

                            if (mysocket == newClient)
                            {
                                channeltosend = myclient.channel;
                                break;
                            }
                        }
                        logs.AppendText("Broadcast message for:" + channeltosend + "\n");
                        foreach (KeyValuePair<Socket, Client> pair in clientsToItsSocket)
                        {
                            Socket mysocket = pair.Key;      // Access the Socket key
                            Client myclient = pair.Value;    // Access the Client value

                            if (channeltosend == myclient.channel) //mysocket != newClient &&
                            {
                                mysocket.Send(Encoding.Default.GetBytes(message));
                            }
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
                         newClient = serverSocket.Accept();                    }

                    //newClient.Close();
                    //clients.Remove(newClient);
                    //connected = false;
                }
            }
        }

        private void IF100_Click(object sender, EventArgs e)
        {
            string key = text_key.Text;
            Byte[] hashed_key = hashWithSHA512(key);
            Byte[] hashed_key_aes = new Byte[16];
            Byte[] hashed_4 = new Byte[16];
            Byte[] hashed_hmac = new Byte[16];

            Buffer.BlockCopy(hashed_key, 0, hashed_key_aes, 0, 16);
            Buffer.BlockCopy(hashed_key, 16, hashed_4, 0, 16);
            Buffer.BlockCopy(hashed_key, 32, hashed_hmac, 0, 16);

            if_channel_aes_key = hashed_key_aes;
            if_channel_4_key = hashed_4;
            if_channel_hmac_key = hashed_hmac;
            logs.AppendText("Channel key created for IF100\n");

        }

        private void MATH101_Click(object sender, EventArgs e)
        {
            string key = text_key.Text;
            Byte[] hashed_key = hashWithSHA512(key);
            Byte[] hashed_key_aes = new Byte[16];
            Byte[] hashed_4 = new Byte[16];
            Byte[] hashed_hmac = new Byte[16];

            Buffer.BlockCopy(hashed_key, 0, hashed_key_aes, 0, 16);
            Buffer.BlockCopy(hashed_key, 16, hashed_4, 0, 16);
            Buffer.BlockCopy(hashed_key, 32, hashed_hmac, 0, 16);

            math_channel_aes_key = hashed_key_aes;
            math_channel_4_key = hashed_4;
            math_channel_hmac_key = hashed_hmac;
            logs.AppendText("Channel key created for MATH101\n");
        }

        private void SPS101_Click(object sender, EventArgs e)
        {
            string key = text_key.Text;
            Byte[] hashed_key = hashWithSHA512(key);
            Byte[] hashed_key_aes = new Byte[16];
            Byte[] hashed_4 = new Byte[16];
            Byte[] hashed_hmac = new Byte[16];

            Buffer.BlockCopy(hashed_key, 0, hashed_key_aes, 0, 16);
            Buffer.BlockCopy(hashed_key, 16, hashed_4, 0, 16);
            Buffer.BlockCopy(hashed_key, 32, hashed_hmac, 0, 16);

            sps_channel_aes_key = hashed_key_aes;
            sps_channel_4_key = hashed_4;
            sps_channel_hmac_key = hashed_hmac;
            logs.AppendText("Channel key created for SPS101\n");
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
            string r = Encoding.Default.GetString(result);
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
            aesObject.Mode = CipherMode.CBC;
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
            string r = Encoding.Default.GetString(result);

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

        static byte[] hashWithSHA512(string input)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create a hasher object from System.Security.Cryptography
            SHA512CryptoServiceProvider sha512Hasher = new SHA512CryptoServiceProvider();
            // hash and save the resulting byte array
            byte[] result = sha512Hasher.ComputeHash(byteInput);

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
            connected = false;
            listenButton.Enabled = true;
            disconnect_button.Enabled = false;
            logs.AppendText("Server Disconnected \n");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientsToItsSocket.Clear();
            //Environment.Exit(0);
        }

    }
}
