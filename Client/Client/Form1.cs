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


        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            InitializeComponent();

            submitButton.Enabled = false;
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
            if (radioButton1.Checked) channel = "IF100";
            else if (radioButton2.Checked) channel = "MATH101";
            else if (radioButton3.Checked) channel = "SPS101";

            password = Encoding.Default.GetString(hashWithSHA512(password));
            string pubString = Encoding.Default.GetString(publicKey);

            string credentials = "username:"+username + "password:" + password + "channel:" + channel;
            byte[] encryptedMsg = encryptWithRSA(credentials, 3072, pubString);


            try
            {
                clientSocket.Send(encryptedMsg);
                logs.AppendText("Submit button clicked. Trying to enroll...\n");
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start();
                
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
                    Byte[] buffer = new Byte[64];
                    Byte[] signature_byte = new byte[384]; 
                    clientSocket.Receive(buffer);
                    clientSocket.Receive(signature_byte);
                    string server_pub = Encoding.Default.GetString(server_signature);

                    string message = Encoding.Default.GetString(buffer);
                    message = message.Substring(0, message.IndexOf("\0"));
    

                    string pubString = Encoding.Default.GetString(publicKey);

                    if (verifyWithRSA(message, 3072,server_pub ,signature_byte))
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

        static Byte[] hashWithSHA512(string input)
        {
            // convert input string to Byte array
            Byte[] ByteInput = Encoding.Default.GetBytes(input);
            // create a hasher object from System.Security.Cryptography
            SHA512CryptoServiceProvider sha512Hasher = new SHA512CryptoServiceProvider();
            // hash and save the resulting Byte array
            Byte[] result = sha512Hasher.ComputeHash(ByteInput);

            return result;
        }
        static Byte[] encryptWithRSA(string input, int algoLength, string xmlStringKey)
        {
            // convert input string to Byte array
            Byte[] ByteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlStringKey);
            Byte[] result = null;

            try
            {
                //true flag is set to perform direct RSA encryption using OAEP padding
                result = rsaObject.Encrypt(ByteInput, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        static bool verifyWithRSA(string input, int algoLength, string xmlString, Byte[] signature)
        {
            // convert input string to Byte array
            Byte[] ByteInput = Encoding.Default.GetBytes(input);
            // create RSA object from System.Security.Cryptography
            RSACryptoServiceProvider rsaObject = new RSACryptoServiceProvider(algoLength);
            // set RSA object with xml string
            rsaObject.FromXmlString(xmlString);
            bool result = false;

            try
            {
                result = rsaObject.VerifyData(ByteInput, "SHA512", signature);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

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
    }
}