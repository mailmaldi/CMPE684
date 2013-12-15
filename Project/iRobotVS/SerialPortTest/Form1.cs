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
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

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

        private RssiValues rssiValues = new RssiValues();
        TargetQueue targetQueue = new TargetQueue();
        Thread serialQParserThread;
        Thread RobotMoveThread;
        private volatile bool robotThreadRunning = false;

        private byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
        //private static System.IO.FileStream file = new FileStream(dir + "\\test.txt", FileMode.Create);
        private System.IO.StreamWriter file = new System.IO.StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\test.txt");
        private System.IO.StreamWriter file2 = new System.IO.StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\test2.txt");

        private BlockingCollection<byte> serialQ = new BlockingCollection<byte>();

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
            serialQParserThread = new Thread(ParserMethod);
            RobotMoveThread = new Thread(RobotMover);
            if (openButton.Text == "Open")
            {
                try
                {
                    serialPort = new SerialPort(portNamesComboBox.SelectedItem.ToString(), 57600, Parity.None, 8, StopBits.One);
                    serialPort.Handshake = Handshake.None;
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                    serialPort.ReadBufferSize = 200;
                    serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(serialPort_ErrorReceived);
                    serialPort.Open();

                    serialQParserThread.Start();
                    RobotMoveThread.Start();
                    robotThreadRunning = true;

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
                    robotThreadRunning = false;
                    try
                    {
                        RobotMoveThread.Abort();
                    }
                    catch (Exception e)
                    {
                        RobotMoveThread.Join();
                    }

                    SendToSerial(207);
                    Thread.Sleep(5000);

                    serialPort.Close();
                    serialPort.Dispose();
                    serialQParserThread.Abort();

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
            //MessageBox.Show("What the heck");
            Console.Out.WriteLine("What the heck");
            //TODO IF error, then wipe out the current buffer?
        }

        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesCount = serialPort.BytesToRead;
            byte[] buffer = new byte[bytesCount];
            serialPort.Read(buffer, 0, bytesCount);
            //file.Write(buffer, 0, bytesCount);
            //file.Write(newline, 0, newline.Length);

            foreach (byte b in buffer)
            {
                serialQ.Add(b);
            }

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
                this.BeginInvoke(d, new object[] { state });
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
                this.BeginInvoke(d, new object[] { str });
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

        private void SendToSerial(byte cmd)
        {
            byte[] cmds = new byte[1];
            cmds[0] = cmd;
            SendToSerial(cmds);
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

        private void ParserMethod()
        {
            while (true)
            {
                byte result = serialQ.Take();
                if (result == 0x7E)
                {
                    result = serialQ.Take();
                    if (result == 0x45)
                    {
                        byte[] buffer = new byte[50];
                        int count = 0;
                        while ((result = serialQ.Take()) != 0x7E)
                        {
                            if (result == 0x7D)
                            {
                                result = serialQ.Take();
                                if (result == 0x5E)
                                {
                                    result = 0x7E;
                                }
                                else if (result == 0x5D)
                                {
                                    result = 0x7D;
                                }
                                else
                                {
                                    Console.Out.WriteLine("MILIND: 7D was not followed by 5E or 5D");
                                }
                            }
                            buffer[count++] = result;
                        }
                        //Console.Out.Write("RAW: ");
                        for (int i = 0; i < count; i++)
                        {
                            //Console.Out.Write(buffer[i] + " ");
                            file2.Write(buffer[i] + " ");
                        }
                        //Console.Out.WriteLine();
                        file2.WriteLine();
                        file2.Flush();
                        parseFrame(buffer, count);
                        //string hexstr = ByteArrayToHexString(buffer);
                        //Console.Out.WriteLine(hexstr);

                    }//End of if 0x45
                }// End of if 0x7E
                //Console.Out.Write(result + " ");
            }

        }// End of ParserMethod Thread function

        private void parseFrame(byte[] buffer, int count)
        {
            byte groupId = buffer[7];
            switch (groupId)
            {
                case 77:
                    updateRssiList(buffer, count);
                    break;
                case 78:
                    updateTargetQueue(buffer, count);
                    break;
                default:
                    Console.Out.WriteLine("MILIND: NO GROUP ID IN PREVIOUS BUFFER!");
                    break;
            }
        }

        //TODO This is a bad function, parse more nicely please
        private void updateRssiList(byte[] buffer, int count)
        {
            if (buffer[7] != 77)
                return;

            List<RssiValue> values = new List<RssiValue>();
            int nodeid = buffer[9];
            values.Add(new RssiValue(0, buffer[11]));
            values.Add(new RssiValue(1, buffer[13]));
            values.Add(new RssiValue(2, buffer[15]));
            values.Add(new RssiValue(3, buffer[17]));
            values.Add(new RssiValue(4, buffer[19]));

            rssiValues.setValuesForNode(nodeid, values, false);

            //Console.Out.WriteLine(rssiValues.toString());
            //RssiValues.printMatrix(rssiValues.getRssiValuesMatrix());
            //Class1.test(rssiValues.getRssiValuesMatrix());

            //TODO remove this
        }

        private void updateTargetQueue(byte[] buffer, int count)
        {
            if (buffer[7] != 78)
                return;
            int nodeid = buffer[9];

            byte[] bytes = { buffer[11], buffer[10] };
            int i = BitConverter.ToInt16(bytes, 0);

            Console.Out.WriteLine("BEFORE: "+ targetQueue.toString());
            if (i > 950)
            {
                targetQueue.addTarget(nodeid);
                Console.Out.WriteLine("MILIND: ADDING NODE {0} AS TARGET SINCE EVENT VALUE {1}", nodeid, i);
                Console.Out.WriteLine("AFTER: "+ targetQueue.toString());
            }
            else
            {
                Console.Out.WriteLine("MILIND: NOT ADDING NODE {0} AS TARGET SINCE EVENT VALUE {1}", nodeid, i);
            }
        }

        private void RobotMover()
        {
            //return;
            Thread.Sleep(10000);
            while (robotThreadRunning)
            {
                try
                {
                    SendToSerial(200); // sing
                    trysleep(1000);
                    SendToSerial(203); // forward
                    trysleep(1000);
                    SendToSerial(207); // stop
                    trysleep(2000);
                    SendToSerial(204); // back
                    trysleep(1000);
                    SendToSerial(207); // stop
                    trysleep(2000);
                }
                catch (Exception e) { }
                finally { }
            }
        }

        private void trysleep(int millis)
        {
            try
            {
                Thread.Sleep(millis);
            }
            catch (Exception e) { }
        }


    }
}
