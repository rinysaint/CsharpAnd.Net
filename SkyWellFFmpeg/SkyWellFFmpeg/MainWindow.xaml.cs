/************************************
*公司名称：开沃集团
*命名空间：SkyWellFFmpeg
*文件名：MainWindow.xaml.cs
*版本号：V1.0.0.0
*创建人:肖日宁
*电子邮件：a11010203@qq.com
*联系方式:QQ:164840753
*创建时间：2019年4月18日
*描述：主窗体文件
*************************************/
using System;
using System.Windows;
using System.Diagnostics;

namespace SkyWellFFmpeg
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string exe_path = "ffmpeg.exe";  // 被调exe
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            tbsavepath.Text = @"D:\video\" + DateTime.Now.ToString("yyyyMMdd") + ".mp4";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbisNoWindow.IsChecked) { MessageBox.Show("无参数不能后台运行！");return; }
            string datet = DateTime.Now.ToString("yyyyMMddHHmmss");
            string save_path = tbsavepath.Text == ""? @"D:\video\" + datet +".mp4": tbsavepath.Text;
            string url = tbURL.Text == "" ? @"" : tbURL.Text;
            string the_args = string.Format("-y -i {0} -t 86400 -an -c:v libx264 {1}", url, save_path);// 被调exe接受的参数
            StartProcess(exe_path, the_args);
        }
        private void btnStartParam_Click(object sender, RoutedEventArgs e)
        {            
            string datet = DateTime.Now.ToString("yyyyMMddHHmmss");
            string save_path = tbsavepath.Text == "" ? @"D:\video\" + datet +".mp4": tbsavepath.Text;
            string url = tbURL.Text == "" ? @"" : tbURL.Text;
            string the_args = tbparam.Text ==""? string.Format("-y -i {0} -t 20 -an -c:v libx264 -b 64k -r 5 {1}", url, save_path) : " " + tbparam.Text;// 被调exe接受的参数
            StartProcess(exe_path, the_args);
            MessageBox.Show("启动成功！");
        }
        
        private void btnStopAll_Click(object sender, RoutedEventArgs e)
        {
            string Command = @"taskkill /f /im ffmpeg.exe /t".Trim().TrimEnd('&')+ "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
            //string Command = "ping www.baidu.com" + "&exit";
            string CmdPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System) + @"\cmd.exe";
            try
            {
                Process p = new Process();
                {
                    p.StartInfo.FileName = CmdPath;
                    p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                    p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                    p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                    p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                    p.Start();//启动程序                                      

                    //向cmd窗口写入命令
                    p.StandardInput.WriteLine(Command);
                    p.StandardInput.AutoFlush = true;

                    //获取cmd窗口的输出信息
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();//等待程序执行完退出进程
                    p.Close();
                    //tbparam.Clear();
                    tbparam.AppendText(output);
                    tbparam.ScrollToEnd();
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }
        public bool StartProcess(string runFilePath, string args)
        {
            args = args.Trim();
            try
            {
                Process process = new Process();//创建进程对象    
                ProcessStartInfo startInfo = new ProcessStartInfo(runFilePath, args); // 括号里是(程序名,参数)
                process.StartInfo = startInfo;
                if(cbisNoWindow.IsChecked.ToString() != "")
                { 
                process.StartInfo.UseShellExecute = cbisNoWindow.IsChecked == true ? false : true;    //是否使用操作系统的shell启动
                //startInfo.RedirectStandardInput = true;      //接受来自调用程序的输入     
                //startInfo.RedirectStandardOutput = true;     //由调用程序获取输出信息
                startInfo.CreateNoWindow = (bool)cbisNoWindow.IsChecked;             //不显示调用程序的窗口                 
                }
                process.Start();
                return true;
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message);return false; }
        }        
    }
}
