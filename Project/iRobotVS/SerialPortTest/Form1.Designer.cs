namespace SerialPortTest
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
            this.openButton = new System.Windows.Forms.Button();
            this.portNamesComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sendButton = new System.Windows.Forms.Button();
            this.cmdTextBox1 = new System.Windows.Forms.TextBox();
            this.cmdTextBox2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cmdTextBox3 = new System.Windows.Forms.TextBox();
            this.rawDataTextBox = new System.Windows.Forms.TextBox();
            this.singButton = new System.Windows.Forms.Button();
            this.leftButton = new System.Windows.Forms.Button();
            this.rightButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.forwardButton = new System.Windows.Forms.Button();
            this.backwardButton = new System.Windows.Forms.Button();
            this.blinkButton = new System.Windows.Forms.Button();
            this.robotRadioButton1 = new System.Windows.Forms.RadioButton();
            this.robotRadioButton2 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cliffButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.leftCliffLabel = new System.Windows.Forms.Label();
            this.rightCliffLabel = new System.Windows.Forms.Label();
            this.fronRightCliffLabel = new System.Windows.Forms.Label();
            this.fronLeftCliffLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(186, 12);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(75, 23);
            this.openButton.TabIndex = 0;
            this.openButton.Text = "Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // portNamesComboBox
            // 
            this.portNamesComboBox.FormattingEnabled = true;
            this.portNamesComboBox.Location = new System.Drawing.Point(59, 12);
            this.portNamesComboBox.Name = "portNamesComboBox";
            this.portNamesComboBox.Size = new System.Drawing.Size(121, 21);
            this.portNamesComboBox.TabIndex = 1;
            this.portNamesComboBox.Text = "---Select One";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Raw Recived Data (in Hex)";
            // 
            // sendButton
            // 
            this.sendButton.Enabled = false;
            this.sendButton.Location = new System.Drawing.Point(186, 56);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 5;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // cmdTextBox1
            // 
            this.cmdTextBox1.Enabled = false;
            this.cmdTextBox1.Location = new System.Drawing.Point(24, 57);
            this.cmdTextBox1.MaxLength = 3;
            this.cmdTextBox1.Name = "cmdTextBox1";
            this.cmdTextBox1.Size = new System.Drawing.Size(28, 20);
            this.cmdTextBox1.TabIndex = 6;
            this.cmdTextBox1.Text = "01";
            this.cmdTextBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmdTextBox1_KeyPress);
            // 
            // cmdTextBox2
            // 
            this.cmdTextBox2.Location = new System.Drawing.Point(58, 57);
            this.cmdTextBox2.MaxLength = 3;
            this.cmdTextBox2.Name = "cmdTextBox2";
            this.cmdTextBox2.Size = new System.Drawing.Size(48, 20);
            this.cmdTextBox2.TabIndex = 8;
            this.cmdTextBox2.Text = "128";
            this.cmdTextBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmdTextBox1_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(65, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "CM1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(109, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "CM2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 44);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "ID";
            // 
            // cmdTextBox3
            // 
            this.cmdTextBox3.Location = new System.Drawing.Point(112, 57);
            this.cmdTextBox3.MaxLength = 3;
            this.cmdTextBox3.Name = "cmdTextBox3";
            this.cmdTextBox3.Size = new System.Drawing.Size(50, 20);
            this.cmdTextBox3.TabIndex = 9;
            this.cmdTextBox3.Text = "132";
            this.cmdTextBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmdTextBox1_KeyPress);
            // 
            // rawDataTextBox
            // 
            this.rawDataTextBox.Location = new System.Drawing.Point(12, 120);
            this.rawDataTextBox.Multiline = true;
            this.rawDataTextBox.Name = "rawDataTextBox";
            this.rawDataTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.rawDataTextBox.Size = new System.Drawing.Size(229, 322);
            this.rawDataTextBox.TabIndex = 14;
            // 
            // singButton
            // 
            this.singButton.Enabled = false;
            this.singButton.Location = new System.Drawing.Point(472, 69);
            this.singButton.Name = "singButton";
            this.singButton.Size = new System.Drawing.Size(75, 48);
            this.singButton.TabIndex = 15;
            this.singButton.Text = "Sing";
            this.singButton.UseVisualStyleBackColor = true;
            this.singButton.Click += new System.EventHandler(this.singButton_Click);
            // 
            // leftButton
            // 
            this.leftButton.Enabled = false;
            this.leftButton.Location = new System.Drawing.Point(27, 80);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(75, 48);
            this.leftButton.TabIndex = 16;
            this.leftButton.Text = "Left";
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler(this.leftButton_Click);
            // 
            // rightButton
            // 
            this.rightButton.Enabled = false;
            this.rightButton.Location = new System.Drawing.Point(189, 80);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(75, 48);
            this.rightButton.TabIndex = 17;
            this.rightButton.Text = "Right";
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler(this.rightButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(108, 80);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 48);
            this.stopButton.TabIndex = 18;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // forwardButton
            // 
            this.forwardButton.Enabled = false;
            this.forwardButton.Location = new System.Drawing.Point(108, 23);
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Size = new System.Drawing.Size(75, 48);
            this.forwardButton.TabIndex = 19;
            this.forwardButton.Text = "Forward";
            this.forwardButton.UseVisualStyleBackColor = true;
            this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
            // 
            // backwardButton
            // 
            this.backwardButton.Enabled = false;
            this.backwardButton.Location = new System.Drawing.Point(108, 134);
            this.backwardButton.Name = "backwardButton";
            this.backwardButton.Size = new System.Drawing.Size(75, 48);
            this.backwardButton.TabIndex = 20;
            this.backwardButton.Text = "Backward";
            this.backwardButton.UseVisualStyleBackColor = true;
            this.backwardButton.Click += new System.EventHandler(this.backwardButton_Click);
            // 
            // blinkButton
            // 
            this.blinkButton.Enabled = false;
            this.blinkButton.Location = new System.Drawing.Point(347, 69);
            this.blinkButton.Name = "blinkButton";
            this.blinkButton.Size = new System.Drawing.Size(75, 48);
            this.blinkButton.TabIndex = 21;
            this.blinkButton.Text = "Blink";
            this.blinkButton.UseVisualStyleBackColor = true;
            this.blinkButton.Click += new System.EventHandler(this.blinkButton_Click);
            // 
            // robotRadioButton1
            // 
            this.robotRadioButton1.AutoSize = true;
            this.robotRadioButton1.Checked = true;
            this.robotRadioButton1.Location = new System.Drawing.Point(6, 19);
            this.robotRadioButton1.Name = "robotRadioButton1";
            this.robotRadioButton1.Size = new System.Drawing.Size(60, 17);
            this.robotRadioButton1.TabIndex = 22;
            this.robotRadioButton1.TabStop = true;
            this.robotRadioButton1.Text = "Robot1";
            this.robotRadioButton1.UseVisualStyleBackColor = true;
            this.robotRadioButton1.CheckedChanged += new System.EventHandler(this.robotRadioButton1_CheckedChanged);
            // 
            // robotRadioButton2
            // 
            this.robotRadioButton2.AutoSize = true;
            this.robotRadioButton2.Location = new System.Drawing.Point(109, 19);
            this.robotRadioButton2.Name = "robotRadioButton2";
            this.robotRadioButton2.Size = new System.Drawing.Size(60, 17);
            this.robotRadioButton2.TabIndex = 23;
            this.robotRadioButton2.TabStop = true;
            this.robotRadioButton2.Text = "Robot2";
            this.robotRadioButton2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.robotRadioButton2);
            this.groupBox1.Controls.Add(this.robotRadioButton1);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(347, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 50);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Robot Selection";
            // 
            // cliffButton
            // 
            this.cliffButton.Enabled = false;
            this.cliffButton.Location = new System.Drawing.Point(17, 27);
            this.cliffButton.Name = "cliffButton";
            this.cliffButton.Size = new System.Drawing.Size(89, 48);
            this.cliffButton.TabIndex = 25;
            this.cliffButton.Text = "Cliff sensors";
            this.cliffButton.UseVisualStyleBackColor = true;
            this.cliffButton.Click += new System.EventHandler(this.cliffButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(117, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Left";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(304, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "Right";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(164, 31);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "Front Left";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(231, 31);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 13);
            this.label10.TabIndex = 29;
            this.label10.Text = "Front Right";
            // 
            // leftCliffLabel
            // 
            this.leftCliffLabel.AutoSize = true;
            this.leftCliffLabel.Location = new System.Drawing.Point(119, 62);
            this.leftCliffLabel.Name = "leftCliffLabel";
            this.leftCliffLabel.Size = new System.Drawing.Size(21, 13);
            this.leftCliffLabel.TabIndex = 30;
            this.leftCliffLabel.Text = "sdf";
            // 
            // rightCliffLabel
            // 
            this.rightCliffLabel.AutoSize = true;
            this.rightCliffLabel.Location = new System.Drawing.Point(300, 62);
            this.rightCliffLabel.Name = "rightCliffLabel";
            this.rightCliffLabel.Size = new System.Drawing.Size(41, 13);
            this.rightCliffLabel.TabIndex = 31;
            this.rightCliffLabel.Text = "label11";
            // 
            // fronRightCliffLabel
            // 
            this.fronRightCliffLabel.AutoSize = true;
            this.fronRightCliffLabel.Location = new System.Drawing.Point(236, 62);
            this.fronRightCliffLabel.Name = "fronRightCliffLabel";
            this.fronRightCliffLabel.Size = new System.Drawing.Size(41, 13);
            this.fronRightCliffLabel.TabIndex = 32;
            this.fronRightCliffLabel.Text = "label12";
            // 
            // fronLeftCliffLabel
            // 
            this.fronLeftCliffLabel.AutoSize = true;
            this.fronLeftCliffLabel.Location = new System.Drawing.Point(166, 62);
            this.fronLeftCliffLabel.Name = "fronLeftCliffLabel";
            this.fronLeftCliffLabel.Size = new System.Drawing.Size(41, 13);
            this.fronLeftCliffLabel.TabIndex = 33;
            this.fronLeftCliffLabel.Text = "label13";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rightCliffLabel);
            this.groupBox2.Controls.Add(this.fronLeftCliffLabel);
            this.groupBox2.Controls.Add(this.cliffButton);
            this.groupBox2.Controls.Add(this.fronRightCliffLabel);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.leftCliffLabel);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Location = new System.Drawing.Point(269, 342);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(356, 100);
            this.groupBox2.TabIndex = 34;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cliff Sensors";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.leftButton);
            this.groupBox3.Controls.Add(this.rightButton);
            this.groupBox3.Controls.Add(this.stopButton);
            this.groupBox3.Controls.Add(this.forwardButton);
            this.groupBox3.Controls.Add(this.backwardButton);
            this.groupBox3.Location = new System.Drawing.Point(333, 134);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(292, 202);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Movement";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 468);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.blinkButton);
            this.Controls.Add(this.singButton);
            this.Controls.Add(this.rawDataTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmdTextBox3);
            this.Controls.Add(this.cmdTextBox2);
            this.Controls.Add(this.cmdTextBox1);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.portNamesComboBox);
            this.Controls.Add(this.openButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IRobot Create Command Sender";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.ComboBox portNamesComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.TextBox cmdTextBox1;
        private System.Windows.Forms.TextBox cmdTextBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox cmdTextBox3;
        private System.Windows.Forms.TextBox rawDataTextBox;
        private System.Windows.Forms.Button singButton;
        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button forwardButton;
        private System.Windows.Forms.Button backwardButton;
        private System.Windows.Forms.Button blinkButton;
        private System.Windows.Forms.RadioButton robotRadioButton1;
        private System.Windows.Forms.RadioButton robotRadioButton2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cliffButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label leftCliffLabel;
        private System.Windows.Forms.Label rightCliffLabel;
        private System.Windows.Forms.Label fronRightCliffLabel;
        private System.Windows.Forms.Label fronLeftCliffLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

