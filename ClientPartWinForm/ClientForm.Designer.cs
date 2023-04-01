namespace ClientPartWinForm
{
    partial class ClientForm
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
            connectionStatus = new Label();
            usernameTextBox = new TextBox();
            connectButton = new Button();
            disconnectButton = new Button();
            messagesRichTextBox = new RichTextBox();
            messageTextBox = new TextBox();
            sendButton = new Button();
            SuspendLayout();
            // 
            // connectionStatus
            // 
            connectionStatus.AutoSize = true;
            connectionStatus.Location = new Point(12, 9);
            connectionStatus.Name = "connectionStatus";
            connectionStatus.Size = new Size(126, 20);
            connectionStatus.TabIndex = 0;
            connectionStatus.Text = "Connection status";
            // 
            // usernameTextBox
            // 
            usernameTextBox.Location = new Point(274, 5);
            usernameTextBox.Name = "usernameTextBox";
            usernameTextBox.Size = new Size(284, 27);
            usernameTextBox.TabIndex = 1;
            usernameTextBox.Enter += usernameTextBox_Enter;
            usernameTextBox.Leave += usernameTextBox_Leave;
            // 
            // connectButton
            // 
            connectButton.Location = new Point(612, 4);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(94, 29);
            connectButton.TabIndex = 2;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // disconnectButton
            // 
            disconnectButton.Location = new Point(745, 5);
            disconnectButton.Name = "disconnectButton";
            disconnectButton.Size = new Size(94, 29);
            disconnectButton.TabIndex = 3;
            disconnectButton.Text = "Disconnect";
            disconnectButton.UseVisualStyleBackColor = true;
            disconnectButton.Click += disconnectButton_Click;
            // 
            // messagesRichTextBox
            // 
            messagesRichTextBox.Location = new Point(12, 43);
            messagesRichTextBox.Name = "messagesRichTextBox";
            messagesRichTextBox.ReadOnly = true;
            messagesRichTextBox.Size = new Size(827, 258);
            messagesRichTextBox.TabIndex = 4;
            messagesRichTextBox.Text = "";
            // 
            // messageTextBox
            // 
            messageTextBox.Location = new Point(12, 307);
            messageTextBox.Multiline = true;
            messageTextBox.Name = "messageTextBox";
            messageTextBox.Size = new Size(694, 131);
            messageTextBox.TabIndex = 5;
            // 
            // sendButton
            // 
            sendButton.Location = new Point(712, 307);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(127, 131);
            sendButton.TabIndex = 6;
            sendButton.Text = "Send message";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += sendButton_Click;
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(851, 450);
            Controls.Add(sendButton);
            Controls.Add(messageTextBox);
            Controls.Add(messagesRichTextBox);
            Controls.Add(disconnectButton);
            Controls.Add(connectButton);
            Controls.Add(usernameTextBox);
            Controls.Add(connectionStatus);
            Name = "ClientForm";
            Text = "Client";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label connectionStatus;
        private TextBox usernameTextBox;
        private Button connectButton;
        private Button disconnectButton;
        private RichTextBox messagesRichTextBox;
        private TextBox messageTextBox;
        private Button sendButton;
    }
}