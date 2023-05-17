using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        bool terminating = false;
        bool connected = false;
        Socket clientSocket;
        string channel = "null";

        byte[] publicKey = File.ReadAllBytes("server_enc_dec_pub.txt");
        byte[] server_signature = File.ReadAllBytes("server_sign_verify_pub.txt");
        //byte[] privateKey = File.ReadAllBytes("server_enc_dec_pub_prv.txt");


        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();

            submitButton.Enabled = false;
            auth_button.Enabled = false;
            disconnect_button.Enabled = false;

        }

       
        private void connectButton_Click(object sender, EventArgs e)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string ip = ipText.Text;
            int portNum;

            if (Int32.TryParse(portText.Text, out portNum))
            {
                try
                {
                    clientSocket.Connect(ip, portNum);
                    connectButton.Enabled = false;
                    connected = true;
                    logs.AppendText("Connected to the server. \n");

                    submitButton.Enabled = true;
                    auth_button.Enabled = true;
                    disconnect_button.Enabled = true;
                    Thread receiveThread = new Thread(ReceiveMessage);
                    receiveThread.Start();

                }
                catch
                {
                    logs.AppendText("Could not connect to the server. \n");
                     connectButton.Enabled = true;

                }

            }
            else
            {
                logs.AppendText("Check port & ip number. \n");
                connectButton.Enabled = true;

            }
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            string username = userText.Text;
            string password = passText.Text;
            if (radioButton1.Checked) channel = "IF100";
            else if (radioButton2.Checked) channel = "MATH101";
            else if (radioButton3.Checked) channel = "SPS101";

            password = Encoding.Default.GetString(hashWithSHA512(password));
            string pubString = Encoding.Default.GetString(publicKey);
            string credentials = "username:"+username + "password:" + password + "channel:" + channel;
            byte[] encryptedMsg = encryptWithRSA(credentials, 3072, pubString);

            try
            {
                logs.AppendText("Submit button clicked. Trying to enroll...\n");
                clientSocket.Send(encryptedMsg);
            }
            catch
            {
                logs.AppendText("The encrypted message couldn't be sent\n");
            }

        }

        private void ReceiveMessage()
        {
            while (connected)
            {
                try
                {
                    Byte[] buffer = new Byte[600];
                    clientSocket.Receive(buffer);
                    Byte[] signature_byte = new Byte[384];
                    string message = Encoding.Default.GetString(buffer);
                    message = message.Trim('\0');
                    bool isCmd = true;
                    if (message == "EXIT")
                    {
                        connected = false;
                        terminating = true;
                        connectButton.Enabled = true;
                        disconnect_button.Enabled = false;
                        submitButton.Enabled = false;
                        auth_button.Enabled = false;
                        logs.AppendText("Client Disconnected\n");
                        //Environment.Exit(0);
                        isCmd = false;
                        break;
                    }
                    else if (message.Substring(0,6) == "AUTH1:")
                    {
                        isCmd = false;
                        if (message.Substring(6, 11) == "No username")
                        {
                            logs.AppendText("The username is wrong! Try again\n");
                        }
                        else
                        {
                            string rand = message.Substring(6);
                            string password = passText.Text;
                            Byte[] hashed_pass = hashWithSHA512(password);
                            Byte[] hashed_pass_quarter = new Byte[16];
                            Buffer.BlockCopy(hashed_pass, 0, hashed_pass_quarter, 0, 16);
                            Byte[] hmac_result = applyHMACwithSHA512(rand, hashed_pass_quarter);
                            string auth1_final = "AUTH2:" + Encoding.Default.GetString(hmac_result);
                            Byte[] auth1_final_buffer = Encoding.Default.GetBytes(auth1_final);
                            clientSocket.Send(auth1_final_buffer);
                        }
                    }
                    else if (message.Substring(0, 6) == "AUTH2:")
                    {
                        isCmd = false;
                        message = message.Substring(6);
                        Byte[] hashed_key_aes = new Byte[16];
                        Byte[] hashed_4 = new Byte[16];
                        string password = passText.Text;
                        Byte[] hashed_pass = hashWithSHA512(password);
                        Buffer.BlockCopy(hashed_pass, 0, hashed_key_aes, 0, 16);
                        Buffer.BlockCopy(hashed_pass, 16, hashed_4, 0, 16);
                        Byte[] decrypt = decryptWithAES128(message, hashed_key_aes, hashed_4);
                        string buffer_decrypt_result = Encoding.Default.GetString(decrypt);
                        if (buffer_decrypt_result == "Authentication Successful")
                        {
                            logs.AppendText("Authentication Successful \n");

                        }
                        else
                        {
                            logs.AppendText("Authentication Unsuccessful \n");

                        }
                    }
                    else if (message == "Signup successful")
                    {
                        clientSocket.Receive(signature_byte);
                    }
                    else if (message == "Signup fail")
                    {
                        clientSocket.Receive(signature_byte);
                    }
                    string server_pub = Encoding.Default.GetString(server_signature);
                    
                    if (!isCmd) { //skip
                    }
                    else if (verifyWithRSA(message, 3072, server_pub, signature_byte))
                    {
                        if (message == "Signup successful")
                        {
                            // Username was valid, display success message to user
                            logs.AppendText("You are successfully enrolled!\n");
                           
                        }
                        else if (message == "Signup fail")
                        {
                            // Username was invalid, display error message to user
                            logs.AppendText("This username already exists!\n");
                            // Prompt the user to enter new credentials
                            // (you could add this logic in the submitButton_Click method)
                        }

                    }
                    else
                    {
                        logs.AppendText("Couldn't verify the signed message\n");
                    }
                
   
                }
                catch
                {
                    if (!terminating)
                    {
                        logs.AppendText("Connection has lost with the server. \n");
                        connectButton.Enabled = true;
                        auth_button.Enabled = false;
                        submitButton.Enabled = false;
                        disconnect_button.Enabled = false;
                      
                    }
                    //clientSocket.Close();
                    connected = false;
                }
                
            }

        }

        private void auth_button_Click(object sender, EventArgs e)
        {
            string username = userText.Text;
            string password = passText.Text;
            Byte[] hashed_pass = hashWithSHA512(password);
            Byte[] hashed_pass_quarter = new Byte[16];
            Buffer.BlockCopy(hashed_pass, 0, hashed_pass_quarter, 0, 16);
            string auth_start = "AUTH1:" + username;
            try
            {
                logs.AppendText("Login button clicked. Trying to authenticate...\n");
                clientSocket.Send(Encoding.Default.GetBytes(auth_start));
            }
            catch
            {
                logs.AppendText("Authentication Request Failure\n");
            }
        }

        private void Form1_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            connected = false;
            terminating = true;
            Environment.Exit(0);
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
        static byte[] encryptWithRSA(string input, int algoLength, string xmlStringKey)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlStringKey);
            byte[] result = null;

            try
            {
                //true flag is set to perform direct RSA encryption using OAEP padding
                result = rsaObject.Encrypt(byteInput, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        static byte[] decryptWithAES128(string input, byte[] key, byte[] IV)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);

            // create AES object from System.Security.Cryptography
            RijndaelManaged aesObject = new RijndaelManaged();
            //aesObject.Padding = PaddingMode.PKCS7;
            //aesObject.Padding = PaddingMode.Zeros;
            // since we want to use AES-128
            aesObject.KeySize = 128;
            // block size of AES is 128 bits
            aesObject.BlockSize = 128;
            // mode -> CipherMode.*
            aesObject.Mode = CipherMode.CBC;
            // feedback size should be equal to block size
            // aesObject.FeedbackSize = 128;
            // set the key
            aesObject.Key = key;
            // set the IV
            aesObject.IV = IV;
            // create an encryptor with the settings provided
            ICryptoTransform decryptor = aesObject.CreateDecryptor();
            byte[] result = null;

            try
            {
                result = decryptor.TransformFinalBlock(byteInput, 0, byteInput.Length);
            }
            catch (Exception e) // if encryption fails
            {
                Console.WriteLine(e.Message); // display the cause
            }

            return result;
        }

        static bool verifyWithRSA(string input, int algoLength, string xmlString, byte[] signature)
        {
            // convert input string to byte array
            byte[] byteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            bool result = false;

            try
            {
                result = rsaObject.VerifyData(byteInput, "SHA512", signature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        // HMAC with SHA-512
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

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                logs.AppendText("You chose IF100 channel. Are you sure? \n");
                channel = "IF100";
            }

        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton1.Checked = false;
                radioButton3.Checked = false;
                logs.AppendText("You chose MATH101 channel. Are you sure? \n");
                channel = "MATH101";
            }
        }

        private void radioButton3_CheckedChanged_1(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                logs.AppendText("You chose SPS101 channel. Are you sure? \n");
                channel = "SPS101";
            }

        }

        private void disconnect_button_Click(object sender, EventArgs e)
        {
            clientSocket.Send(Encoding.Default.GetBytes("EXIT"));
            connected = false;
            terminating = true;
            connectButton.Enabled = true;
            auth_button.Enabled = false;
            submitButton.Enabled = false;
            
        }
    }
}