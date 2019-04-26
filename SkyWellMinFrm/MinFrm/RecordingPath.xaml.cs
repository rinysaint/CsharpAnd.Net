/************************************
*公司名称：开沃集团
*命名空间：MinFrm
*文件名：RecordingPath.xaml.cs
*版本号：V1.0.0.0
*创建人:肖日宁
*电子邮件：a11010203@qq.com
*联系方式:QQ:164840753
*创建时间：2019年3月8日
*描述：录制路径窗体文件
*************************************/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MinFrm
{
    /// <summary>
    /// RecordingPath.xaml 的交互逻辑
    /// </summary>
    public partial class RecordingPath : Window
    {
        public RecordingPath()
        {
            InitializeComponent();
            MapLeft(getXY());
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("确认退出遥控模式？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                //保存数据到TXT
                //SaveFileDialog objSave = new SaveFileDialog();
                //objSave.Filter = "(*.txt)|*.txt|" + "(*.*)|*.*";

                //objSave.FileName = "路径文件" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

                //if (objSave.ShowDialog() == DialogResult.OK)
                //{
                //    StreamWriter FileWriter = new StreamWriter(objSave.FileName, true); //写文件

                //    FileWriter.Write(textBlock.Text);//将字符串写入
                //    FileWriter.Close(); //关闭StreamWriter对象
                //}
                this.Close();
            }
        }

        //左边窗口显示导航地图
        private void MapLeft(double[,] xy)
        {

            canvas_RecordingPath.Children.Clear();
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
                    canvas_RecordingPath.Children.Add(newlabel);
                }
            }
            //直线对象
            canvas_RecordingPath.Children.Add(pg);
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
                prec[i, 0] = Math.Abs(p0[i, 0] - p0[0, 0]) * 50000;
                prec[i, 1] = Math.Abs(p0[i, 1] - p0[0, 1]) * 50000;
                if (i == 0 || i == 5 || i == 12) { prec[i, 2] = 1; }
            }
            return prec;
        }
    }
}
