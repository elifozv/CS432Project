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
        string channel;

        byte[] publicKey = File.ReadAllBytes("server_enc_dec_pub.txt");
        byte[] signature = File.ReadAllBytes("server_sign_verify_pub.txt");

        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();

            submitButton.Enabled = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                logs.AppendText("You chose IF100 channel. Are you sure? \n");
                channel = "IF100";
            }

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                logs.AppendText("You chose MATH101 channel. Are you sure? \n");
                channel = "MATH101";
            }

        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                logs.AppendText("You chose SPS101 channel. Are you sure? \n");
                channel = "SPS101";
            }

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

            password = Encoding.Default.GetString(hashWithSHA512(password));
            string pubString = Encoding.Default.GetString(publicKey);
            string credentials = username + " " + password + " " + channel;
            byte[] encryptedMsg = encryptWithRSA(credentials, 3072, pubString);

            clientSocket.Send(encryptedMsg);

            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();

        }

        private void ReceiveMessage()
        {
            while (connected)
            {
                try
                {
                    Byte[] buffer = new Byte[64];
                    clientSocket.Receive(buffer);

                    string message = Encoding.Default.GetString(buffer);
                    message = message.Substring(0, message.IndexOf("\0"));
    

                    string pubString = Encoding.Default.GetString(publicKey);

                    if (verifyWithRSA(message, 3072, pubString, signature))
                    {
                        if (message == "success")
                        {
                            // Username was valid, display success message to user
                            logs.AppendText("You are successfully enrolled!\n");
                            submitButton.Enabled = false;
                            radioButton1.Enabled = false;
                            radioButton2.Enabled = false;
                            radioButton3.Enabled = false;
                        }
                        else if (message == "error")
                        {
                            // Username was invalid, display error message to user
                            logs.AppendText("This username already exists!\n");
                            // Prompt the user to enter new credentials
                            // (you could add this logic in the submitButton_Click method)
                        }

                    }
                }
                catch
                {
                    if (!terminating)
                    {
                        logs.AppendText("Connection has lost with the server. \n");
                        connectButton.Enabled = true;

                    }

                    //clientSocket.Close();
                    connected = false;
                }
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
                result = rsaObject.VerifyData(byteInput, "SHA256", signature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }


    }
}