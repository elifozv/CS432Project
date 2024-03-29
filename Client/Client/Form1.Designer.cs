﻿
namespace Client
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.connectButton = new System.Windows.Forms.Button();
            this.submitButton = new System.Windows.Forms.Button();
            this.ipText = new System.Windows.Forms.TextBox();
            this.portText = new System.Windows.Forms.TextBox();
            this.userText = new System.Windows.Forms.TextBox();
            this.passText = new System.Windows.Forms.TextBox();
            this.logs = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.auth_button = new System.Windows.Forms.Button();
            this.disconnect_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.msg_box = new System.Windows.Forms.TextBox();
            this.send_msg_btn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(196, 269);
            this.connectButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(172, 64);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // submitButton
            // 
            this.submitButton.Location = new System.Drawing.Point(38, 718);
            this.submitButton.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(186, 72);
            this.submitButton.TabIndex = 1;
            this.submitButton.Text = "Sign Up";
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
            // 
            // ipText
            // 
            this.ipText.Location = new System.Drawing.Point(168, 141);
            this.ipText.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.ipText.Name = "ipText";
            this.ipText.Size = new System.Drawing.Size(196, 38);
            this.ipText.TabIndex = 2;
            // 
            // portText
            // 
            this.portText.Location = new System.Drawing.Point(172, 210);
            this.portText.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(196, 38);
            this.portText.TabIndex = 3;
            // 
            // userText
            // 
            this.userText.Location = new System.Drawing.Point(181, 370);
            this.userText.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.userText.Name = "userText";
            this.userText.Size = new System.Drawing.Size(196, 38);
            this.userText.TabIndex = 4;
            // 
            // passText
            // 
            this.passText.Location = new System.Drawing.Point(181, 424);
            this.passText.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.passText.Name = "passText";
            this.passText.Size = new System.Drawing.Size(196, 38);
            this.passText.TabIndex = 5;
            // 
            // logs
            // 
            this.logs.Location = new System.Drawing.Point(468, 118);
            this.logs.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.logs.Name = "logs";
            this.logs.Size = new System.Drawing.Size(579, 966);
            this.logs.TabIndex = 6;
            this.logs.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(40, 495);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.groupBox1.Size = new System.Drawing.Size(345, 194);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Channels";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(38, 157);
            this.radioButton3.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(157, 36);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "SPS101";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged_1);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(32, 93);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(179, 36);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "MATH101";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged_1);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(32, 41);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(124, 36);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "IF100";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(116, 147);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 32);
            this.label1.TabIndex = 8;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(97, 216);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 32);
            this.label2.TabIndex = 9;
            this.label2.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 370);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 32);
            this.label3.TabIndex = 10;
            this.label3.Text = "Username";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 427);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 32);
            this.label4.TabIndex = 11;
            this.label4.Text = "Password";
            // 
            // auth_button
            // 
            this.auth_button.Location = new System.Drawing.Point(253, 718);
            this.auth_button.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.auth_button.Name = "auth_button";
            this.auth_button.Size = new System.Drawing.Size(187, 72);
            this.auth_button.TabIndex = 12;
            this.auth_button.Text = "Login";
            this.auth_button.UseVisualStyleBackColor = true;
            this.auth_button.Click += new System.EventHandler(this.auth_button_Click);
            // 
            // disconnect_button
            // 
            this.disconnect_button.Location = new System.Drawing.Point(40, 1012);
            this.disconnect_button.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.disconnect_button.Name = "disconnect_button";
            this.disconnect_button.Size = new System.Drawing.Size(400, 72);
            this.disconnect_button.TabIndex = 13;
            this.disconnect_button.Text = "Disconnect";
            this.disconnect_button.UseVisualStyleBackColor = true;
            this.disconnect_button.Click += new System.EventHandler(this.disconnect_button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 839);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 32);
            this.label5.TabIndex = 14;
            this.label5.Text = "Message";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // msg_box
            // 
            this.msg_box.Location = new System.Drawing.Point(181, 839);
            this.msg_box.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.msg_box.Name = "msg_box";
            this.msg_box.Size = new System.Drawing.Size(259, 38);
            this.msg_box.TabIndex = 15;
            // 
            // send_msg_btn
            // 
            this.send_msg_btn.Location = new System.Drawing.Point(178, 899);
            this.send_msg_btn.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.send_msg_btn.Name = "send_msg_btn";
            this.send_msg_btn.Size = new System.Drawing.Size(262, 62);
            this.send_msg_btn.TabIndex = 16;
            this.send_msg_btn.Text = "Send";
            this.send_msg_btn.UseVisualStyleBackColor = true;
            this.send_msg_btn.Click += new System.EventHandler(this.send_msg_btn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1074, 1099);
            this.Controls.Add(this.send_msg_btn);
            this.Controls.Add(this.msg_box);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.disconnect_button);
            this.Controls.Add(this.auth_button);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.logs);
            this.Controls.Add(this.passText);
            this.Controls.Add(this.userText);
            this.Controls.Add(this.portText);
            this.Controls.Add(this.ipText);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.connectButton);
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.TextBox ipText;
        private System.Windows.Forms.TextBox portText;
        private System.Windows.Forms.TextBox userText;
        private System.Windows.Forms.TextBox passText;
        private System.Windows.Forms.RichTextBox logs;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button auth_button;
        private System.Windows.Forms.Button disconnect_button;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox msg_box;
        private System.Windows.Forms.Button send_msg_btn;
    }
}