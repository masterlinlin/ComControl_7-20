using Microsoft.VisualBasic.PowerPacks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ComControl_7_20
{
    public partial class Form1 : Form
    {

        //device 1
        // byte[] DeviceOpen1 = new byte[] { 0XAA, 0X08, 0X03, 0X00, 0X00, 0XCC };
        const string DeviceOpen1 = "AA0802000";
        const string DeviceClose1 = "AA08020000";

        //device 2
        const string DeviceOpen2 = "AA08020002";
        const string DeviceClose2 = "AA08020001";

        //device 3
        const string DeviceOpen3 = "AA08020004";
        const string DeviceClose3 = "AA08020003";

        //device 4
        const string DeviceOpen4 = "AA08020008";
        const string DeviceClose4 = "AA08020007";

        //device 5
        const string DeviceOpen5 = "AA08020010";
        const string DeviceClose5 = "AA0802000F";

        //device 6
        const string DeviceOpen6 = "AA08020020";
        const string DeviceClose6 = "AA0802001F";

        //device 7
        const string DeviceOpen7 = "AA08020040";
        const string DeviceClose7 = "AA0802003F";

        //device 8
        const string DeviceOpen8 = "AA08020080";
        const string DeviceClose8 = "AA0802007F";

        //device 9
        const string DeviceOpen9 = "AA08020100";
        const string DeviceClose9 = "AA080200FF";

        //device 10
        const string DeviceOpen10 = "AA08020200";
        const string DeviceClose10 = "AA080201FF";

        //device 11
        const string DeviceOpen11 = "AA08020400";
        const string DeviceClose11 = "AA080203FF";

        //device 12
        const string DeviceOpen12 = "AA08020800";
        const string DeviceClose12 = "AA080207FF";

        //device 13
        const string DeviceOpen13 = "AA08021000";
        const string DeviceClose13 = "AA08020FFF";

        //device 14
        const string DeviceOpen14 = "AA08022000";
        const string DeviceClose14 = "AA08021FFF";

        //device 15
        const string DeviceOpen15 = "AA08024000";
        const string DeviceClose15 = "AA08023FFF";

        //device16
        const string DeviceOpen16 = "AA08028000";
        const string DeviceClose16 = "AA08027FFF";

        //AllDevice
        const string AllDeviceOpen = "AA0802FFFF";
        const string AllDeviecClose = "AA08020000";

        byte[] SeriaPortDataBuffer = new byte[1];

        public Form1()
        {
            InitializeComponent();

        }


        private void button16_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {   //串口打开就关闭
                try
                {
                    serialPort1.Close();
                }
                catch { }

                MessageBox.Show("关闭串口成功！");
                button16.Text = "打开串口";
                button16.BackColor = Color.White;
            }
            else
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text;//端口号
                    serialPort1.Open();
                    button16.Text = "关闭串口";
                    MessageBox.Show("打开串口成功！");
                    button16.BackColor = Color.Green;
                }
                catch
                {

                    MessageBox.Show("串口打开失败"); ;
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SearchAddSerialToComBox(serialPort1, comboBox1);//扫描串口封装函数 
        }
        private void SerialPortCommanGroup()
        {//CRC校验结果拼接

            string data = DeviceOpen1;//传入要计算合成的
            string result = CRCForModbus(data);//CRC计算结果字符

        }


        private void WriteByteToSerialPort(byte data)
        {      //单字节写入串口  data为传入的功能码
            byte[] Buffer = new byte[2] { 0XAA, data };//定义的这个数组为俩个字节 0x00为头码 data功能码
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write(Buffer, 0, 2);
                }
                catch
                {

                    MessageBox.Show("串口数据发送错误，请检查");  //错误处理
                }
            }
        }
        //CRC数据校验方法 START--------------------------------------------------------------------
        public static String CRCForModbus(String data)  //CRC数据校验方法 
        {
            //处理数字转换
            String sendBuf = data;
            String sendnoNull1 = sendBuf.Trim();//去掉字符串前后的空格
            String sendnoNull2 = sendnoNull1.Replace(" ", "");//去掉字符串中间的空格
            String sendNOComma = sendnoNull2.Replace(',', ' ');    //去掉英文逗号
            String sendNOComma1 = sendNOComma.Replace('，', ' '); //去掉中文逗号
            String strSendNoComma2 = sendNOComma1.Replace("0x", "");   //去掉0x
            data = strSendNoComma2.Replace("0X", "");   //去掉0X

            Byte[] crcbuf = strToHexByte(data);

            //计算并填写CRC校验码
            Int32 crc = 0xffff;
            Int32 len = crcbuf.Length;
            for (Int32 n = 0; n < len; n++)
            {
                Byte i;
                crc = crc ^ crcbuf[n];
                for (i = 0; i < 8; i++)
                {
                    Int32 TT;
                    TT = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (TT == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }
            }
            crc = ((crc & 0xFF) << 8 | (crc >> 8));//高低字节换位
            String CRCString = crc.ToString("X2");

            return (data + CRCString);
        }

        public static Byte[] strToHexByte(String hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString = hexString.Insert(hexString.Length - 1, 0.ToString());
            Byte[] returnBytes = new Byte[hexString.Length / 2];
            for (Int32 i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        //CRC数据校验方法 END--------------------------------------------------------------------

        private void SearchAddSerialToComBox(SerialPort MyPort, ComboBox MyBox)
        {   //扫描串口加到下拉框中 
            string[] Mystring = new string[20];                          //将可用端口号添加到combox中
            string Buffer;                  //缓存
            MyBox.Items.Clear();//清空combox内容
            //int count = 0;
            for (int i = 0; i < 20; i++) //循环
            {
                try
                {
                    Buffer = "COM" + i.ToString();
                    MyPort.PortName = Buffer;
                    MyPort.Open();
                    Mystring[i - 1] = Buffer;
                    // MyPort[count]=Buffer;
                    MyBox.Items.Add(Buffer);//打开成功
                    MyPort.Close();
                    //count++;
                }
                catch
                {


                }
            }
            MyBox.Text = Mystring[0];
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)//接收串口返回值函数 
        {//串口中断函数 校验
         //byte DataReceived = (byte)(serialPort1.ReadByte());  //定义一个变量,直接接收数据，强制转换为字节
         //SetOvlShape(DataReceived);
         //byte[] buffin = new byte[] { 0XAA, 0X08, 0X03, 0X00, 0X00, 0XCE, 0X78, 0XCC };
         //serialPort1.Write(buffin, 0, 8);
         //DateTime dt = DateTime.Now;
         //while (serialPort1.BytesToRead == 0)
         //{
         //    Thread.Sleep(1);

            //    if (DateTime.Now.Subtract(dt).TotalMilliseconds > 5000) //如果5秒后仍然无数据返回，则视为超时
            //    {
            //        MessageBox.Show("主版无响应");
            //    }
            //}
            //List<byte> recList = new List<byte>();
            //byte[] recData = new byte[serialPort1.BytesToRead];
            //serialPort1.Read(recData, 0, recData.Length);
            //recList.AddRange(recData);//读出十进制返回数据   此数据为所有的十进制返回值集合 

            //recList = recList.GetRange(9, 16);


        }
        private void SetOvlShape(int which)
        {
            switch (which)//判断接收的数据是什么，直接修改灯的颜色，和发射端没有关系  此参数为随机设置，非正确指令 
            {
                case 0x10://器件1开
                    ovalShape11.FillColor = Color.Green;
                    break;
                case 0x11://器件1关
                    ovalShape11.FillColor = Color.Red;
                    break;
                case 0x02://器件2开
                    ovalShape12.FillColor = Color.Green;
                    break;
                case 0x12://器件2关
                    ovalShape12.FillColor = Color.Red;
                    break;
                case 0x03://器件3开
                    ovalShape13.FillColor = Color.Green;
                    break;
                case 0x13://器件3关
                    ovalShape13.FillColor = Color.Red;
                    break;
                case 0x04://器件4开
                    ovalShape14.FillColor = Color.Green;
                    break;
                case 0x14://器件4关
                    ovalShape14.FillColor = Color.Red;
                    break;
                case 0x05://器件5开
                    ovalShape15.FillColor = Color.Green;
                    break;
                case 0x15://器件5关
                    ovalShape15.FillColor = Color.Red;
                    break;
                case 0x06://器件6开
                    ovalShape16.FillColor = Color.Green;
                    break;
                case 0x16://器件6关
                    ovalShape16.FillColor = Color.Red;
                    break;
                case 0x07://器件7开
                    ovalShape16.FillColor = Color.Green;
                    break;
                case 0x17://器件7关
                    ovalShape16.FillColor = Color.Red;
                    break;
                case 0x08://器件8开
                    ovalShape16.FillColor = Color.Green;
                    break;
                case 0x18://器件8关
                    ovalShape16.FillColor = Color.Red;
                    break;
                case 0x09://器件9开
                    ovalShape16.FillColor = Color.Green;
                    break;
                case 0x19://器件9关
                    ovalShape16.FillColor = Color.Red;
                    break;
                case 0x1://器件10开
                    ovalShape16.FillColor = Color.Green;
                    break;
                case 0x20://器件10关
                    ovalShape16.FillColor = Color.Red;
                    break;
                case 0x21://器件11开
                    ovalShape16.FillColor = Color.Green;
                    break;
                case 0x31://器件11关
                    ovalShape16.FillColor = Color.Red;
                    break;

                case 0x22://器件12开
                    ovalShape16.FillColor = Color.Green;
                    break;
                case 0x32://器件12关
                    ovalShape16.FillColor = Color.Red;
                    break;
                    break;
                case 0x23://器件13开
                    ovalShape16.FillColor = Color.Green;
                    break;
                case 0x33://器件13关
                    ovalShape16.FillColor = Color.Red;
                    break;
                case 0x24://器件14开
                    ovalShape16.FillColor = Color.Green;
                    break;
                case 0x34://器件14关
                    ovalShape16.FillColor = Color.Red;
                    break;
                case 0x25://器件15开
                    ovalShape16.FillColor = Color.Green;
                    break;
                case 0x35://器件15关
                    ovalShape16.FillColor = Color.Red;
                    break;
                    break;
                case 0x26://器件16开
                    ovalShape16.FillColor = Color.Green;
                    break;
                case 0x36://器件16关
                    ovalShape16.FillColor = Color.Red;
                    break;
                case 0://器件全开
                    ovalShape12.FillColor = Color.Green;
                    ovalShape13.FillColor = Color.Green;
                    ovalShape14.FillColor = Color.Green;
                    ovalShape15.FillColor = Color.Green;
                    ovalShape16.FillColor = Color.Green;
                    ovalShape17.FillColor = Color.Green;
                    ovalShape18.FillColor = Color.Green;
                    ovalShape19.FillColor = Color.Green;
                    ovalShape20.FillColor = Color.Green;
                    ovalShape21.FillColor = Color.Green;
                    ovalShape22.FillColor = Color.Green;
                    ovalShape23.FillColor = Color.Green;
                    ovalShape24.FillColor = Color.Green;
                    ovalShape25.FillColor = Color.Green;
                    ovalShape26.FillColor = Color.Green;
                    ovalShape27.FillColor = Color.Green;

                    break;
                case 12://器件全关
                    ovalShape12.FillColor = Color.Green;
                    ovalShape13.FillColor = Color.Green;
                    ovalShape14.FillColor = Color.Red;
                    ovalShape15.FillColor = Color.Red;
                    ovalShape16.FillColor = Color.Red;
                    ovalShape17.FillColor = Color.Red;
                    ovalShape18.FillColor = Color.Red;
                    ovalShape19.FillColor = Color.Red;
                    ovalShape20.FillColor = Color.Red;
                    ovalShape21.FillColor = Color.Red;
                    ovalShape22.FillColor = Color.Red;
                    ovalShape23.FillColor = Color.Red;
                    ovalShape24.FillColor = Color.Red;
                    ovalShape25.FillColor = Color.Red;
                    ovalShape26.FillColor = Color.Red;
                    ovalShape27.FillColor = Color.Red;

                    break;
                default:
                    break;
            }
        }
        //判断字节数组相同的方法
        private bool PasswordEquals(byte[] b1,
        byte[] b2)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return
            false;
            for (int i =
            0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return
                    false;
            return true;
        }
        //获取串口返回数据的方法 
        public void ReceiveDate()
        {
            byte[] m_recvBytes = new byte[serialPort1.BytesToRead]; //定义缓冲区大小 
            int result = serialPort1.Read(m_recvBytes, 0, m_recvBytes.Length); //从串口读取数据 
            if (result <= 0)
                return;
            string strResult = Encoding.UTF8.GetString(m_recvBytes, 0, m_recvBytes.Length); //对数据进行转换            
            serialPort1.DiscardInBuffer();//丢弃来自串行驱动程序的接收缓冲区的数据。
        }
        //if (this.DataReceived != null)
        //    this.DataReceived(this, new SerialSortEventArgs() { Code = strResult });        } 




        private void button15_Click(object sender, EventArgs e)//扫描并并调用串口添加到下拉列表中 
        {
            SearchAddSerialToComBox(serialPort1, comboBox1);
        }


        static int buffersize = 8;   //十六进制数的大小（假设为Byte,可调整数字大小）
        byte[] buffer = new Byte[buffersize];   //创建缓冲区

        public int dbs { get; private set; }
        public int s { get; private set; }
        public string function { get; private set; }

        public static string byteToHexStr(byte[] bytes) //字节数组转16进制字符串的方法
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        /// <summary>
        /// 校验入参的方法。开启前需要检验下一器件是否打开，没开则开，开则不执行
        /// 判断开器件2状态时，使用开器件1返回值（此时器件2未打开）判断器件2的状态
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string TestInsert(string bytes)
        {//校验入参指令的方法，通过CRC直接校验对应字节值即可完成校验  
         //1.器件打开指令的合成。此时不写入输入指令，根据判断是否写入。
            string result = CRCForModbus(DeviceClose1); //CRC校验数据结果  
            string resulted = result + "CC";//完整通讯指令合成
            byte[] byteArray = strToHexByte1(resulted);//通讯指令转换成字节完成

            //2.器件状态判断
            byte[] Status = new byte[] { 0XAA, 0X08, 0X03, 0X00, 0X00, 0XCE, 0X78, 0XCC };
            serialPort1.Write(Status, 0, 8);
            Thread.Sleep(100);
            int a = serialPort1.Read(buffer, 0, buffersize);
            string ResultStatus;
            ResultStatus = byteToHexStr(buffer);//字节数组转换为字符串，器件状态已获取 
             

            //3.状态校验（下一器件是否打开）
            //ResultStatus.Substring();
            //if （）
            //{

            //}


            //4.
            return result;
        }
        public  void GetStatus() {//判断器件状态的方法
            byte[] buffin = new byte[] { 0XAA, 0X08, 0X03, 0X00, 0X00, 0XCE, 0X78, 0XCC };//判断器件状态的通用通讯指令
            serialPort1.Write(buffin, 0, 8);
            int a = serialPort1.Read(buffer, 0, buffersize);
            string ss;
            ss = byteToHexStr(buffer); //用到函数byteToHexStr字节数组转换为字符串
                                       //  textBox1.Text = ss;
            DateTime dt = DateTime.Now;
            while (serialPort1.BytesToRead < 2)
            {
                Thread.Sleep(1);

                if (DateTime.Now.Subtract(dt).TotalMilliseconds > 5000) //如果5秒后仍然无数据返回，则视为超时
                {
                    MessageBox.Show("主版无响应");
                }
            }


        }





        private void button1_Click(object sender, EventArgs e)//器件1开
        {
            //WriteByteToSerialPort(DeviceOpen1);

            //serialPort1.DiscardInBuffer();
            //serialPort1.DiscardOutBuffer();
            try
            {
               
              //  KeepStatus();
                KeepStatus();//维持器件之前状态不会改变的方法
                string OpenNumber = DeviceOpenNumber(18);//单控开关的 打开指定器件方法
                                                         //再次下发指令时，仅为单控开关的指令
                serialPort1.DiscardInBuffer();
                string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
                string commanded = command + "CC";//完整通讯指令合成
                byte[] byteArray2 = strToHexByte1(commanded);
                serialPort1.Write(byteArray2, 0, 8);
                //  byte[] buff = new byte[] { 0XAA,0X08,0X02,0X00,0X01,0X5E,0X78,0XCC };//仅打开1-2
                // serialPort1.Write(buff, 0, 8);
                //serialPort1.DiscardInBuffer();
                //serialPort1.DiscardOutBuffer();
                
                ovalShape11.FillColor = Color.Green;

            }
            catch (Exception E)
            {
                ovalShape11.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }
      

         

            // ovalShape11.FillColor = Color.Green;
        }

        private void button2_Click(object sender, EventArgs e)//器件1关
        {
            try
            {
                KeepStatus();
                string CloseNumber = DeviceCloseNumber(18);//单控开关的 打开指定器件方法
                                                           //再次下发指令时，仅为单控开关的指令
                serialPort1.DiscardInBuffer();
                string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
                string commanded = command + "CC";//完整通讯指令合成
                byte[] byteArray2 = strToHexByte1(commanded);
                serialPort1.Write(byteArray2, 0, 8);
                ovalShape11.FillColor = Color.Red;
            }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }

           


        }
        public byte[] strToHexByte1(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        private void button3_Click(object sender, EventArgs e)//器件2开
        {
            try
            {

           
            KeepStatus();//维持器件之前状态不会改变的方法
             string OpenNumber= DeviceOpenNumber(17);//单控开关的 打开指定器件方法
             //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape12.FillColor = Color.Green;
            //byte[] buff = new byte[] { 0XAA, 0X08, 0X02, 0X00, 0X05, 0X5F, 0XBB, 0XCC };//仅1和3
            //serialPort1.Write(buff, 0, 8);
            //ovalShape11.FillColor = Color.Green;
            // WriteByteToSerialPort(DeviceOpen2);


        }
            catch (Exception E)
            {
                ovalShape12.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }
}

        private void button4_Click(object sender, EventArgs e)//器件2关
        {
            try
            {

           
            //WriteByteToSerialPort(DeviceClose2);
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(17);//单控开关的 打开指定器件方法
                                                     //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape12.FillColor = Color.Red;
            }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }

        }

        private void button5_Click(object sender, EventArgs e)//器件3开
        {
            try
            {

          
            // WriteByteToSerialPort(DeviceOpen3);
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(16);//单控开关的 打开指定器件方法
           //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape13.FillColor = Color.Green;
                //byte[] buff = new byte[] { 0XAA, 0X08, 0X02, 0X00, 0X21, 0X5F, 0XA0, 0XCC };//仅打开6
                //serialPort1.Write(buff, 0, 8);
                //ovalShape13.FillColor = Color.Green;
            }
            catch (Exception E)
            {
                ovalShape13.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }

        }

        private void button6_Click(object sender, EventArgs e)//器件3关
        {

            try
            {

           
            // WriteByteToSerialPort(DeviceClose3);
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(16);//单控开关的 打开指定器件方法
                                                      //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape13.FillColor = Color.Red;
            }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }

        }

        private void button7_Click(object sender, EventArgs e)//器件4开
        {
            try
            {

          

            //WriteByteToSerialPort(DeviceOpen4);
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(15);//单控开关的 打开指定器件方法
           //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            //byte[] buff = new byte[] { 0XAA, 0X08, 0X02, 0X00, 0X19, 0X5E, 0X72, 0XCC };//仅打开4-5
            //serialPort1.Write(buff, 0, 8);
            ovalShape14.FillColor = Color.Green;
            }
            catch (Exception E)
            {
                ovalShape14.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }


        }

        private void button8_Click(object sender, EventArgs e)//器件4关
        {
            // WriteByteToSerialPort(DeviceClose4);

            try
            {

           
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(15);//单控开关的 打开指定器件方法
                                                      //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape14.FillColor = Color.Red;
            }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }

        }

        private void button9_Click(object sender, EventArgs e)//器件5开
        {
            //  WriteByteToSerialPort(DeviceOpen5);
            //byte[] buff = new byte[] { 0XAA, 0X08, 0X02, 0X00, 0X09, 0X5F, 0XBE, 0XCC };//仅打开4-5
            //serialPort1.Write(buff, 0, 8);
            try
            {

           
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(13);//单控开关的 打开指定器件方法
                                                    //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape15.FillColor = Color.Green;
            }
            catch (Exception E)
            {
                ovalShape15.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }

        }

        private void button10_Click(object sender, EventArgs e)//器件5关
        {
            // WriteByteToSerialPort(DeviceClose5);
            try
            {

           

            KeepStatus();
            string CloseNumber = DeviceCloseNumber(13);//单控开关的 打开指定器件方法
                                                      //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape15.FillColor = Color.Red;
            }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }

        }

        private void button11_Click(object sender, EventArgs e)//器件6开
        {

            try
            {

           
            // WriteByteToSerialPort(DeviceClose5);
            KeepStatus();
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(12);//单控开关的 打开指定器件方法
                                                    //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape16.FillColor = Color.Green;
            }
            catch (Exception E)
            {
                ovalShape16.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }

        }

        private void button12_Click(object sender, EventArgs e)//器件6关
        {

            try
            {

            
            // WriteByteToSerialPort(DeviceClose5);
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(12);//单控开关的 打开指定器件方法
                                                      //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape16.FillColor = Color.Red;
            }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }
        }

        private void button13_Click(object sender, EventArgs e)//器件7开
        {
            try
            {


                // WriteByteToSerialPort(DeviceClose5);
                KeepStatus();
                KeepStatus();//维持器件之前状态不会改变的方法
                string OpenNumber = DeviceOpenNumber(11);//单控开关的 打开指定器件方法
                                                         //再次下发指令时，仅为单控开关的指令
                serialPort1.DiscardInBuffer();
                string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
                string commanded = command + "CC";//完整通讯指令合成
                byte[] byteArray2 = strToHexByte1(commanded);
                serialPort1.Write(byteArray2, 0, 8);
                ovalShape17.FillColor = Color.Green;
            }
            catch (Exception E)
            {
                ovalShape17.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }

        }

        private void button14_Click(object sender, EventArgs e)//器件7关
        {
            // WriteByteToSerialPort(DeviceClose5);

            try
            {

           
          
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(11);//单控开关的 打开指定器件方法
                                                      //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape17.FillColor = Color.Red;
        }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }
}

        private void button17_Click(object sender, EventArgs e)//器件8开
        {
            // WriteByteToSerialPort(DeviceClose5);
            try
            {

           
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(10);//单控开关的 打开指定器件方法
                                                    //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape18.FillColor = Color.Green;
        }
            catch (Exception E)
            {
                ovalShape18.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }

}

        private void button18_Click(object sender, EventArgs e)//器件8关
        {
            // WriteByteToSerialPort(DeviceClose5);

            try
            {

          
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(10);//单控开关的 打开指定器件方法
                                                      //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape18.FillColor = Color.Red;
            }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }
        }

        private void button21_Click(object sender, EventArgs e)//器件9开
        {
            //  WriteByteToSerialPort(DeviceClose5);
            try
            {

          
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(8);//单控开关的 打开指定器件方法
                                                    //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape27.FillColor = Color.Green;
        }
            catch (Exception E)
            {
                ovalShape27.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }
}

        private void button22_Click(object sender, EventArgs e)//器件9关
        {
            // WriteByteToSerialPort(DeviceClose5);
            try
            {

           
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(8);//单控开关的 打开指定器件方法
                                                       //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape27.FillColor = Color.Red;
        }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }
}

        private void button23_Click(object sender, EventArgs e)//器件10开
        {
            //  WriteByteToSerialPort(DeviceClose5);
            try
            {

          
            KeepStatus();
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(7);//单控开关的 打开指定器件方法
                                                    //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape26.FillColor = Color.Green;
        }
            catch (Exception E)
            {
                ovalShape26.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }
}

        private void button24_Click(object sender, EventArgs e)//器件10关
        {
            // WriteByteToSerialPort(DeviceClose5);
            try
            {

           
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(7);//单控开关的 打开指定器件方法
                                                       //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape26.FillColor = Color.Red;
        }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }
}

        private void button25_Click(object sender, EventArgs e)//器件11开
        {
            // WriteByteToSerialPort(DeviceClose5);
            try
            {

           
            KeepStatus();
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(6);//单控开关的 打开指定器件方法
                                                    //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape25.FillColor = Color.Green;
        }
            catch (Exception E)
            {
                ovalShape25.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }
}

        private void button26_Click(object sender, EventArgs e)//器件11关
        {
            // WriteByteToSerialPort(DeviceClose5);
            try
            {

           
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(6);//单控开关的 打开指定器件方法
                                                       //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape25.FillColor = Color.Red;
        }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }
}

        private void button27_Click(object sender, EventArgs e)//器件12开
        {
            //  WriteByteToSerialPort(DeviceClose5);
            try
            {

           
            KeepStatus();
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(5);//单控开关的 打开指定器件方法
                                                    //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape24.FillColor = Color.Green;
        }
            catch (Exception E)
            {
                ovalShape24.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }
}

        private void button28_Click(object sender, EventArgs e)//器件12关
        {
            // WriteByteToSerialPort(DeviceClose5);
            try
            {

          
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(5);//单控开关的 打开指定器件方法
                                                       //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape24.FillColor = Color.Red;
        }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }
}

        private void button29_Click(object sender, EventArgs e)//器件13开
        {
            // WriteByteToSerialPort(DeviceClose5);
            try
            {

           
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(3);//单控开关的 打开指定器件方法
                                                    //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape23.FillColor = Color.Green;
        }
            catch (Exception E)
            {
                ovalShape23.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }
}

        private void button30_Click(object sender, EventArgs e)//器件13关
        {
            // WriteByteToSerialPort(DeviceClose5);

            try
            {

           
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(3);//单控开关的 打开指定器件方法
                                                       //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape23.FillColor = Color.Red;
            }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }


        }

        private void button31_Click(object sender, EventArgs e)//器件14开
        {
            // WriteByteToSerialPort(DeviceClose5);
            try
            {

          
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(2);//单控开关的 打开指定器件方法
                                                    //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape22.FillColor = Color.Green;
            }
            catch (Exception E)
            {
                ovalShape22.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }
        }

        private void button32_Click(object sender, EventArgs e)//器件14关
        {
            // WriteByteToSerialPort(DeviceClose5);
            try
            {

            
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(2);//单控开关的 打开指定器件方法
                                                       //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape22.FillColor = Color.Red;
        }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }



}

        private void button33_Click(object sender, EventArgs e)//器件15开
        {
            //  WriteByteToSerialPort(DeviceClose5);

            try
            {

           
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(1);//单控开关的 打开指定器件方法
                                                    //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape21.FillColor = Color.Green;
        }
            catch (Exception E)
            {
                ovalShape21.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }

}

        private void button34_Click(object sender, EventArgs e)//器件15关
        {
            // WriteByteToSerialPort(DeviceClose5);
            try
            {

           
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(1);//单控开关的 打开指定器件方法
                                                       //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape21.FillColor = Color.Red;
        }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }

}

        private void button35_Click(object sender, EventArgs e)//器件16开
        {
            // WriteByteToSerialPort(DeviceClose5);

            try
            {

           
            KeepStatus();//维持器件之前状态不会改变的方法
            string OpenNumber = DeviceOpenNumber(0);//单控开关的 打开指定器件方法
                                                    //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(OpenNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape20.FillColor = Color.Green;
        }
            catch (Exception E)
            {
                ovalShape20.FillColor = Color.Red;
                MessageBox.Show(E.ToString());
            }

}

        private void button36_Click(object sender, EventArgs e)//器件16关
        { // WriteByteToSerialPort(DeviceClose5);
            try
            {

          
            KeepStatus();
            string CloseNumber = DeviceCloseNumber(0);//单控开关的 打开指定器件方法
                                                       //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(CloseNumber); //CRC校验数据结果  
            string commanded = command + "CC";//完整通讯指令合成
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);
            ovalShape20.FillColor = Color.Red;
        }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }

}
        private void button20_Click(object sender, EventArgs e)//器件全关
        {
            try
            {

          
            string result = CRCForModbus(AllDeviecClose); //CRC校验数据结果
            string resulted = result + "CC";//完整通讯指令合成
            byte[] byteArray = strToHexByte1(resulted);
            serialPort1.Write(byteArray, 0, 8);
            ovalShape11.FillColor = Color.Red;
            ovalShape12.FillColor = Color.Red;
            ovalShape13.FillColor = Color.Red;
            ovalShape14.FillColor = Color.Red;
            ovalShape15.FillColor = Color.Red;
            ovalShape16.FillColor = Color.Red;
            ovalShape17.FillColor = Color.Red;
            ovalShape18.FillColor = Color.Red;
            ovalShape19.FillColor = Color.Red;
            ovalShape20.FillColor = Color.Red;
            ovalShape21.FillColor = Color.Red;
            ovalShape22.FillColor = Color.Red;
            ovalShape23.FillColor = Color.Red;
            ovalShape24.FillColor = Color.Red;
            ovalShape25.FillColor = Color.Red;
            ovalShape26.FillColor = Color.Red;
            ovalShape27.FillColor = Color.Red;
           // string Input = KeepStatus();//0000 0000 0000 0000 

          //  textBox1.Text = Input;//返回信息位为上一步操作中信息

        }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }
}

        private void button19_Click(object sender, EventArgs e)//器件全开
        {
            try
            {

           
            string result = CRCForModbus(AllDeviceOpen); //CRC校验数据结果
            string resulted = result + "CC";//完整通讯指令合成
            byte[] byteArray = strToHexByte1(resulted);
            serialPort1.Write(byteArray, 0, 8);
            ovalShape11.FillColor = Color.Green;
            ovalShape12.FillColor = Color.Green;
            ovalShape13.FillColor = Color.Green;
            ovalShape14.FillColor = Color.Green;
            ovalShape15.FillColor = Color.Green;
            ovalShape16.FillColor = Color.Green;
            ovalShape17.FillColor = Color.Green;
            ovalShape18.FillColor = Color.Green;
            ovalShape19.FillColor = Color.Green;
            ovalShape20.FillColor = Color.Green;
            ovalShape21.FillColor = Color.Green;
            ovalShape22.FillColor = Color.Green;
            ovalShape23.FillColor = Color.Green;
            ovalShape24.FillColor = Color.Green;
            ovalShape25.FillColor = Color.Green;
            ovalShape26.FillColor = Color.Green;
            ovalShape27.FillColor = Color.Green;
            //string Input = KeepStatus();//0000 0000 0000 0000 

            //textBox1.Text = Input;//返回信息位为上一步操作中信息
        }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }


}
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void ovalShape1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            this.BackColor= SystemColors.Control;
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {

                float db = float.Parse(textBox2.Text);
                float dbs = db * 100;
                int s = Convert.ToInt16(dbs);
                byte[] buff1 = new byte[8] { 0XAA, 0X08, 0X02, 0X00, 0X01, 0X5E, 0X78, 0XCC };//
                serialPort1.Write(buff1, 0, 8);

                Thread.Sleep(s);
                byte[] buff2 = new byte[8] { 0XAA, 0X08, 0X02, 0X00, 0X03, 0XDF, 0XB9, 0XCC };//
                serialPort1.Write(buff2, 0, 8);
                Thread.Sleep(s);
                byte[] buff3 = new byte[8] { 0XAA, 0X08, 0X02, 0X00, 0X07, 0XDE, 0X7A, 0XCC };//
                serialPort1.Write(buff3, 0, 8);
                Thread.Sleep(s);
                byte[] buff4 = new byte[8] { 0XAA, 0X08, 0X02, 0X00, 0X0F, 0XDF, 0XBC, 0XCC };//
                serialPort1.Write(buff4, 0, 8);
                Thread.Sleep(s);
                byte[] buff5 = new byte[8] { 0XAA, 0X08, 0X02, 0X00, 0X1F, 0XDE, 0X70, 0XCC };//
                serialPort1.Write(buff5, 0, 8);
                Thread.Sleep(s);
                byte[] buff6 = new byte[8] { 0XAA, 0X08, 0X02, 0X00, 0X3F, 0XDF, 0XA8, 0XCC };//
                serialPort1.Write(buff6, 0, 8);
                Thread.Sleep(s);
                byte[] buff7 = new byte[8] { 0XAA, 0X08, 0X02, 0X00, 0X7F, 0XDE, 0X58, 0XCC };//
                serialPort1.Write(buff7, 0, 8);
                Thread.Sleep(s);
                byte[] buff8 = new byte[8] { 0XAA, 0X08, 0X02, 0X00, 0XFF, 0XDF, 0XF8, 0XCC };//
                serialPort1.Write(buff8, 0, 8);
                Thread.Sleep(s);
                byte[] buff9 = new byte[8] { 0XAA, 0X08, 0X02, 0X01, 0XFF, 0XDE, 0X68, 0XCC };//
                serialPort1.Write(buff9, 0, 8);
                Thread.Sleep(s);
                byte[] buff10 = new byte[8] { 0XAA, 0X08, 0X02, 0X03, 0XFF, 0XDF, 0X08, 0XCC };//
                serialPort1.Write(buff10, 0, 8);
                Thread.Sleep(s);
                byte[] buff11 = new byte[8] { 0XAA, 0X08, 0X02, 0X07, 0XFF, 0XDD, 0XC8, 0XCC };//
                serialPort1.Write(buff11, 0, 8);
                Thread.Sleep(s);
                byte[] buff12 = new byte[8] { 0XAA, 0X08, 0X02, 0X0F, 0XFF, 0XDA, 0X08, 0XCC };//
                serialPort1.Write(buff12, 0, 8);
                Thread.Sleep(s);
                byte[] buff13 = new byte[8] { 0XAA, 0X08, 0X02, 0X1F, 0XFF, 0XD7, 0XC8, 0XCC };//
                serialPort1.Write(buff13, 0, 8);
                Thread.Sleep(s);
                byte[] buff14 = new byte[8] { 0XAA, 0X08, 0X02, 0X3F, 0XFF, 0XCE, 0X08, 0XCC };//
                serialPort1.Write(buff14, 0, 8);
                Thread.Sleep(s);
                byte[] buff15 = new byte[8] { 0XAA, 0X08, 0X02, 0X7F, 0XFF, 0XFF, 0XC8, 0XCC };//
                serialPort1.Write(buff15, 0, 8);
                Thread.Sleep(s);
                byte[] buff16 = new byte[8] { 0XAA, 0X08, 0X02, 0XFF, 0XFF, 0XFF, 0XC8, 0XCC };//
                serialPort1.Write(buff16, 0, 8);
                Thread.Sleep(s);
                byte[] buffAll = new byte[8] { 0XAA, 0X08, 0X02, 0X00, 0X00, 0X9F, 0XB8, 0XCC };
                serialPort1.Write(buffAll, 0, 8);
                Thread.Sleep(s);
            }
            catch (Exception E)
            {

                MessageBox.Show(E.ToString());
            }
        }



        private void button38_Click(object sender, EventArgs e)//计时器开关
        {
            // timer1.Interval =  100;
            //   timer1.Start();

            try
            {
                if (textBox2.Text == String.Empty)
                {
                    MessageBox.Show("请输入循环间隔时间！");
                }
            }
            catch
            { }
            if (button38.Text == "停止")

            {
                timer1.Enabled = false;
                button38.Text = "开始";

            }
            else
            {
                button38.Text = "停止";
                timer1.Enabled = true;
            };
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)//限制文本输入框的值仅为数字 
        {
            if (!(char.IsNumber(e.KeyChar)) && e.KeyChar != (char)(8))
            {
                e.Handled = true;
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button37_Click(object sender, EventArgs e)//通道状态显示的按钮
        {

            ovalShape28.FillColor = Color.Red;
            ovalShape29.FillColor = Color.Red;
            ovalShape30.FillColor = Color.Red;
            ovalShape31.FillColor = Color.Red;
            ovalShape32.FillColor = Color.Red;
            ovalShape33.FillColor = Color.Red;
            ovalShape34.FillColor = Color.Red;
            ovalShape35.FillColor = Color.Red;
            serialPort1.DiscardInBuffer();
            serialPort1.DiscardOutBuffer();
            byte[] bufferin = new byte[] { 0XAA, 0X08, 0X01, 0X00, 0X00, 0X6F, 0XB8, 0XCC };//判断通道状态的通用指令
            serialPort1.Write(bufferin, 0, 8);
            Thread.Sleep(200);
            int number = serialPort1.Read(buffer, 0, buffersize);

            string LineStatus;
            LineStatus = byteToHexStr(buffer);//使用封装好的函数，将获取到串口返回值字节转换为字符串
           // textBox1.Text = LineStatus;

            string FunctionNumber = LineStatus.Substring(8, 2);//获取到对应状态功能码 
           // MessageBox.Show(FunctionNumber);
               
            string function = HexString2BinString(FunctionNumber);//将十六进制转换为二进制
            textBox1.Text = function;
           // MessageBox.Show(function);
            string line1 = function.Substring(0,1);//通道一值
            string line2 = function.Substring(1,1);//通道二值
            string line3 = function.Substring(2,1);//通道三值
            string line4 = function.Substring(3,1);//通道四值
            string line5 = function.Substring(5,1);//通道五值
            string line6 = function.Substring(6,1);//通道六值
            string line7 = function.Substring(7,1);//通道七值
            string line8 = function.Substring(8,1);//通道八值
            if (line1=="1") {
                MessageBox.Show("通道一开启成功！");
                ovalShape28.FillColor = Color.Green;
            }
            if (line2 == "1")
            {
                MessageBox.Show("通道2开启成功！");
                ovalShape29.FillColor = Color.Green;
            }
            if (line3 == "1")
            {
                MessageBox.Show("通道3开启成功！");
                ovalShape30.FillColor = Color.Green;
            }
            if (line4 == "1")
            {
                MessageBox.Show("通道4开启成功！");
                ovalShape31.FillColor = Color.Green;
            }
            if (line5 == "1")
            {
                MessageBox.Show("通道5开启成功！");
                ovalShape32.FillColor = Color.Green;
            }
            if (line6 == "1")
            {
                MessageBox.Show("通道6开启成功！");
                ovalShape33.FillColor = Color.Green;
            }
            if (line7 == "1")
            {
                MessageBox.Show("通道7开启成功！");
                ovalShape34.FillColor = Color.Green;
            }
            if (line8 == "1")
            {
                MessageBox.Show("通道8开启成功！");
                ovalShape35.FillColor = Color.Green;
            }
        }
        static string HexString2BinString(string hexString)//十六进制转换为二进制的方法
        {
            string result = string.Empty;
            foreach (char c in hexString)
            {
                int v = Convert.ToInt32(c.ToString(), 16);
                int v2 = int.Parse(Convert.ToString(v, 2));
                // 去掉格式串中的空格，即可去掉每个4位二进制数之间的空格，
                result += string.Format("{0:d4} ", v2);
            }
            return result;
        }
        public string KeepStatus() {  //保持器件状态的方法

            try
            {


                byte[] buffin = new byte[] { 0XAA, 0X08, 0X03, 0X00, 0X00, 0XCE, 0X78, 0XCC };//判断器件状态的通用通讯指令
                serialPort1.Write(buffin, 0, 8);
                Thread.Sleep(100);
                int a = serialPort1.Read(buffer, 0, buffersize);
              

                string b = byteToHexStr(buffer);
                //  textBox1.Text = b;

                string FunctionNumber = b.Substring(6, 4);//获取到对应状态功能码 
                                                          // textBox1.Text = FunctionNumber;                                                   
                                                          //   MessageBox.Show(FunctionNumber);

                string function = HexString2BinString(FunctionNumber);//将十六进制转换为二进制   对应位二进制值已经获取
                                                                      //  MessageBox.Show(function);

                return function;

            }
            catch (Exception r)
            {
                return null;
                MessageBox.Show(r.ToString());

            }
        
    }
        public string DeviceOpenNumber(int number) {//按照器件的编号替换  指定功能码中 二进制值为 1的方法  指定单开器件
            string F = "F";
            // KeepStatus();
            string Input = KeepStatus();//0000 0000 0000 0000 
           // string EndCommand;
           // MessageBox.Show(Input);
           textBox1.Text = Input;
            List<string> lst = new List<string>();
           
            foreach (var item in Input)
            {
                
              //  MessageBox.Show(Convert.ToString(item));
                lst.Add(Convert.ToString(item));
            }
           // lst.Remove(" ");
            lst[number] ="1";//此处为需要改变的功能位
            //textBox1.Text = lst[number];
         //   MessageBox.Show(lst[7]);
                int a = 0;
                int b = 0;
                int c = 0;
                int d = 0;
           
                if (lst[18] == "1") {//判断器件1打开
                    a = a + 1;
                }
                if (lst[17] == "1")//判断器件2打开
                {
                    a = a + 2;
                }
                if (lst[16] == "1")//判断器件3打开
                {
                    a = a + 4;
                }
                if (lst[15] == "1")//判断器件4打开
                {
                    a = a + 8;
               
                }
            string aHexString = Convert.ToString(a,16);
         //  MessageBox.Show("a1=" + Convert.ToString(aHexString));
                if (lst[13] == "1")//判断器件5打开
                {
                    b = b + 1;
                }
                if (lst[12] == "1")//判断器件6打开
                {
                    b = b + 2;
                }
                if (lst[11] == "1")//判断器件7打开
                {
                    b = b + 4;
                }
                if (lst[10] == "1")//判断器件8打开
                {
                    b = b + 8;
                }
            string bHexString = Convert.ToString(b, 16);
         //  MessageBox.Show("b1=" + Convert.ToString(bHexString));
                if (lst[8] == "1")//判断器件9打开
                {
                    c = c + 1;
                }
                if (lst[7] == "1")//判断器件10打开
                {
                    c = c + 2;
                }
                if (lst[6] == "1")//判断器件11打开
                {
                    c = c + 4;
                }
                if (lst[5] == "1")//判断器件12打开
                {
                    c = c + 8;
                }
            string cHexString = Convert.ToString(c, 16);
        //   MessageBox.Show("c1=" + Convert.ToString(cHexString));
                if (lst[3] == "1")//判断器件13打开
                {
                    d = d + 1;
                }
                if (lst[2] == "1")//判断器件14打开
                {
                    d = d + 2;
                }
                if (lst[1] == "1")//判断器件15打开
                {
                    d = d + 4;
                }
                if (lst[0] == "1")//判断器件16打开
                {
                    d = d + 8;
                }
            string dHexString = Convert.ToString(d, 16);
        //    MessageBox.Show("d1=" + Convert.ToString(dHexString));
            string FunctionHexString = "AA0802"+ dHexString + cHexString + bHexString + aHexString;//完整功能位字节转换位16进制拼接转换完成
         //   MessageBox.Show(FunctionHexString);
            //if (a!=15||b!=15||c!=15||d!=15)
            //{
            //    a = a;
            //    b = int.Parse(F);
            //    c = int.Parse(F);
            //    d = int.Parse(F);

            //}

            return FunctionHexString;
        }
        public string DeviceCloseNumber(int number)
        {//按照器件的编号替换  指定功能码中 二进制值为 1的方法  指定单开器件
            string F = "F";
            // KeepStatus();
            string Input = KeepStatus();//0000 0000 0000 0000 
                                        // string EndCommand;
        //    MessageBox.Show(Input);
            textBox1.Text = Input;
            List<string> lst = new List<string>();

            foreach (var item in Input)
            {

                //  MessageBox.Show(Convert.ToString(item));
                lst.Add(Convert.ToString(item));
            }
            // lst.Remove(" ");
            lst[number] = "0";//此处为需要改变的功能位
           // textBox1.Text = lst[number];
            //   MessageBox.Show(lst[7]);
            int a = 0;
            int b = 0;
            int c = 0;
            int d = 0;

            if (lst[18] == "1")
            {//判断器件1打开
                a = a + 1;
            }
            if (lst[17] == "1")//判断器件2打开
            {
                a = a + 2;
            }
            if (lst[16] == "1")//判断器件3打开
            {
                a = a + 4;
            }
            if (lst[15] == "1")//判断器件4打开
            {
                a = a + 8;

            }
            string aHexString = Convert.ToString(a, 16);
            //  MessageBox.Show("a1=" + Convert.ToString(aHexString));
            if (lst[13] == "1")//判断器件5打开
            {
                b = b + 1;
            }
            if (lst[12] == "1")//判断器件6打开
            {
                b = b + 2;
            }
            if (lst[11] == "1")//判断器件7打开
            {
                b = b + 4;
            }
            if (lst[10] == "1")//判断器件8打开
            {
                b = b + 8;
            }
            string bHexString = Convert.ToString(b, 16);
            //  MessageBox.Show("b1=" + Convert.ToString(bHexString));
            if (lst[8] == "1")//判断器件9打开
            {
                c = c + 1;
            }
            if (lst[7] == "1")//判断器件10打开
            {
                c = c + 2;
            }
            if (lst[6] == "1")//判断器件11打开
            {
                c = c + 4;
            }
            if (lst[5] == "1")//判断器件12打开
            {
                c = c + 8;
            }
            string cHexString = Convert.ToString(c, 16);
            //   MessageBox.Show("c1=" + Convert.ToString(cHexString));
            if (lst[3] == "1")//判断器件13打开
            {
                d = d + 1;
            }
            if (lst[2] == "1")//判断器件14打开
            {
                d = d + 2;
            }
            if (lst[1] == "1")//判断器件15打开
            {
                d = d + 4;
            }
            if (lst[0] == "1")//判断器件16打开
            {
                d = d + 8;
            }
            string dHexString = Convert.ToString(d, 16);
            //    MessageBox.Show("d1=" + Convert.ToString(dHexString));
            string FunctionHexString = "AA0802" + dHexString + cHexString + bHexString + aHexString;//完整功能位字节转换位16进制拼接转换完成
            //   MessageBox.Show(FunctionHexString);
             //if (a!=15||b!=15||c!=15||d!=15)
             //{
            //    a = a;
            //    b = int.Parse(F);
            //    c = int.Parse(F);
            //    d = int.Parse(F);

            //}

            return FunctionHexString;
        }
        private void button39_Click(object sender, EventArgs e)
        {
            
           KeepStatus();//维持器件之前状态不会改变的方法
           //DeviceOpenNumber(6);
            //再次下发指令时，仅为单控开关的指令
            serialPort1.DiscardInBuffer();
            string command = CRCForModbus(DeviceOpenNumber(3)); //CRC校验数据结果  此时打开全部
            string commanded = command + "CC";//完整通讯指令合成
                                              //   MessageBox.Show(commanded);
            byte[] byteArray2 = strToHexByte1(commanded);
            serialPort1.Write(byteArray2, 0, 8);



        }

        private void button40_Click(object sender, EventArgs e)
        {
            MessageBox.Show("cccc");
        }
    }
}
    


