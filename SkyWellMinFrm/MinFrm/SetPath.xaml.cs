/************************************
*公司名称：开沃集团
*命名空间：MinFrm
*文件名：SetPath.xaml.cs
*版本号：V1.0.0.0
*创建人:肖日宁
*电子邮件：a11010203@qq.com
*联系方式:QQ:164840753
*创建时间：2019年3月8日
*描述：设置路径窗体文件
*************************************/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;

namespace MinFrm
{
    /// <summary>
    /// SetPath.xaml 的交互逻辑
    /// </summary>
    public partial class SetPath : Window
    {
        Thread thread_previewpath;
        bool previewstatus =true;
        public SetPath()
        {
            InitializeComponent();
            MapLeft(getXY());
            thread_previewpath = new Thread(previewpath);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
        }
        //退出提示程序
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确认更改并保存路径？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                //this.Close();
                //Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }
        //左边窗口显示导航地图
        private void MapLeft(double[,] xy)
        {

            canvas_Setpath.Children.Clear();
            Polygon pg = new Polygon() { Stroke = new SolidColorBrush(Colors.Blue) };
            int busstopnum = 0;
            for (int i = 0; i < xy.GetLength(0); i++)
            {
                pg.Points.Add(
                    new System.Windows.Point(
                        xy[i, 0],
                        xy[i, 1]
                     ));

                if (xy[i, 2] == 1)
                {
                    busstopnum++;
                    Label newlabel = new Label();
                    newlabel.Content = "站点" + busstopnum.ToString();
                    newlabel.Width = 60; newlabel.Height = 25;
                    newlabel.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 0, 0));
                    Canvas.SetLeft(newlabel, xy[i, 0]);
                    Canvas.SetTop(newlabel, xy[i, 1]);
                    canvas_Setpath.Children.Add(newlabel);
                }
            }
            //直线对象
            canvas_Setpath.Children.Add(pg);
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
            double[,] prec = new double[p0.GetLength(0), 3];
            for (int i = 0; i < p0.GetLength(0); i++)
            {
                prec[i, 0] = Math.Abs(p0[i, 0] - p0[0, 0]) * 70000;
                prec[i, 1] = Math.Abs(p0[i, 1] - p0[0, 1]) * 70000;
                if (i == 0 || i == 5 || i == 12) { prec[i, 2] = 1; }
            }
            return prec;
        }
        //预览
        private void button_preview_Click(object sender, RoutedEventArgs e)
        {
            //启动线程处理
            if (button_preview.Content.ToString() == "预览")
            {
                button_preview.Content = "停止预览";
                previewstatus = true;
                if (!thread_previewpath.IsAlive)
                {
                    thread_previewpath = new Thread(previewpath);
                    thread_previewpath.IsBackground = true;//设置为后台线程，当主线程结束后，后台线程自动退出，否则不会退出程序不能结束
                    thread_previewpath.Start();
                }
            }
            else {
                button_preview.Content = "预览";
                //thread_previewpath.Abort();
                previewstatus = false;
                //thread_previewpath.Join();
            }
        }
        //显示路径
        private void showpath(int num)
        {
            double[,] xy = getXY();
            if (num < xy.GetLength(0))
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(@"img/carpath.gif", UriKind.Relative));
                if (num != 0)
                {
                    Canvas.SetLeft(img, xy[num - 1, 0]);
                    Canvas.SetTop(img, xy[num - 1, 1]);
                    canvas_Setpath.Children.Remove(img);
                }
                Canvas.SetLeft(img, xy[num, 0]);
                Canvas.SetTop(img, xy[num, 1]);
                canvas_Setpath.Children.Add(img);
            }
        }
        //线程方法，修改canvas_Setpath内容
        private void previewpath()
        {
            int i = 0;
            while (previewstatus)
            {
                Action action1 = () =>
                {
                    showpath(i);
                };
                canvas_Setpath.Dispatcher.BeginInvoke(action1);
                // 如果不设置等待，整个程序死循环
                Thread.Sleep(1000);
                i++;
                if (i > getXY().GetLength(0)) { i = 0; }
            }
        }
    }
}
