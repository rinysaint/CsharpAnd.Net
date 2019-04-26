using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;  // IP，IPAddress, IPEndPoint，端口等；
using System.Threading;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace tcpserver
{
    public partial class frm_server : Form
    {
        public frm_server()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        Thread threadWatch = null; // 负责监听客户端连接请求的 线程；
        Socket socketWatch = null;


        Dictionary<string, Socket> dict = new Dictionary<string, Socket>();//通信套接字 集合；
        Dictionary<string, string> dictroute = new Dictionary<string, string>();//通信路由表 集合；
        Dictionary<string, Thread> dictThread = new Dictionary<string, Thread>();//通信线程集合；

        private void btnBeginListen_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建包含ip和端口号的网络节点对象；
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(txtIp.Text.Trim()), int.Parse(txtPort.Text.Trim()));
                // 判断端口是否被占用
                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();
                foreach (IPEndPoint ipendPoint in ipEndPoints)
                {
                    if (ipendPoint.Port == endPoint.Port)
                    {
                        MessageBox.Show("异常：端口已被占用！");
                        return;
                    }
                }
                // 创建负责监听的套接字，注意其中的参数；
                socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 将负责监听的套接字绑定到唯一的ip和端口上；
                socketWatch.Bind(endPoint);
            }
            catch (SocketException se)
            {
                MessageBox.Show("异常：" + se.Message);
                return;
            }
            // 设置监听队列的长度；
            socketWatch.Listen(10);
            // 创建负责监听的线程；
            threadWatch = new Thread(WatchConnecting);
            threadWatch.IsBackground = true;
            threadWatch.Start();
            System.Timers.Timer MsgTimer = new System.Timers.Timer(60000);//实例化MsgTimer类，设置间隔时间为10000毫秒；
            MsgTimer.Elapsed += new System.Timers.ElapsedEventHandler(Msgtimer_Tick); //到达时间的时候执行事件；   
            MsgTimer.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；   
            MsgTimer.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件；
            ShowMsg("服务器启动监听成功！");
        }

        /// <summary>
        /// 监听客户端请求的方法；
        /// </summary>
        void WatchConnecting()
        {
            while (true)  // 持续不断的监听客户端的连接请求；
            {
                try
                { 
                    // 开始监听客户端连接请求，Accept方法会阻断当前的线程；
                    Socket sokConnection = socketWatch.Accept(); // 一旦监听到一个客户端的请求，就返回一个与该客户端通信的 套接字；
                    // 想列表控件中添加客户端的IP信息；
                    lbOnline.Items.Add(sokConnection.RemoteEndPoint.ToString());
                    // 将与客户端连接的 套接字 对象添加到集合中；
                    dict.Add(sokConnection.RemoteEndPoint.ToString(), sokConnection);
                    dictroute.Add(sokConnection.RemoteEndPoint.ToString(), "");
                    ShowMsg(sokConnection.RemoteEndPoint.ToString()+"客户端连接成功！");
                    Thread thr = new Thread(RecMsg);
                    thr.IsBackground = true;
                    thr.Start(sokConnection);
                    dictThread.Add(sokConnection.RemoteEndPoint.ToString(), thr);  //  将新建的线程 添加 到线程的集合中去。
                }
                catch (SocketException e)
                {
                    MessageBox.Show("异常：" + e.Message);
                    break;
                }
            }
                
        }
        // 接收消息
        void RecMsg(object sokConnectionparn)
        {
            Socket sokClient = sokConnectionparn as Socket;
            while (true)
            {
                // 定义一个1k的缓存区；
                byte[] arrMsgRec = new byte[1024];
                // 将接受到的数据存入到输入  arrMsgRec中；
                int length = -1;
                try
                {
                    length = sokClient.Receive(arrMsgRec); // 接收数据，并返回数据的长度；
                }
                catch (SocketException se)
                {
                    ShowMsg(sokClient.RemoteEndPoint.ToString() + "异常：" + se.Message);
                    // 从 通信套接字 集合中删除被中断连接的通信套接字；
                    dict.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从 通信路由表 集合中删除被中断连接的通信；
                    dictroute.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从通信线程集合中删除被中断连接的通信线程对象；
                    dictThread.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从列表中移除被中断的连接IP
                    lbOnline.Items.Remove(sokClient.RemoteEndPoint.ToString());
                    break;
                }
                catch (Exception e)
                {
                    ShowMsg(sokClient.RemoteEndPoint.ToString() + "异常：" + e.Message);
                    // 从 通信套接字 集合中删除被中断连接的通信套接字；
                    dict.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从 通信路由表 集合中删除被中断连接的通信；
                    dictroute.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从通信线程集合中删除被中断连接的通信线程对象；
                    dictThread.Remove(sokClient.RemoteEndPoint.ToString());
                    // 从列表中移除被中断的连接IP
                    lbOnline.Items.Remove(sokClient.RemoteEndPoint.ToString());
                    break;
                }
                if (length > 0)
                {
                    if (arrMsgRec[0] != 0)  // 表示接收到的是数据；
                    {

                        //string strMsg = System.Text.Encoding.Default.GetString(arrMsgRec);
                        // byte[] arrMsg = new byte[arrMsgRec[11] + 14];
                        byte[] arrMsg = new byte[length];
                        Array.Copy(arrMsgRec, arrMsg, length);
                        //string[] strMsg = Regex.Split(byteToHexStr(arrMsg), "(?<=\\G.{2})");
                        if (arrMsg[0] != 0x23)
                        {
                            ShowMsg(sokClient.RemoteEndPoint.ToString() + ":" + Encoding.UTF8.GetString(arrMsg, 0, arrMsg.Length));
                            ShowMsg(sokClient.RemoteEndPoint.ToString() + ":" + byteToHexStr(arrMsg));
                        }
                        else { ShowMsg(sokClient.RemoteEndPoint.ToString() + ":" + byteToHexStr(arrMsg)); }
                        
                        //连接数据不做发送处理。
                        if (arrMsg.Length > 10)
                        {
                            Send_MsgbyIP(arrMsg, sokClient.RemoteEndPoint.ToString());
                        }
                    }
                }
                else
                {
                    //Send_Msg("", sokClient.RemoteEndPoint.ToString());
                }
            }            
        }

        /// <summary>
        /// 按照信息类型发送消息
        /// </summary>
        /// <param name="strMsg">消息</param>
        /// <param name="ip">发送方IP</param>
        private void Send_Msg(string strMsg, string ip)
        {
            try
            {
                //if (!string.IsNullOrEmpty(strMsg) && !string.IsNullOrEmpty(ip))
                //{
                //    byte[] arrMsg = (ASCIIEncoding.Default.GetBytes(strMsg)); // 将要发送的字符串转换成ASCII字节数组；
                //    string strKey = "";
                //    strKey = lbOnline.Text.Trim();
                //    Dictionary<string, Socket> dictdel = new Dictionary<string, Socket>(dict);
                //    dictdel.Remove(ip);
                //    if (dictdel.Count > 0)
                //    {
                //        foreach (Socket s in dictdel.Values)
                //        {
                //            s.Send(arrMsg);
                //        }
                //    }
                //}
                //else 
                if (!string.IsNullOrEmpty(strMsg) && string.IsNullOrEmpty(ip))
                {
                    byte[] arrMsg = (ASCIIEncoding.Default.GetBytes(strMsg)); // 将要发送的字符串转换成ASCII字节数组；
                    Dictionary<string, Socket> dicts = new Dictionary<string, Socket>(dict);
                    if (dicts.Count > 0)
                    {
                        foreach (Socket s in dicts.Values)
                        {
                            s.Send(arrMsg);
                        }
                    }
                }
                //if (string.IsNullOrEmpty(strMsg) && !string.IsNullOrEmpty(ip))
                //{
                //    Dictionary<string, string> rt = new Dictionary<string, string>(dictroute);
                //    foreach (KeyValuePair<string, string> no in rt)
                //    {
                //        if (no.Value.Equals("00")|| no.Value.Equals(""))
                //        {
                //            Dictionary<string, Socket> dt = new Dictionary<string, Socket>(dict);

                //            if (no.Key == ip)
                //            {
                //                // 从列表中移除被中断的连接IP
                //                lbOnline.Items.Remove(ip);
                //                dict[ip].Close();
                //                ShowMsg(ip + "关闭连接：");
                //                // 从 通信套接字 集合中删除被中断连接的通信套接字；
                //                dict.Remove(ip);
                //                // 从 通信路由表 集合中删除被中断连接的通信；
                //                dictroute.Remove(ip);
                //                // 从通信线程集合中删除被中断连接的通信线程对象；
                //                dictThread.Remove(ip);
                //                dictThread[ip].Abort();
                //            }
                            
                //        }
                //    }
                //}
                //else if (!string.IsNullOrEmpty(txtMsgSend.Text.Trim()))
                //{
                //    strMsg = txtMsgSend.Text.Trim() + Environment.NewLine;
                //    byte[] arrMsg = (ASCIIEncoding.Default.GetBytes(strMsg)); // 将要发送的字符串转换成ASCII字节数组；
                //    foreach (Socket s in dict.Values)
                //    {
                //        s.Send(arrMsg);
                //    }
                //    ShowMsg(strMsg);
                //    txtMsgSend.Clear();
                //    ShowMsg(" 群发完毕～～～");
                //}
            }
            catch (Exception ex)
            { ShowMsg("异常：" + ex.Message); return; }
        }

        /// <summary>
        /// 按照信息类型发送消息
        /// </summary>
        /// <param name="strMsg">消息</param>
        /// <param name="ip">发送方IP</param>
        private void Send_MsgbyIP(byte[] byteMsg, string ip)
        {
            // 将要发送的字符串转换成UINT16位数；
            int len = byteMsg[11]; UInt16 sum = 0; UInt16 readsum = 1;
            if (byteMsg.Length >= len + 13)
            { 
                readsum = (UInt16)(((UInt16)(byteMsg[len + 12]) << 8) + (UInt16)byteMsg[len + 13]);            
                for (int i = 0; i < len + 12; i++)
                {
                    sum += byteMsg[i];
                }
            }
            if (sum == readsum)//和校验
            {
                if (!string.IsNullOrEmpty(byteMsg[3].ToString()) && !string.IsNullOrEmpty(byteMsg[4].ToString()))
                {
                    //发送端为站点处理方法
                    if (byteMsg[4] == 0x00)
                    {

                        string vno = byteMsg[5].ToString() + byteMsg[6].ToString() + byteMsg[7].ToString() + byteMsg[8].ToString() + byteMsg[9].ToString() + byteMsg[10].ToString();
                        try
                        {
                            //登陆应答
                            if (byteMsg[2] == 0x00)
                            {                        //更新站点路由表信息
                                updatedictroute(ip, vno);
                                byteMsg[3] = Convert.ToByte("01"); Socket lgin = dict[ip];
                                lgin.Send(CheckSum(byteMsg));
                                ShowMsg(ip + "站点登陆:" + byteToHexStr(CheckSum(byteMsg)));
                                return;
                            }
                        }
                        catch (Exception e)
                        { ShowMsg("异常：" + e.Message); return; }
                        int flag = 0;
                        Dictionary<string, string> rt = new Dictionary<string, string>(dictroute);
                        foreach (KeyValuePair<string, string> no in rt)
                        {
                            try
                            {
                                //查找车辆地址，按IP发送数据，无车辆信息返回03连接丢失
                                if (no.Value.Equals("01"))
                                {
                                    Socket s = dict[no.Key];
                                    s.Send(byteMsg);
                                    flag = 1;
                                    ShowMsg(ip+">>>"+ no.Key+ ":" + byteToHexStr(byteMsg));
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            catch (Exception e)
                            { ShowMsg("异常：" + e.Message); continue; }
                        }
                        if (flag == 0)
                        {
                            try
                            {
                                byteMsg[3] = Convert.ToByte("03");
                                Socket fls = dict[ip];
                                fls.Send(CheckSum(byteMsg));
                                ShowMsg(ip + "信号丢失03：" + byteToHexStr(CheckSum(byteMsg)));
                            }
                            catch (Exception e)
                            { ShowMsg("站点信号丢失：" + e.Message); return; }
                        }
                    }
                    //发送端为车辆处理方法
                    if (byteMsg[4] == 0x01)
                    {
                        string vno = byteMsg[5].ToString() + byteMsg[6].ToString() + byteMsg[7].ToString() + byteMsg[8].ToString() + byteMsg[9].ToString() + byteMsg[10].ToString();
                        int flag = 0;
                        try
                        {
                            //登陆应答
                            if (byteMsg[2] == 0x00)
                            {
                                //更新车辆路由表信息                                       
                                updatedictroute(ip, "01");
                                byteMsg[3] = Convert.ToByte("01"); Socket lgin = dict[ip];
                                lgin.Send(CheckSum(byteMsg));
                                ShowMsg(ip + "车辆登陆:" + byteToHexStr(CheckSum(byteMsg)));
                                return;
                            }
                        }
                        catch (Exception e)
                        { ShowMsg("异常：" + e.Message); return; }
                        Dictionary<string, string> rt = new Dictionary<string, string>(dictroute);
                        foreach (KeyValuePair<string, string> no in rt)
                        {
                            try
                            {
                                //查找站点地址，按IP发送数据，无站点信息返回03连接丢失
                                if (no.Value.Equals(vno))
                                {
                                    Socket s = dict[no.Key];
                                    s.Send(byteMsg);
                                    flag = 1;
                                    ShowMsg(ip + ">>>" + no.Key +":"+ byteToHexStr(byteMsg));
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            catch (Exception e)
                            { ShowMsg("异常：" + e.Message); continue; }

                        }
                        if (flag == 0)
                        {
                            try
                            {
                                byteMsg[3] = Convert.ToByte("03");
                                Socket fls = dict[ip];
                                fls.Send(CheckSum(byteMsg));
                                ShowMsg(ip + "信号丢失03：" + byteToHexStr(CheckSum(byteMsg)));
                            }
                            catch (Exception e)
                            { ShowMsg("车辆信号丢失：" + e.Message); return; }
                        }
                    }
                }
            }      
        }

        // 更新路由信息
        private void updatedictroute(string ip,string val)
        {
            Dictionary<string, string> dictno = new Dictionary<string, string> (dictroute);
            foreach (KeyValuePair<string, string> kvp in dictno)
            {
                if (kvp.Value.Equals(val))
                {
                    if (kvp.Key != ip) { dictroute[kvp.Key] = "";  }
                }
            }            
            if (dictroute.ContainsKey(ip)) { dictroute[ip] = val; }
        }
        //执行连接状态验证
        private void Msgtimer_Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            Send_Msg("test", "");
        }
        // 发送消息
        private void btnSend_Click(object sender, EventArgs e)
        {
            string strMsg = txtMsgSend.Text.Trim() + Environment.NewLine;
            byte[] arrMsg = ASCIIEncoding.Default.GetBytes(strMsg); // 将要发送的字符串转换成ASCII字节数组；
            byte[] arrSendMsg=new byte[arrMsg.Length+1];
            arrSendMsg[0] = 0; // 表示发送的是消息数据
            Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
            string strKey = "";
            strKey = lbOnline.Text.Trim();
            try
            {
                if (string.IsNullOrEmpty(strKey))   // 判断是不是选择了发送的对象；
                {
                    MessageBox.Show("请选择你要发送的好友！！！");
                }            
                else
                {
                    dict[strKey].Send(arrSendMsg);// 解决了 sokConnection是局部变量，不能再本函数中引用的问题；
                    ShowMsg(strMsg);
                    txtMsgSend.Clear();
                }
            }
            catch (Exception ex)
            { ShowMsg("异常：" + ex.Message); }
        }

        /// <summary>
        /// 群发消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">消息</param>
        private void btnSendToAll_Click(object sender, EventArgs e)
        {
            //Send_Msg("", "");
        }
        /// <summary>
        /// 界面显示消息
        /// </summary>
        void ShowMsg(string str)
        {
            txtMsg.AppendText(str + Environment.NewLine);
            WriteLog(str);
        }
        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string byteToHexStr(byte[] bytes)
        {
            try
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
            catch (Exception e)
            {
                ShowMsg("异常：" + e.Message);
                return "";
            }
        }
        //16进制格式string 转byte[]：
        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private byte[] strToToHexByte(string hexString)
        {
            try
            {
                hexString = hexString.Replace(" ", "");
                if ((hexString.Length % 2) != 0)
                    hexString += " ";
                byte[] returnBytes = new byte[hexString.Length / 2];
                for (int i = 0; i < returnBytes.Length; i++)
                    returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                return returnBytes;
            }
            catch (Exception e)
            {
                ShowMsg("异常：" + e.Message);
                byte[] a =new byte[1]; a[0] = Convert.ToByte("0", 16);
                return a;
            }
        }
        //校验
        private byte[] CheckSum(byte[] str)
        {
            try
            {
                UInt16 cks = 0;
                byte[] strsum =  str;
                int len = strsum[11];
                for (int i = 0; i < len + 12; i++)
                {
                    cks += strsum[i];
                }                
                strsum[len+12] =Convert.ToByte(cks >> 8);
                strsum[len+13] = Convert.ToByte(cks & 0x00FF);
                //byte[] returnMsg = new byte[len + 14];
                //Array.Copy(strsum, returnMsg, len + 14);
                return strsum;
            }
            catch (Exception e)
            {
                ShowMsg("异常：" + e.Message);
                byte[] a = new byte[1]; a[0] = Convert.ToByte("0", 16);
                return a;
            }
        }

        /// <summary>
        /// 写入日志文件
        /// </summary>
        /// <param name="input"></param>
        public static void WriteLog(string input)
        {
            ///指定日志文件的目录
            string LogAddress = System.Windows.Forms.Application.StartupPath + "\\" + DateTime.Now.Year + '-' + DateTime.Now.Month + '-' + DateTime.Now.Day + "_LogFile" + ".txt";
            if (!File.Exists(LogAddress))
            {
                //不存在文件
                File.Create(LogAddress).Dispose();//创建该文件
            }
            /**/
            ///判断文件是否存在以及是否大于2K
            /* if (finfo.Length > 1024 * 1024 * 10)
            {
            /**/
            //文件超过10MB则重命名
            /* File.Move(fname, Directory.GetCurrentDirectory() + DateTime.Now.TimeOfDay + "\\LogFile.txt");
            //删除该文件
            //finfo.Delete();
            }*/
            try
            {
                using (StreamWriter log = new StreamWriter(LogAddress, true))
                {
                    //FileStream fs = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);FileMode.Append

                    ///设置写数据流的起始位置为文件流的末尾
                    log.BaseStream.Seek(0, SeekOrigin.End);
                    ///写入“Log Entry : ”
                    log.Write("\n\rLog Entry : ");
                    ///写入当前系统时间并换行
                    log.Write("{0} {1} \n\r", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                    ///写入日志内容并换行
                    log.Write(input + "\n\r");
                    //清空缓冲区
                    log.Flush();
                    //关闭流
                    log.Close();
                }
            }
            catch (Exception e)
            {
                return;
            }
        }



        /// <summary>
        /// 将异常打印到LOG文件
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="LogAddress">日志文件地址</param>
        public static void WriteLogTxt(Exception ex, string LogAddress = "")
        {
            //如果日志文件为空，则默认在Debug目录下新建 YYYY-mm-dd_Log.log文件
            if (LogAddress == "")
            {
                ///指定日志文件的目录
                LogAddress = System.Windows.Forms.Application.StartupPath +"\\"+ DateTime.Now.Year + '-' + DateTime.Now.Month + '-' + DateTime.Now.Day + "_LogFile" + ".txt";
            }

            FileInfo finfo = new FileInfo(LogAddress);

            if (!finfo.Exists)
            {
                FileStream fs;
                fs = File.Create(LogAddress);
                fs.Close();
                finfo = new FileInfo(LogAddress);
            }
            try
            {
                //把异常信息输出到文件
                StreamWriter sw = new StreamWriter(LogAddress, true);
                sw.WriteLine("当前时间：" + DateTime.Now.ToString());
                sw.WriteLine("异常信息：" + ex.Message);
                sw.WriteLine("异常对象：" + ex.Source);
                sw.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim());
                sw.WriteLine("触发方法：" + ex.TargetSite);
                sw.WriteLine();
                sw.Close();
            }
            catch (Exception e)
            {
                return;
            }
        }
    }
}
