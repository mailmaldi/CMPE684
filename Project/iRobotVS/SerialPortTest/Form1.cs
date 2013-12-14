using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace SerialPortTest
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
        private byte selectedRobot = 1;
        private bool sendRobotID = true;
        private bool cliffSensors = false;
        private int numberOfsensorBytes = 4;
        private int currentReadNumber = 0;
        private byte[] sensorsData;

        private static string dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
        //private static System.IO.FileStream file = new FileStream(dir + "\\test.txt", FileMode.Create);
        private static System.IO.StreamWriter file = new System.IO.StreamWriter( dir + "\\test.txt" );

        private delegate void AddItemCallBack(string str);
        private delegate void GUIRelatedUpdateCallBack(bool state);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            portNamesComboBox.Items.AddRange(SerialPort.GetPortNames());
            leftCliffLabel.Text = "";
            rightCliffLabel.Text = "";
            fronLeftCliffLabel.Text = "";
            fronRightCliffLabel.Text = "";
            this.sensorsData = new byte[numberOfsensorBytes];
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            if (portNamesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show(this, "Please select one port from the list", "OOps");
                return;
            }
            OpenThePort(portNamesComboBox.SelectedItem.ToString());
        }


        private void ButtonEnables(bool state)
        {
            portNamesComboBox.Enabled = !state;
            sendButton.Enabled = state;
            cliffButton.Enabled = state;
            singButton.Enabled = state;
            blinkButton.Enabled = state;
            stopButton.Enabled = state;
            forwardButton.Enabled = state;
            backwardButton.Enabled = state;
            leftButton.Enabled = state;
            rightButton.Enabled = state;
        }

        private void OpenThePort(String portName)
        {
            if (openButton.Text == "Open")
            {
                try
                {
                    serialPort = new SerialPort(portNamesComboBox.SelectedItem.ToString(), 57600, Parity.None, 8, StopBits.One);
                    serialPort.Handshake = Handshake.None;
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                    serialPort.ReadBufferSize = 20;
                    serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(serialPort_ErrorReceived);
                    serialPort.Open();

                    openButton.Text = "Close";
                    ButtonEnables(true);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(this, ex.Message, "ERROR!!!");
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Port is open already!!!!\r\nchecked other apps");
                }
            }
            else
            {
                try
                {
                    serialPort.Close();
                    serialPort.Dispose();

                    openButton.Text = "Open";
                    ButtonEnables(false);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(this, ex.Message, "ERROR!!!");
                }
            }
        }

        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            MessageBox.Show("What the heck");
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesCount = serialPort.BytesToRead;
            byte[] buffer = new byte[bytesCount];
            serialPort.Read(buffer, 0, bytesCount);
            //file.Write(buffer, 0, bytesCount);
            //file.Write(newline, 0, newline.Length);

            string hexStr = ByteArrayToHexString(buffer);

            file.WriteLine(hexStr);
            file.Flush();

            AddItem(hexStr);

            if (this.cliffSensors)
            {
                for (int i = 0; i < bytesCount && this.currentReadNumber < this.numberOfsensorBytes; ++i, ++currentReadNumber)
                {
                    this.sensorsData[currentReadNumber] = buffer[i];
                }
                if (this.currentReadNumber == this.numberOfsensorBytes)
                {
                    this.currentReadNumber = 0;
                    this.cliffSensors = false;
                    GUIRelatedUpdate(true);
                }
            }
        }

        private void GUIRelatedUpdate(bool state)
        {
            if (this.cliffButton.InvokeRequired)
            {
                GUIRelatedUpdateCallBack d = new GUIRelatedUpdateCallBack(GUIRelatedUpdate);
                this.Invoke(d, new object[] { state });
            }
            else
            {
                //this.cliffButton.Enabled = state;
                ShowSensorStates();
            }
        }

        private void ShowSensorStates()
        {
            leftCliffLabel.Text = (sensorsData[0] == 0) ? "Off" : "On";
            fronLeftCliffLabel.Text = (sensorsData[1] == 0) ? "Off" : "On";
            fronRightCliffLabel.Text = (sensorsData[2] == 0) ? "Off" : "On";
            rightCliffLabel.Text = (sensorsData[3] == 0) ? "Off" : "On";
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }

        private void AddItem(string str)
        {
            if (this.rawDataTextBox.InvokeRequired)
            {
                AddItemCallBack d = new AddItemCallBack(AddItem);
                this.Invoke(d, new object[] { str });
            }
            else
            {
                this.rawDataTextBox.Text += str + "\r\n";
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.Close();
                }
                catch (InvalidOperationException) { }

            }
        }

        private void cmdTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!((e.KeyChar >= 'a' && e.KeyChar <= 'f') || (e.KeyChar >= 'A' && e.KeyChar <= 'F') ||
                (e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar == Convert.ToChar(Keys.Back))))
            {
                e.Handled = true;
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            byte[] cmds = new byte[3];
            cmds[0] = byte.Parse(cmdTextBox1.Text, System.Globalization.NumberStyles.Integer);
            cmds[1] = byte.Parse(cmdTextBox2.Text, System.Globalization.NumberStyles.Integer);
            cmds[2] = byte.Parse(cmdTextBox3.Text, System.Globalization.NumberStyles.Integer);

            if (serialPort != null && serialPort.IsOpen)
                serialPort.Write(cmds, 0, 3);
            else
                MessageBox.Show("WHY");
        }

        private void SendToSerial(byte[] cmds)
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                MessageBox.Show("serial port is not open!!!!");
                return;
            }

            //if (this.sendRobotID)
            //{
            //    byte[] temp = new byte[2];
            //    temp[0] = 255; //this my code for to inform that next byte is robot address
            //    temp[1] = selectedRobot;
            //    serialPort.Write(temp, 0, temp.Length);
            //    this.sendRobotID = false;
            //}

            serialPort.Write(cmds, 0, cmds.Length);
        }

        private void singButton_Click(object sender, EventArgs e)
        {
            byte[] cmds = new byte[1];
            cmds[0] = 200; //commandid 200
            SendToSerial(cmds);
        }

        private void blinkButton_Click(object sender, EventArgs e)
        {
            byte[] cmds = new byte[1];
            cmds[0] = 201; //commandid 201
            SendToSerial(cmds);
        }

        private void robotRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (robotRadioButton1.Checked)
                this.selectedRobot = 1;
            else
                this.selectedRobot = 2;

            this.sendRobotID = true;
        }

        private void cliffButton_Click(object sender, EventArgs e)
        {
            byte[] cmds = new byte[1];
            cmds[0] = 202; //commandid 202
            SendToSerial(cmds);
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            byte[] cmds = new byte[1];
            cmds[0] = 203; //commandid 203
            SendToSerial(cmds);
        }

        private void backwardButton_Click(object sender, EventArgs e)
        {
            byte[] cmds = new byte[1];
            cmds[0] = 204; //commandid 204
            SendToSerial(cmds);
        }

        private void leftButton_Click(object sender, EventArgs e)
        {
            byte[] cmds = new byte[1];
            cmds[0] = 205; //commandid 205
            SendToSerial(cmds);
        }

        private void rightButton_Click(object sender, EventArgs e)
        {
            byte[] cmds = new byte[1];
            cmds[0] = 206; //commandid 206
            SendToSerial(cmds);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            byte[] cmds = new byte[1];
            cmds[0] = 207; //commandid 207
            SendToSerial(cmds);
        }
    }
}
