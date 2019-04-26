using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using Vlc.DotNet.Wpf;

namespace SkyWellRearviewMirror
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            init("");
        }
        public void init(string Messages)
        {
            try
            {
                //设置标题和窗口全屏
                this.Title = ConfigurationManager.AppSettings["WindowsTitle"];
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;

                //开启视频连接
                mediaplay();

                //启动线程处理断线重连
                Thread threadupdate = new Thread(timeoutconn);
                threadupdate.IsBackground = true;//设置为后台线程，当主线程结束后，后台线程自动退出，否则不会退出程序不能结束
                threadupdate.Start();
            }
            catch (Exception e) { throw e; }
        }
        //开启视频连接
        private void mediaplay()
        {
            try
            {
                RearviewMirror.SourceProvider.CreatePlayer(new DirectoryInfo(Path.Combine("libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64")));
                string strVideoUrl = ConfigurationManager.AppSettings["VideoUrl"];
                string[] arguments = { ":network-caching=90 " };
                RearviewMirror.SourceProvider.MediaPlayer.Play(new Uri(strVideoUrl), arguments);
            }
            catch (Exception e)
            { }
        }
        //线程方法，处理断线重连
        private void timeoutconn()
        {
            try
            {
                while (true)
                {
                    Action action0 = () =>
                    {
                        if (RearviewMirror.SourceProvider.MediaPlayer.State != Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                        {
                            mediaplay();
                        }
                    };
                    RearviewMirror.Dispatcher.BeginInvoke(action0);
                    // 如果不设置等待，整个程序死循环
                    Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["timeout"]));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
