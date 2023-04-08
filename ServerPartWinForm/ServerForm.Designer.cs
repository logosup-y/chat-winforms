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
            disconnectButton = new Button();
            richTextBoxLog = new RichTextBox();
            connectionPortTextBox = new TextBox();
            SuspendLayout();
            // 
            // labelStatus
            // 
            labelStatus.Location = new Point(15, 11);
            labelStatus.Margin = new Padding(4, 0, 4, 0);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(398, 25);
            labelStatus.TabIndex = 0;
            labelStatus.Text = "status connection";
            // 
            // ConnectButton
            // 
            ConnectButton.Location = new Point(695, 6);
            ConnectButton.Margin = new Padding(4);
            ConnectButton.Name = "ConnectButton";
            ConnectButton.Size = new Size(118, 36);
            ConnectButton.TabIndex = 1;
            ConnectButton.Text = "Connect";
            ConnectButton.UseVisualStyleBackColor = true;
            ConnectButton.Click += Connect_Click;
            // 
            // disconnectButton
            // 
            disconnectButton.Location = new Point(851, 6);
            disconnectButton.Margin = new Padding(4);
            disconnectButton.Name = "disconnectButton";
            disconnectButton.Size = new Size(118, 36);
            disconnectButton.TabIndex = 2;
            disconnectButton.Text = "Disconnect";
            disconnectButton.UseVisualStyleBackColor = true;
            disconnectButton.Click += DisconnectButton_Click;
            // 
            // richTextBoxLog
            // 
            richTextBoxLog.Location = new Point(15, 50);
            richTextBoxLog.Margin = new Padding(4);
            richTextBoxLog.Name = "richTextBoxLog";
            richTextBoxLog.ReadOnly = true;
            richTextBoxLog.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBoxLog.Size = new Size(665, 496);
            richTextBoxLog.TabIndex = 3;
            richTextBoxLog.Text = "";
            // 
            // connectionPortTextBox
            // 
            connectionPortTextBox.Location = new Point(420, 9);
            connectionPortTextBox.Name = "connectionPortTextBox";
            connectionPortTextBox.Size = new Size(150, 31);
            connectionPortTextBox.TabIndex = 4;
            connectionPortTextBox.Click += ConnectionPort_Click;
            connectionPortTextBox.Enter += ConnectionPort_Enter;
            connectionPortTextBox.Leave += ConnectionPort_Leave;
            // 
            // ServerForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 562);
            Controls.Add(connectionPortTextBox);
            Controls.Add(richTextBoxLog);
            Controls.Add(disconnectButton);
            Controls.Add(ConnectButton);
            Controls.Add(labelStatus);
            Margin = new Padding(4);
            Name = "ServerForm";
            Text = "Server";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelStatus;
        private Button ConnectButton;
        private Button disconnectButton;
        private RichTextBox richTextBoxLog;
        private TextBox connectionPortTextBox;
    }
}