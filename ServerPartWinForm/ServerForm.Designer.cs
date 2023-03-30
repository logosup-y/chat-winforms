namespace ServerPartWinForm
{
    partial class ServerForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelStatus = new Label();
            ConnectButton = new Button();
            DisconnectButton = new Button();
            richTextBoxLog = new RichTextBox();
            SuspendLayout();
            // 
            // labelStatus
            // 
            labelStatus.Location = new Point(12, 9);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(318, 20);
            labelStatus.TabIndex = 0;
            labelStatus.Text = "status connection";
            // 
            // ConnectButton
            // 
            ConnectButton.Location = new Point(556, 5);
            ConnectButton.Name = "ConnectButton";
            ConnectButton.Size = new Size(94, 29);
            ConnectButton.TabIndex = 1;
            ConnectButton.Text = "Connect";
            ConnectButton.UseVisualStyleBackColor = true;
            ConnectButton.Click += Connect_Click;
            // 
            // DisconnectButton
            // 
            DisconnectButton.Location = new Point(681, 5);
            DisconnectButton.Name = "DisconnectButton";
            DisconnectButton.Size = new Size(94, 29);
            DisconnectButton.TabIndex = 2;
            DisconnectButton.Text = "Disconnect";
            DisconnectButton.UseVisualStyleBackColor = true;
            DisconnectButton.Click += DisconnectButton_Click;
            // 
            // richTextBoxLog
            // 
            richTextBoxLog.Location = new Point(12, 40);
            richTextBoxLog.Name = "richTextBoxLog";
            richTextBoxLog.Size = new Size(763, 398);
            richTextBoxLog.TabIndex = 3;
            richTextBoxLog.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(richTextBoxLog);
            Controls.Add(DisconnectButton);
            Controls.Add(ConnectButton);
            Controls.Add(labelStatus);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Label labelStatus;
        private Button ConnectButton;
        private Button DisconnectButton;
        private RichTextBox richTextBoxLog;
    }
}