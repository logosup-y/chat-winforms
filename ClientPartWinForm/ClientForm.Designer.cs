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
            DisconnectButton = new Button();
            messagesRichTextBox = new RichTextBox();
            messageTextBox = new TextBox();
            sendButton = new Button();
            SuspendLayout();
            // 
            // connectionStatus
            // 
            connectionStatus.AutoSize = true;
            connectionStatus.Location = new Point(15, 11);
            connectionStatus.Margin = new Padding(4, 0, 4, 0);
            connectionStatus.Name = "connectionStatus";
            connectionStatus.Size = new Size(154, 25);
            connectionStatus.TabIndex = 0;
            connectionStatus.Text = "Connection status";
            // 
            // usernameTextBox
            // 
            usernameTextBox.Location = new Point(342, 6);
            usernameTextBox.Margin = new Padding(4, 4, 4, 4);
            usernameTextBox.Name = "usernameTextBox";
            usernameTextBox.Size = new Size(354, 31);
            usernameTextBox.TabIndex = 1;
            usernameTextBox.Enter += UsernameTextBox_Enter;
            usernameTextBox.Leave += UsernameTextBox_Leave;
            // 
            // connectButton
            // 
            connectButton.Location = new Point(765, 5);
            connectButton.Margin = new Padding(4, 4, 4, 4);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(118, 36);
            connectButton.TabIndex = 2;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // DisconnectButton
            // 
            DisconnectButton.Location = new Point(931, 6);
            DisconnectButton.Margin = new Padding(4, 4, 4, 4);
            DisconnectButton.Name = "DisconnectButton";
            DisconnectButton.Size = new Size(118, 36);
            DisconnectButton.TabIndex = 3;
            DisconnectButton.Text = "Disconnect";
            DisconnectButton.UseVisualStyleBackColor = true;
            DisconnectButton.Click += DisconnectButton_Click;
            // 
            // messagesRichTextBox
            // 
            messagesRichTextBox.Location = new Point(15, 54);
            messagesRichTextBox.Margin = new Padding(4, 4, 4, 4);
            messagesRichTextBox.Name = "messagesRichTextBox";
            messagesRichTextBox.ReadOnly = true;
            messagesRichTextBox.Size = new Size(1033, 322);
            messagesRichTextBox.TabIndex = 4;
            messagesRichTextBox.Text = "";
            // 
            // messageTextBox
            // 
            messageTextBox.Location = new Point(15, 384);
            messageTextBox.Margin = new Padding(4, 4, 4, 4);
            messageTextBox.Multiline = true;
            messageTextBox.Name = "messageTextBox";
            messageTextBox.Size = new Size(866, 163);
            messageTextBox.TabIndex = 5;
            // 
            // sendButton
            // 
            sendButton.Location = new Point(890, 384);
            sendButton.Margin = new Padding(4, 4, 4, 4);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(159, 164);
            sendButton.TabIndex = 6;
            sendButton.Text = "Send message";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += SendButton_Click;
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1064, 562);
            Controls.Add(sendButton);
            Controls.Add(messageTextBox);
            Controls.Add(messagesRichTextBox);
            Controls.Add(DisconnectButton);
            Controls.Add(connectButton);
            Controls.Add(usernameTextBox);
            Controls.Add(connectionStatus);
            Margin = new Padding(4, 4, 4, 4);
            Name = "ClientForm";
            Text = "Client";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label connectionStatus;
        private TextBox usernameTextBox;
        private Button connectButton;
        private Button DisconnectButton;
        private RichTextBox messagesRichTextBox;
        private TextBox messageTextBox;
        private Button sendButton;
    }
}