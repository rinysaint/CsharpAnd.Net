/************************************
*公司名称：开沃集团
*命名空间：MinFrm
*文件名：MainWindow.xaml.cs
*版本号：V1.0.0.0
*创建人:肖日宁
*电子邮件：a11010203@qq.com
*联系方式:QQ:164840753
*创建时间：2019年3月8日
*描述：小屏主窗体文件
*************************************/
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MinFrm.Dal;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace MinFrm
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ChineseDate chndate = new ChineseDate();
        ImagesDal imgdal = new ImagesDal();
        public MainWindow()
        {
            InitializeComponent();
            MapLeft(getXY());
            Main_Righttxt();        
        }
        //左边窗口显示导航地图
        private void MapLeft(double[,] xy)
        {
            canvas_left.Children.Clear();
            Polygon pg = new Polygon() { Stroke = new SolidColorBrush(Color.FromRgb(42, 202, 200)) };
            pg.StrokeThickness = 5;
            int busstopnum = 0;
            for (int i = 0; i < xy.GetLength(0); i++)
            {
                pg.Points.Add(
                    new System.Windows.Point(
                        xy[i, 0]+10,
                        xy[i, 1]+10
                     ));

                if (xy[i, 2] == 1) {
                    busstopnum++;
                    Label newlabel = new Label();
                    newlabel.Content = "站点" + busstopnum.ToString();
                    newlabel.Width = 60;newlabel.Height = 25;
                    newlabel.Foreground = new SolidColorBrush(Colors.White);
                    if (i != 0)
                    {
                        Canvas.SetLeft(newlabel, xy[i, 0]+10);
                        Canvas.SetTop(newlabel, xy[i, 1]);
                    }
                    else
                    {
                        Canvas.SetLeft(newlabel, 10);
                        Canvas.SetTop(newlabel, -5);
                    }
                    canvas_left.Children.Add(newlabel);
                }
            }
            //直线对象
            canvas_left.Children.Add(pg);            
        }
        private void showpath(int num) {
            double[,] xy = getXY(); 
            if (num < xy.GetLength(0))
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"img/carpath.gif", UriKind.Relative));
                MapLeft(xy);
                Canvas.SetLeft(img, xy[num, 0]);
                Canvas.SetTop(img, xy[num, 1]);
                canvas_left.Children.Add(img);
            }
        }
        //取得GPS经纬度坐标系数组
        private double[,] getXY()
        {
            double[,] p0 = new double[,]{
                {   118.9268913 ,   31.7310468  },//站点p0(0)
                {   118.9296340 ,   31.7298293  },
                {   118.9305549 ,   31.7294090  },
                {   118.9323794 ,   31.7285864  },
                {   118.9342072 ,   31.7277685  },
                {   118.9343225 ,   31.7276871  },//站点p0(5)
                {   118.9342963 ,   31.7276154  },
                {   118.9338230,    31.7268890  },
                {   118.9332777 ,   31.7260141  },
                {   118.9329893 ,   31.7255693  },
                {   118.9321352 ,   31.7246492  },
                {   118.9318589 ,   31.7243152  },

                {   118.9318912 ,   31.7242407  },//站点p0(12)
                {   118.9319599 ,   31.7242920  },
                {   118.9320488 ,   31.7247455  },
                {   118.9287764 ,   31.7262443  },
                {   118.9286959 ,   31.7263192  },
                {   118.9287196 ,   31.7264127  },
                {   118.9291883 ,   31.7271667  },
                {   118.9293785 ,   31.7273890  },
                {   118.9295072 ,   31.7275824  },
                {   118.9295264 ,   31.7276821  },
                {   118.9294482 ,   31.7277549  },
                {   118.9286243 ,   31.7281081  },
                {   118.9259997 ,   31.7292757  },
                {   118.9259024,    31.7293598  },
                {   118.9259127 ,   31.7294378  },
                {   118.9268268 ,   31.7309829  },
                };
            double[,] prec = new double[p0.GetLength(0),3];
            for (int i = 0; i < p0.GetLength(0); i++)
            {
                prec[i,0] = Math.Abs(p0[i, 0] - p0[0,0])*70000;
                prec[i,1] = Math.Abs(p0[i, 1] - p0[0,1])*70000;
                if (i == 0 || i == 5 || i == 12) { prec[i, 2] =1; }
            }
            return prec;
        }

        //右边窗口显示信息
        public void Main_Righttxt ()
        {
            //textBlock1.Text = new Random().Next(30, 35) +"km/h";//车速     
            Thread_textblockupdate();
            textBlock1_2.Text = "自动模式";//工作模式
            textBlock1_3.Text = "D";//档位
            //EPB状态
            image4.Source = new BitmapImage(new Uri(@"img/QQ20190322134705.png", UriKind.Relative));
            //车门状态
            image5.Source = new BitmapImage(new Uri(@"img/doorstatus.gif", UriKind.Relative));

            //textBlock6.Text = "正常";//胎压:前左
            //textBlock7.Text = "正常";//胎压:前右
            //textBlock8.Text = "正常";//胎压:后左
            //textBlock9.Text = "正常";//胎压:后右

            //电池状态
            int batterynum = 15;
            textBlock10.Text = (batterynum).ToString() + "%";//SOC;             
            
            // 使用切割后的图源
            if (batterynum >= 100) { image10.Source = imgdal.cutImage(@"img/batteryinfo.jpg", BatteryStatus.getIntRect(BatteryStatusEnum.full)); }
            else if (batterynum < 100 && batterynum >= 75) { image10.Source = imgdal.cutImage(@"img/batteryinfo.jpg", BatteryStatus.getIntRect(BatteryStatusEnum.quarter)); }
            else if (batterynum < 75 && batterynum >= 50)
            { image10.Source = imgdal.cutImage(@"img/batteryinfo.jpg", BatteryStatus.getIntRect(BatteryStatusEnum.half)); }
            else if (batterynum < 50 && batterynum >= 25)
            { image10.Source = imgdal.cutImage(@"img/batteryinfo.jpg", BatteryStatus.getIntRect(BatteryStatusEnum.low)); }
            else if (batterynum < 25 && batterynum >= 0)
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"img/batterywarning.gif", UriKind.Relative);
                image.EndInit();
                ImageBehavior.SetAnimatedSource(image10, image);
            }

            image11.Source = new BitmapImage(new Uri(@"img/QQ20190322134650.png", UriKind.Relative));//充电状态

            textBlock12.Text = "666km";//累计行驶里程
                textBlock13.Text = "233km";//所剩续航里程
            textBlock14_2.Text = DateTime.Now.ToString("d") + Environment.NewLine + chndate.GetChineseDate(DateTime.Now);//日期
            //天气数据
            int weatherinfo = 1;
            textBlock15.Text = WeatherInfo.description((WeatherInfoEnum)weatherinfo).ToString() + " 2-10℃";//天气
            image15.Source = new BitmapImage(new Uri(@"img/QQ20190322134547.png", UriKind.Relative));
            //image15.Source = imgdal.cutImage(@"img/QQ图片20190322134547.jpg", WeatherInfo.getWeatherRect((WeatherInfoEnum)weatherinfo));

            textBlock16.Text = "15℃";//车内温度
                textBlock17.Text = "5℃";//车外温度
                textBlock22.Text = "25℃";//空调设定温度;
                //转向灯
                image18.Source = new BitmapImage(new Uri(@"img/QQ20190322134623.png", UriKind.Relative));
                textBlock0.Text = "1级";//智驾系统可靠性等级
                //急停状态
            //image23.Source = new BitmapImage(new Uri(@"img/qstop.gif", UriKind.Relative));
        }

        private void Thread_textblockupdate()
        {
            //启动线程处理
            Thread threadupdate = new Thread(UpdateTextblock);
            threadupdate.IsBackground = true;//设置为后台线程，当主线程结束后，后台线程自动退出，否则不会退出程序不能结束
            threadupdate.Start();
        }
        //线程方法，修改textBlock123内容
        private void UpdateTextblock()
        {
            int i = 0;
            while (true)
            {
                Action action1 = () =>
                {
                    textBlock1.Text = new Random().Next(15, 20).ToString() + " km/h";
                    textBlock2.Text = new Random().Next(270, 300).ToString() + " r/min";
                    textBlock3.Text = new Random().Next(60, 80).ToString() + " N\"m";
                    textBlock14_1.Text = DateTime.Now.ToString("t");//时间

                    showpath(i);
                };
               textBlock1.Dispatcher.BeginInvoke(action1);
                // 如果不设置等待，整个程序死循环
                Thread.Sleep(1000);
                i++;
                if (i > getXY().GetLength(0)) { i = 0; }
            }
        }
        //工作模式－自动模式－载入路径弹窗
        private void Loadpath_Click(object sender, RoutedEventArgs e)
        {
            new Form_ModeAuto().Show();
        }
        //工作模式－自动模式－录制路径弹窗
        private void Recordingpath_Click(object sender, RoutedEventArgs e)
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory;
            //path = path.Substring(0, path.LastIndexOf("bin"));
            RecordingPath rpwindow = new RecordingPath();            
            rpwindow.Show();
        }
        //工作模式－自动模式－设置路径弹窗
        private void Setpath_Click(object sender, RoutedEventArgs e)
        {
            SetPath spwindow = new SetPath();
            spwindow.Show();
        }
        //工作模式－遥控模式－平板操作弹窗
        private void Padcontrol_Click(object sender, RoutedEventArgs e)
        {
            PadControl pcwindow = new PadControl();
            pcwindow.Show();
        }
        //工作模式－遥控模式－遥控操作弹窗
        private void Remotecontrol_Click(object sender, RoutedEventArgs e)
        {

        }
        private void button_WorkMode_Click(object sender, RoutedEventArgs e)
        {
            new Form_ModeAuto().Show();
        }
        //空调设置弹窗
        private void button_ACSet_Click(object sender, RoutedEventArgs e)
        {
            new Form_ACSet().Show();
        }
        //故障信息弹窗
        private void button_FaultInfo_Click(object sender, RoutedEventArgs e)
        {
            new Form_FaultInfo().Show();
        }
        //电池信息弹窗
        private void button_BatteryInfo_Click(object sender, RoutedEventArgs e)
        {
            new Form_BatteryInfo().Show();
        }
        //多媒体操作弹窗
        private void button_MediaSet_Click(object sender, RoutedEventArgs e)
        {
            new Form_MediaSet().Show();
        }
        //灯光控制弹窗
        private void button_LightSet_Click(object sender, RoutedEventArgs e)
        {
            new Form_LightSet().Show();
        }
        //电机信息弹窗
        private void button_MotorInfo_Click(object sender, RoutedEventArgs e)
        {
            new Form_MotorInfo().Show();
        }
    }
}
