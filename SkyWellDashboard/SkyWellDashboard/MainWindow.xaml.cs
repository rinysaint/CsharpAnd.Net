/************************************
*公司名称：开沃集团
*命名空间：SkyWellDashboard
*文件名：MainWindow.xaml.cs
*版本号：V1.0.0.0
*创建人:肖日宁
*电子邮件：a11010203@qq.com
*联系方式:QQ:164840753
*创建时间：2019年3月27日
*描述：主窗体文件
*************************************/
using log4net;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SkyWellDashboard
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 全局变量
        /// <summary>
        /// 用于UDP发送的网络服务类
        /// </summary>
        private UdpClient udpcSend;
        /// <summary>
        /// 用于UDP接收的网络服务类
        /// </summary>
        private UdpClient udpcRecv;
        /// <summary>
        /// 开关：在监听UDP报文阶段为true，否则为false
        /// </summary>
        bool IsUdpcRecvStart = false;
        /// <summary>
        /// 线程：不断监听UDP报文
        /// </summary>
        Thread thrRecv;
        /// <summary>
        /// 线程：监听UDP端口
        /// </summary>
        IPEndPoint localIpep = new IPEndPoint(IPAddress.Parse("172.16.5.26"), 8899); // 本机IP和监听端口号

        /// <summary>
        /// 用于PING的网络地址
        /// </summary>
        private string pingip = "192.168.0.103";

        static int pingcount = 0, pinglosscount = 0;
        static Thickness tkwheelmiddle, tkwheeltopleft, tkwheeltopright;   
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            //窗口全屏
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            init("");
            UDPRecv();
        }
        


        public void init(string Messages)
        {
            try
            {
                if (!string.IsNullOrEmpty(Messages))
                { }
                else
                {
                    //星期显示
                    textBlock_week.Text = DateTime.Now.ToString("dddd");
                    //日期显示
                    textBlock_date.Text = DateTime.Now.ToString("D");
                    //时间显示
                    textBlock_Time.Text = DateTime.Now.ToString("t");

                    //模式
                    label_Mode.Content = "自动模式";
                    label_ChangeMode.Content = "切换到手动模式";

                    //车轮转角
                    double corner = 155.843 / 15.43;
                    textBlock_corner.Text = corner + " °";

                    //360环视视频
                    //初始化配置，指定引用库
                    try
                    {
                        myVideo.MediaPlayer.VlcLibDirectoryNeeded += OnVlcControlNeedsLibDirectory;
                        myVideo.MediaPlayer.EndInit();
                        string[] arguments = { ":network-caching=90" };
                        myVideo.MediaPlayer.Play(new Uri("rtsp://172.16.5.168:/6"), arguments);
                        //forvideos();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    //网络延时
                    textBlock_networkdelay.Text = "30 ms";

                    //电机转速
                    textBlock_imgenginespeed.Text = "3";

                    //SOC电池百分比
                    textBlock_imgElectricQuantity.Text = "90";

                    //电机温度
                    textBlock_MotorTEMP.Text = "30" + " ℃";

                    //电池温度
                    textBlock_BatteryTEMP.Text = "60" + " ℃";

                    //车辆速度
                    textBlock_Speed.Text = "66";

                    //档位状态
                    imgdrivestatus.Source = new BitmapImage(new Uri(@"img/D.png", UriKind.Relative));

                    //车辆速度
                    textBlock_Speed.Text = "66";

                    //行驶时间
                    textBlock_BatteryVoltage.Text = "72" + " V";

                    //行驶时间
                    textBlock_BatteryCurrent.Text = "48" + " A";

                    //车门状态
                    textBlock_DoorStatus.Text = "开";
                    //更新
                    Thread_textblockupdate();
                    //保存图片位置
                    tkwheelmiddle = imgwheelmiddle.Margin;
                    tkwheeltopleft = imgwheeltopleft.Margin;
                    tkwheeltopright = imgwheeltopright.Margin;
                }
            }
            catch (Exception e)
            {
                AppLog.Error(e.Message);
            }
        }

        private void Thread_textblockupdate()
        {
            //启动线程处理
            Thread threadupdate = new Thread(UpdateTextblock);
            threadupdate.IsBackground = true;//设置为后台线程，当主线程结束后，后台线程自动退出，否则不会退出程序不能结束
            threadupdate.Start();
        }
        //线程方法，修改textBlock内容
        private void UpdateTextblock()
        {
            try
            {
                int i = 0;
                while (true)
                {
                    Action action0 = () =>
                    {
                        //1.星期显示
                        textBlock_week.Text = DateTime.Now.ToString("dddd");
                        //1.日期显示
                        textBlock_date.Text = DateTime.Now.ToString("D");
                        //1.时间显示
                        textBlock_Time.Text = DateTime.Now.ToString("t");
                        //5.网络延迟
                        PingMethod(pingip);
                        
                        //if (myVideo.MediaPlayer.State != Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                        //{
                        //    string[] arguments = { ":network-caching=90" };
                        //    myVideo.MediaPlayer.Play(new Uri("rtsp://172.16.5.168:/6"), arguments);
                        //}
                    };
                    textBlock_Time.Dispatcher.BeginInvoke(action0);
                    // 如果不设置等待，整个程序死循环
                    Thread.Sleep(1000);
                    i++;
                    if (i > 100)
                    {
                        i = 0;
                    }
                }
            }
            catch (Exception e)
            {
                AppLog.Error(e.Message);
            }
        }

        /// <summary>
        /// 接收UDP数据开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UDPRecv()
        {
            if (!IsUdpcRecvStart) // 未监听的情况，开始监听
            {
                try
                {
                    udpcRecv = new UdpClient(localIpep);
                    thrRecv = new Thread(ReceiveMessage);
                    thrRecv.Start();
                    IsUdpcRecvStart = true;
                    writelog("UDP监听器已成功启动");
                }
                catch (Exception e)
                { writelog(e.Message); }
            }
            else // 正在监听的情况，终止监听
            {
                thrRecv.Abort(); // 必须先关闭这个线程，否则会异常
                udpcRecv.Close();
                IsUdpcRecvStart = false;
                writelog("UDP监听器已成功关闭");
            }
        }


        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="obj"></param>
        private void ReceiveMessage(object obj)
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    //读取报文
                    byte[] bytRecv = udpcRecv.Receive(ref remoteIpep);
                    if (bytRecv.Length > 0)
                    {
                        //byte[] message = new byte[bytRecv.Length];
                        //Array.Copy(bytRecv, message, bytRecv.Length);
                        if ((bytRecv[0]).ToString("X2") != "")
                        {
                            int i = 0;
                            Action action1 = () =>
                            {
                                //数据长度有效性判断
                                if (bytRecv.Length >= 13)
                                {
                                    for (int msgcount = 0; msgcount < bytRecv.Length / 13; msgcount++)
                                    {
                                        StringBuilder strbd = new StringBuilder();
                                        try {
                                            strbd.Append(bytRecv[1].ToString("X2")).Append(bytRecv[2].ToString("X2")).Append(bytRecv[3].ToString("X2")).Append(bytRecv[4].ToString("X2"));
                                            switch (strbd.ToString())
                                            {
                                                //远程控制器 EPS 命令
                                                case "1801B0C0":
                                                    //异或校验
                                                    byte VerifyByte = bytRecv[5];
                                                    for (int msgi = 6; msgi < bytRecv.Length - 1; msgi++)
                                                    {
                                                        if (bytRecv[msgi].ToString() == "0") { continue; }
                                                        VerifyByte = (byte)(VerifyByte ^ bytRecv[msgi]);
                                                    }
                                                    //校验通过
                                                    if (VerifyByte.ToString("X2") == bytRecv[bytRecv.Length - 1].ToString("X2"))
                                                    {
                                                        //3.方向盘角度
                                                        if (is16Number(bytRecv[8].ToString("X2")))
                                                        {
                                                            double corner = (Convert.ToDouble(Convert.ToInt16(((bytRecv[8]).ToString("X2") + bytRecv[9].ToString("X2")),16)) / 10) - 1080;
                                                            //3.车轮角度
                                                            if (corner >= -694.35 && corner <= 694.35)
                                                            {
                                                                double wheelcorner = Math.Round(corner / 15.43, 1);
                                                                textBlock_corner.Text = wheelcorner + "°";
                                                                double width = imgwheeltopleft.ActualWidth;
                                                                double height = imgwheeltopleft.ActualHeight;
                                                                double width2 = imgwheeltopright.ActualWidth;
                                                                double height2 = imgwheeltopright.ActualHeight;
                                                                imgwheelmiddle.Margin = tkwheelmiddle; imgwheeltopleft.Margin = tkwheeltopleft; imgwheeltopright.Margin = tkwheeltopright;
                                                                if (wheelcorner < -10 && tkwheelmiddle != null)
                                                                { imgwheelmiddle.Margin = new Thickness(tkwheelmiddle.Left, tkwheelmiddle.Top + 10, tkwheelmiddle.Right, tkwheelmiddle.Bottom - 10); imgwheeltopleft.Margin = new Thickness(tkwheeltopleft.Left, tkwheeltopleft.Top + 10, tkwheeltopleft.Right, tkwheeltopleft.Bottom - 10); }
                                                                else if(wheelcorner > 10 && tkwheeltopleft != null)
                                                                { imgwheeltopleft.Margin = new Thickness(tkwheeltopleft.Left, tkwheeltopleft.Top - 10, tkwheeltopleft.Right, tkwheeltopleft.Bottom + 10); }
                                                                imgwheeltopleft.LayoutTransform = new System.Windows.Media.RotateTransform(wheelcorner, width / 2, height / 2);
                                                                imgwheeltopright.LayoutTransform = new System.Windows.Media.RotateTransform(wheelcorner, width2 / 2, height2 / 2);
                                                            }
                                                        }
                                                    }
                                                    break;
                                                //行车状态
                                                case "1804A0B0":
                                                    //异或校验
                                                    byte VerifyByte2 = bytRecv[5];
                                                    for (int msgi = 6; msgi < bytRecv.Length - 1; msgi++)
                                                    {
                                                        if (bytRecv[msgi].ToString() == "0") { continue; }
                                                        VerifyByte2 = (byte)(VerifyByte2 ^ bytRecv[msgi]);
                                                    }
                                                    //校验通过
                                                    if (VerifyByte2.ToString("X2") == bytRecv[bytRecv.Length - 1].ToString("X2"))
                                                    {
                                                        //13.档位状态                 
                                                        StringBuilder sb = new StringBuilder();
                                                        int drivestatus = Convert.ToInt16(sb.Append(((bytRecv[5] & 32) == 32 ? 1 : 0).ToString()).Append(((bytRecv[5] & 16) == 16 ? 1 : 0).ToString()).Append(((bytRecv[5] & 8) == 8 ? 1 : 0).ToString()).Append(((bytRecv[5] & 4) == 4 ? 1 : 0).ToString()).ToString(), 2);
                                                        if (drivestatus >= 0 && drivestatus <= 3)
                                                        {
                                                            switch (drivestatus)
                                                            {
                                                                case 0:
                                                                    imgdrivestatus.Source = new BitmapImage(new Uri(@"img/N.png", UriKind.Relative));
                                                                    break;
                                                                case 1:
                                                                    imgdrivestatus.Source = new BitmapImage(new Uri(@"img/D.png", UriKind.Relative));
                                                                    break;
                                                                case 2:
                                                                    imgdrivestatus.Source = new BitmapImage(new Uri(@"img/R.png", UriKind.Relative));
                                                                    break;
                                                                case 3:
                                                                    imgdrivestatus.Source = new BitmapImage(new Uri(@"img/P.png", UriKind.Relative));
                                                                    break;
                                                                default:
                                                                    imgdrivestatus.Source = null;
                                                                    break;
                                                            }
                                                        }
                                                        //6.电机转速
                                                        StringBuilder sb2 = new StringBuilder();
                                                        if (is16Number(bytRecv[6].ToString("X2")) && is16Number(bytRecv[7].ToString("X2")))
                                                        {
                                                            double speed = (Convert.ToDouble(Convert.ToInt16(sb2.Append(bytRecv[6].ToString("X2")).Append(bytRecv[7].ToString("X2")).ToString(), 16) - 15000) / 1000);
                                                            if (speed > 0 && speed <= 7.0)
                                                            {
                                                                textBlock_imgenginespeed.Text = speed.ToString("f1");
                                                                progressbar_left.Value = speed * 10;
                                                            }
                                                            else
                                                            {
                                                                textBlock_imgenginespeed.Text = speed.ToString("f1");
                                                                progressbar_left.Value = 0;
                                                            }
                                                        }
                                                        //15.电机扭距
                                                        StringBuilder sb3 = new StringBuilder();
                                                        if (is16Number(bytRecv[8].ToString("X2")) && is16Number(bytRecv[9].ToString("X2")))
                                                        {
                                                            int mt = Convert.ToInt16(sb3.Append(bytRecv[8].ToString("X2")).Append(bytRecv[9].ToString("X2")).ToString(), 16) - 5000;
                                                            if (mt >= -5000 && mt <= 5000)
                                                            {
                                                                textBlock_MotorTorque.Text = mt.ToString() + " Nm";
                                                            }
                                                            else
                                                            {
                                                                textBlock_MotorTorque.Text = " Nm";
                                                            }
                                                        }
                                                    }
                                                    break;
                                                //车辆状态1
                                                case "1806A0B0":
                                                    //2.自驾控制模式
                                                    int sw = Convert.ToInt16(((bytRecv[5] & 2) == 2 ? 1 : 0).ToString() + ((bytRecv[5] & 0) == 0 ? 1 : 0).ToString(), 2);
                                                    if (sw >= 0 && sw <= 3)
                                                    {
                                                        switch (sw)
                                                        {
                                                            case 0:
                                                                label_Mode.Content = EnumClass.description(EnumClass.ControlModel.ManualDriving);
                                                                label_ChangeMode.Content = "切换到" + EnumClass.description(EnumClass.ControlModel.AutoPilot);
                                                                break;
                                                            case 1:
                                                                label_Mode.Content = EnumClass.description(EnumClass.ControlModel.AutoPilot);
                                                                label_ChangeMode.Content = "切换到" + EnumClass.description(EnumClass.ControlModel.ManualDriving);
                                                                break;
                                                            case 2:
                                                                label_Mode.Content = EnumClass.description(EnumClass.ControlModel.RemoteDriving);
                                                                label_ChangeMode.Content = "切换到" + EnumClass.description(EnumClass.ControlModel.AutoPilot);
                                                                break;
                                                            case 3:
                                                                label_Mode.Content = EnumClass.description(EnumClass.ControlModel.LongDistanceDriving);
                                                                label_ChangeMode.Content = "切换到" + EnumClass.description(EnumClass.ControlModel.ManualDriving);
                                                                break;
                                                            default:
                                                                label_Mode.Content = EnumClass.description(EnumClass.ControlModel.AutoPilot);
                                                                label_ChangeMode.Content = "切换到" + EnumClass.description(EnumClass.ControlModel.ManualDriving);
                                                                break;
                                                        }
                                                    }
                                                    //14.车门状态
                                                    int doorstatus = Convert.ToInt16(((bytRecv[5] & 4) == 4 ? 1 : 0).ToString(),2);
                                                    if (doorstatus == 1) { textBlock_DoorStatus.Text = "关"; } else if (doorstatus == 0) { textBlock_DoorStatus.Text = "开"; }
                                                    //10.车辆速度 取绝对值
                                                    if (isNumber(bytRecv[7].ToString()))
                                                    {
                                                        int speed = Convert.ToInt16(bytRecv[7].ToString()) - 50;
                                                        if (speed >= -50 && speed <= 250)
                                                        { textBlock_Speed.Text = Math.Abs(speed).ToString(); }
                                                    }

                                                    if (isNumber(bytRecv[8].ToString()))
                                                    //7.SOC电池百分比
                                                    {
                                                        double eq = Convert.ToInt16(bytRecv[8].ToString()) * 0.5;
                                                        if (eq > 0 && eq <= 100.0)
                                                        {
                                                            textBlock_imgElectricQuantity.Text = eq.ToString("f0");
                                                            progressbar_right.Value = Math.Round(eq * 10 / 13, 2);
                                                        }
                                                        else if (eq > 100 && eq <= 125.0)
                                                        {
                                                            textBlock_imgElectricQuantity.Text = eq.ToString("f0");
                                                            progressbar_right.Value = Math.Round(1000.0 / 13, 2);
                                                        }
                                                        else
                                                        {
                                                            textBlock_imgElectricQuantity.Text = eq.ToString("f0");
                                                            progressbar_right.Value = 0;
                                                        }
                                                    }
                                                    break;
                                                //车辆状态2
                                                case "1810A0B0":
                                                    //11.电池电压
                                                    if (is16Number(bytRecv[5].ToString("X2") + bytRecv[6].ToString("X2")))
                                                    {
                                                        int bv = Convert.ToInt16(bytRecv[5].ToString("X2") + bytRecv[6].ToString("X2"), 16);
                                                        if (bv >= 0 && bv <= 800)
                                                        { textBlock_BatteryVoltage.Text = bv * 0.2 + " V"; }
                                                    }
                                                    //12.电池电流
                                                    if (is16Number(bytRecv[7].ToString("X2") + bytRecv[8].ToString("X2")))
                                                    {
                                                        int bc = Convert.ToInt16(bytRecv[7].ToString("X2") + bytRecv[8].ToString("X2"), 16);
                                                        if (bc >= 0 && bc <= 65000)
                                                        { textBlock_BatteryCurrent.Text = (bc * 0.02) - 500 + " A"; }
                                                    }
                                                    break;
                                                //车辆状态3
                                                case "1811A0B0":
                                                    //9.电池温度，取电池组最高温度
                                                    if (is16Number(bytRecv[5].ToString("X2")))
                                                    {
                                                        int bTEMP = Convert.ToInt16(bytRecv[5].ToString("X2"), 16) - 40;
                                                        if (bTEMP >= -40 && bTEMP <= 215)
                                                        { textBlock_BatteryTEMP.Text = bTEMP.ToString() + " ℃"; }
                                                    }
                                                    //8.电机温度
                                                    if (is16Number(bytRecv[10].ToString()))
                                                    {
                                                        int mTEMP = Convert.ToInt16(bytRecv[10].ToString("X2"), 16) - 40;
                                                        if (mTEMP >= -40 && mTEMP <= 215)
                                                        { textBlock_MotorTEMP.Text = mTEMP.ToString() + " ℃"; }
                                                    }
                                                    break;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            AppLog.Error(e.Message);
                                        }
                                    }
                                }
                            };
                            textBlock_Speed.Dispatcher.BeginInvoke(action1);
                            // 如果不设置等待，整个程序死循环
                            Thread.Sleep(20);
                            i++;
                            if (i > 100)
                            {
                                i = 0;
                            }
                        }
                    }
                    else { continue; }
                }
                catch (Exception e)
                {
                    AppLog.Error(e.Message);
                    continue;
                }
            }
        }

        private void OnVlcControlNeedsLibDirectory(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (currentDirectory == null)
                return;
            e.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, @"VLC\"));
        }        


        

        /// <summary>
        /// ping命令
        /// </summary>
        /// <param name="host">发送主机名或Ip地址</param>
        /// <returns></returns>
        private void PingMethod(string host)
        {
            Ping pingSender = new Ping();
            //调用同步 send 方法发送数据,将返回结果保存至PingReply实例
            try {
                PingReply reply = pingSender.Send(hostNameOrAddress: host, timeout: 120);
                if (pingcount >= 60) { pingcount = 0; pinglosscount = 0; pingcount++; }
                else { pingcount++; }
                if (reply.Status == IPStatus.Success)
                {
                    //Console.WriteLine("当前在线，已ping通！");
                    textBlock_networkdelay.Text = (reply.RoundtripTime/2).ToString() + " ms";
                    //StringBuilder sbuilder = new StringBuilder();
                    //sbuilder.AppendLine(string.Format("答复的主机地址: {0} ", reply.Address.ToString()));
                    //sbuilder.AppendLine(string.Format("往返时间: {0} ", reply.RoundtripTime));
                    //sbuilder.AppendLine(string.Format("生存时间（TTL）: {0} ", reply.Options.Ttl));
                    //sbuilder.AppendLine(string.Format("是否控制数据包的分段: {0} ", reply.Options.DontFragment));
                    //sbuilder.AppendLine(string.Format("缓冲区大小: {0} ", reply.Buffer.Length));
                    //Console.WriteLine(sbuilder.ToString());
                }
                else
                {
                    pinglosscount++;
                    if (pinglosscount > 0)
                    {
                        writelog("总PING数：" + pingcount + "丢包数：" + pinglosscount + "丢包率：" + (((double)pinglosscount / (double)pingcount)*100).ToString("f2"));
                    }
                    //Console.WriteLine("不在线，ping不通！");
                }
            }
            catch (Exception e)
            {
                AppLog.Error(e.Message);
            }
        }

        //写入日志
        public void writelog(string str)
        {
            try
            {
                AppLog.Info(str);
                writeblackbox(str);
            }
            catch (Exception e)
            {
                AppLog.Error(e.Message);
            }
        }        
        //写入黑匣子
        public void writeblackbox(string content)
        {
            try
            {
                string path = "./blackbox_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                FileStream fs = new FileStream(path, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(DateTime.Now.ToString("o") + "接收到数据:" + content + Environment.NewLine);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                AppLog.Error(e.Message);
            }
        }
        //判断是否是数字
        public static bool isNumber(string value)
        {
            try
            {
                return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
            }
            catch (Exception e)
            {
                AppLog.Error(e.Message);
                return false;
            }
        }

        //判断是否是十六进制
        public static bool is16Number(string value)
        {
            try
            {
                return Regex.IsMatch(value, @"[A-Fa-f0-9]+$");
            }
            catch (Exception e)
            {
                AppLog.Error(e.Message);
                return false;
            }
        }
    }
}
