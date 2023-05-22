
namespace Server
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
            this.logs = new System.Windows.Forms.RichTextBox();
            this.ipText = new System.Windows.Forms.TextBox();
            this.listenButton = new System.Windows.Forms.Button();
            this.portText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.disconnect_button = new System.Windows.Forms.Button();
            this.text_key = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.IF100 = new System.Windows.Forms.Button();
            this.MATH101 = new System.Windows.Forms.Button();
            this.SPS101 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logs
            // 
            this.logs.Location = new System.Drawing.Point(360, 62);
            this.logs.Margin = new System.Windows.Forms.Padding(6);
            this.logs.Name = "logs";
            this.logs.Size = new System.Drawing.Size(448, 814);
            this.logs.TabIndex = 0;
            this.logs.Text = "";
            // 
            // ipText
            // 
            this.ipText.Location = new System.Drawing.Point(118, 178);
            this.ipText.Margin = new System.Windows.Forms.Padding(6);
            this.ipText.Name = "ipText";
            this.ipText.Size = new System.Drawing.Size(196, 38);
            this.ipText.TabIndex = 1;
            // 
            // listenButton
            // 
            this.listenButton.Location = new System.Drawing.Point(46, 315);
            this.listenButton.Margin = new System.Windows.Forms.Padding(6);
            this.listenButton.Name = "listenButton";
            this.listenButton.Size = new System.Drawing.Size(268, 63);
            this.listenButton.TabIndex = 2;
            this.listenButton.Text = "Listen";
            this.listenButton.UseVisualStyleBackColor = true;
            this.listenButton.Click += new System.EventHandler(this.listenButton_Click);
            // 
            // portText
            // 
            this.portText.Location = new System.Drawing.Point(118, 251);
            this.portText.Margin = new System.Windows.Forms.Padding(6);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(196, 38);
            this.portText.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(68, 184);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 32);
            this.label1.TabIndex = 4;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 257);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 32);
            this.label2.TabIndex = 5;
            this.label2.Text = "Port";
            // 
            // disconnect_button
            // 
            this.disconnect_button.Location = new System.Drawing.Point(46, 785);
            this.disconnect_button.Margin = new System.Windows.Forms.Padding(6);
            this.disconnect_button.Name = "disconnect_button";
            this.disconnect_button.Size = new System.Drawing.Size(272, 72);
            this.disconnect_button.TabIndex = 6;
            this.disconnect_button.Text = "Disconnect";
            this.disconnect_button.UseVisualStyleBackColor = true;
            this.disconnect_button.Click += new System.EventHandler(this.disconnect_button_Click);
            // 
            // text_key
            // 
            this.text_key.Location = new System.Drawing.Point(118, 461);
            this.text_key.Margin = new System.Windows.Forms.Padding(6);
            this.text_key.Name = "text_key";
            this.text_key.Size = new System.Drawing.Size(196, 38);
            this.text_key.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(47, 461);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 32);
            this.label3.TabIndex = 8;
            this.label3.Text = "Key";
            // 
            // IF100
            // 
            this.IF100.Location = new System.Drawing.Point(140, 511);
            this.IF100.Margin = new System.Windows.Forms.Padding(6);
            this.IF100.Name = "IF100";
            this.IF100.Size = new System.Drawing.Size(178, 41);
            this.IF100.TabIndex = 9;
            this.IF100.Text = "IF100";
            this.IF100.UseVisualStyleBackColor = true;
            this.IF100.Click += new System.EventHandler(this.IF100_Click);
            // 
            // MATH101
            // 
            this.MATH101.Location = new System.Drawing.Point(140, 564);
            this.MATH101.Margin = new System.Windows.Forms.Padding(6);
            this.MATH101.Name = "MATH101";
            this.MATH101.Size = new System.Drawing.Size(174, 42);
            this.MATH101.TabIndex = 10;
            this.MATH101.Text = "MATH101";
            this.MATH101.UseVisualStyleBackColor = true;
            // 
            // SPS101
            // 
            this.SPS101.Location = new System.Drawing.Point(140, 618);
            this.SPS101.Margin = new System.Windows.Forms.Padding(6);
            this.SPS101.Name = "SPS101";
            this.SPS101.Size = new System.Drawing.Size(174, 42);
            this.SPS101.TabIndex = 11;
            this.SPS101.Text = "SPS101";
            this.SPS101.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 1110);
            this.Controls.Add(this.SPS101);
            this.Controls.Add(this.MATH101);
            this.Controls.Add(this.IF100);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.text_key);
            this.Controls.Add(this.disconnect_button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.portText);
            this.Controls.Add(this.listenButton);
            this.Controls.Add(this.ipText);
            this.Controls.Add(this.logs);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox logs;
        private System.Windows.Forms.TextBox ipText;
        private System.Windows.Forms.Button listenButton;
        private System.Windows.Forms.TextBox portText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button disconnect_button;
        private System.Windows.Forms.TextBox text_key;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button IF100;
        private System.Windows.Forms.Button MATH101;
        private System.Windows.Forms.Button SPS101;
    }
}

