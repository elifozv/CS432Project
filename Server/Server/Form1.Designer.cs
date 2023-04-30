
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
            this.SuspendLayout();
            // 
            // logs
            // 
            this.logs.Location = new System.Drawing.Point(180, 32);
            this.logs.Name = "logs";
            this.logs.Size = new System.Drawing.Size(226, 422);
            this.logs.TabIndex = 0;
            this.logs.Text = "";
            // 
            // ipText
            // 
            this.ipText.Location = new System.Drawing.Point(59, 92);
            this.ipText.Name = "ipText";
            this.ipText.Size = new System.Drawing.Size(100, 22);
            this.ipText.TabIndex = 1;
            // 
            // listenButton
            // 
            this.listenButton.Location = new System.Drawing.Point(75, 194);
            this.listenButton.Name = "listenButton";
            this.listenButton.Size = new System.Drawing.Size(84, 37);
            this.listenButton.TabIndex = 2;
            this.listenButton.Text = "listen";
            this.listenButton.UseVisualStyleBackColor = true;
            this.listenButton.Click += new System.EventHandler(this.listenButton_Click);
            // 
            // portText
            // 
            this.portText.Location = new System.Drawing.Point(59, 149);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(100, 22);
            this.portText.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "ip";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "port";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 573);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.portText);
            this.Controls.Add(this.listenButton);
            this.Controls.Add(this.ipText);
            this.Controls.Add(this.logs);
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
    }
}

