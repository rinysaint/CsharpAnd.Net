/************************************
*公司名称：开沃集团
*命名空间：MinFrm
*文件名：PadControl.xaml.cs
*版本号：V1.0.0.0
*创建人:肖日宁
*电子邮件：a11010203@qq.com
*联系方式:QQ:164840753
*创建时间：2019年3月8日
*描述：平板模式窗体文件
*************************************/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MinFrm.Dal;
using System.Windows.Media.Animation;
using System.Threading;

namespace MinFrm
{
    /// <summary>
    /// PadControl.xaml 的交互逻辑
    /// </summary>
    public partial class PadControl : Window
    {
        ImagesDal imgdal = new ImagesDal();
        public string getSpeed { get; set; }//定义一个可读可写的公用的字符串：getSpeed
        public PadControl()
        {
            InitializeComponent();
            loaddata();
            //if (getSpeed != "") { label9.Content = getSpeed; }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确认退出遥控模式？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            else
            {
                this.Close();
                //Application.Current.Shutdown();
            }
        }
        private void loaddata()
        {
            Image img1 = new Image();
            img1.Source = new BitmapImage(new Uri(@"img/padcontrolwheel.gif", UriKind.Relative));
            //img1.Width = 240; img1.Height = 240;
            canvas_PadControl.Children.Add(img1);

            Image img2 = new Image();
            img2.Source = new BitmapImage(new Uri(@"img/padcontrolcarwheel.gif", UriKind.Relative));
            //img2.Width = 850; img2.Height = 550;
            canvas_PadControl_wheel.Children.Add(img2);

            Image img3 = new Image();
            img3.Source = new BitmapImage(new Uri(@"img/padcontrolgear.gif", UriKind.Relative));
            img3.Width = 150; img3.Height = 100;
            canvas_PadControl_gear.Children.Add(img3);

            Image img4 = new Image();
            img4.Source = new BitmapImage(new Uri(@"img/padcontrolspeed.gif", UriKind.Relative));
            //img.Width = 850; img.Height = 550;
            canvas_PadControl_speed.Children.Add(img4);

            Image img5 = new Image();
            img5.Source = new BitmapImage(new Uri(@"img/padcontrolspeedcontrol.gif", UriKind.Relative));
            //img.Width = 850; img.Height = 550;
            canvas_PadControl_speedcontrol.Children.Add(img5);
        }

        //触摸转动
        //public static readonly RoutedEvent TouchDownEvent;
        //public static readonly RoutedEvent TouchEnterEvent;
        //public static readonly RoutedEvent TouchLeaveEvent;
        //public static readonly RoutedEvent TouchMoveEvent;
        //public static readonly RoutedEvent TouchUpEvent;

        static Point pstart;//开始坐标
        static Point pend;//结束坐标
        private void canvas_PadControl_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var element = e.Source as FrameworkElement;
            if (element != null)
            {
                if (e.IsInertial)
                {
                    Rect containingRect = new Rect(((FrameworkElement)e.ManipulationContainer).RenderSize);

                    Rect shapeBounds = element.RenderTransform.TransformBounds(new Rect(element.RenderSize));
                    if (e.IsInertial && !containingRect.Contains(shapeBounds))
                    {
                        e.ReportBoundaryFeedback(e.DeltaManipulation);
                        e.Complete();
                    }
                }
                try
                {
                    ManipulationDelta deltaManipulation = e.DeltaManipulation;
                    Matrix matrix = element.RenderTransform.Value;
                    Point center = new Point(element.ActualWidth / 2, element.ActualHeight / 2);
                    center = matrix.Transform(center);  //设置中心点
                    //处理缩放
                    //matrix.ScaleAt(deltaManipulation.Scale.X, deltaManipulation.Scale.Y, center.X, center.Y);
                    // 处理旋转
                    matrix.RotateAt(e.DeltaManipulation.Rotation, center.X, center.Y);
                    //处理移动

                    //matrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
                    //pstart = new Point(((FrameworkElement)e.ManipulationContainer).);
                    element.RenderTransform = new MatrixTransform(matrix);
                    //MessageBox.Show(e.DeltaManipulation.Rotation.ToString());

                    e.Handled = true;
                    resetcanvas(e.DeltaManipulation.Rotation);
                    label6.Content = e.DeltaManipulation.Rotation; 
                    canvas_PadControl_wheel_ManipulationDelta(sender,e);
                }
                catch (Exception ei)
                {
                    MessageBox.Show(ei.ToString());
                }
            }
        } 
        private void canvas_PadControl_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            // 移动惯性
            //e.TranslationBehavior = new InertiaTranslationBehavior()
            //{
            //    InitialVelocity = e.InitialVelocities.LinearVelocity,
            //    DesiredDeceleration = 1 / (1000.0 * 1000.0)   // 单位：一个WPF单位 / ms
            //};

            // 缩放惯性
            //e.ExpansionBehavior = new InertiaExpansionBehavior()
            //{
            //    InitialVelocity = e.InitialVelocities.ExpansionVelocity,
            //    DesiredDeceleration = 1 / 1000.0 * 1000.0   // 单位：一个WPF单位 / ms
            //};

            // 旋转惯性
            e.RotationBehavior = new InertiaRotationBehavior()
            {
                InitialVelocity = e.InitialVelocities.AngularVelocity,
                DesiredDeceleration = 720 / (1000.0 * 1000.0)  //单位：一个角度 / ms
            };
            e.Handled = true;
            canvas_PadControl_wheel_ManipulationInertiaStarting(sender,e);
        }
        private void canvas_PadControl_wheel_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var element = e.Source as FrameworkElement;
            if (element != null)
            {
                if (e.IsInertial)
                {
                    Rect containingRect = new Rect(((FrameworkElement)e.ManipulationContainer).RenderSize);

                    Rect shapeBounds = element.RenderTransform.TransformBounds(new Rect(element.RenderSize));
                    if (e.IsInertial && !containingRect.Contains(shapeBounds))
                    {
                        e.ReportBoundaryFeedback(e.DeltaManipulation);
                        e.Complete();
                    }
                }
                try
                {
                    Matrix matrix = element.RenderTransform.Value;
                    Point center = new Point(element.ActualWidth / 2, element.ActualHeight / 2);
                    center = matrix.Transform(center);  //设置中心点
                    // 处理旋转
                    matrix.RotateAt(e.DeltaManipulation.Rotation/5, center.X, center.Y);

                    element.RenderTransform = new MatrixTransform(matrix);
                    e.Handled = true;
                    label8.Content = e.DeltaManipulation.Rotation / 5;
                }
                catch (Exception ei)
                {
                    MessageBox.Show(ei.ToString());
                }
            }
        }

        private void canvas_PadControl_wheel_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            // 旋转惯性
            e.RotationBehavior = new InertiaRotationBehavior()
            {
                InitialVelocity = e.InitialVelocities.AngularVelocity,
                DesiredDeceleration = 720 / (1000.0 * 1000.0)  //单位：一个角度 / ms
            };
            e.Handled = true;
        }
        //触摸手势
        private void canvas_PadControl_TouchDown(object sender, TouchEventArgs e)
        {
            pstart = new Point(0, 0);
            pstart = e.GetTouchPoint(canvas_PadControl).Position;//WPF方法
        }
        private void canvas_PadControl_TouchUp(object sender, TouchEventArgs e)
        {
            var element = e.Source as FrameworkElement;
            pend = e.GetTouchPoint(canvas_PadControl).Position;//WPF方法
            Matrix matrix = element.RenderTransform.Value;

            //三个点，O为原点，A、B为圆上另外两点
            Point Point_O = new Point(180, 180);
            Point Point_A = pstart;
            Point Point_B = pend;

            //x轴上的向量
            int Vector_Xx = 360;
            int Vector_Xy = 0;

            //oa向量
            double Vector_ax = Point_A.X - Point_O.X;
            double Vector_ay = Point_A.Y - Point_O.Y;

            //ob向量
            double Vector_bx = Point_B.X - Point_O.X;
            double Vector_by = Point_B.Y - Point_O.Y;

            //oa和X轴上向量的点乘积
            double Point_Mul_a = (Vector_ax * Vector_Xx) + (Vector_ay * Vector_Xy);
            double Mul_a = Math.Sqrt(Vector_ax * Vector_ax + Vector_ay * Vector_ay) * Math.Sqrt(Vector_Xx * Vector_Xx + Vector_Xy * Vector_Xy);


            //计算oa和x轴夹角余弦值
            double Cos_a = Point_Mul_a / Mul_a;
            double A_Cos = Math.Acos(Cos_a);

            //求出几何坐标系中的角度，即按逆时针的方法
            double A_Angle = A_Cos * (180 / Math.PI);


            //b和X轴上向量的点乘积
            double Point_Mul_b = (Vector_bx * Vector_Xx) + (Vector_by * Vector_Xy);
            double Mul_b = Math.Sqrt(Vector_bx * Vector_bx + Vector_by * Vector_by) * Math.Sqrt(Vector_Xx * Vector_Xx + Vector_Xy * Vector_Xy);

            ////计算b和x轴夹角余弦值
            double Cos_b = Point_Mul_b / Mul_b;
            double B_Cos = Math.Acos(Cos_b);

            //求出几何坐标系中的角度，即按逆时针的方法
            double B_Angle = B_Cos * (180 / Math.PI);

            //if (pstart.X - pend.X < 0) { B_Angle = -B_Angle; }

            resetcanvas(A_Angle - B_Angle); //resetarc(Math.Abs(A_Angle - B_Angle));
        }

        //绘制手势
        private void canvas_PadControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            pstart = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法
        }
        //鼠标离手后进行路径绘制，计算角度进行方向盘的操作
        private void canvas_PadControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            pend = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法

            double radians = Math.Atan2(pstart.Y - pend.Y,pstart.X-pend.X);
            double angles = radians * (180 / Math.PI);
            //三个点，O为原点，A、B为圆上另外两点
            Point Point_O = new Point(180, 180);
            Point Point_A = pstart;
            Point Point_B = pend;

            //x轴上的向量
            int Vector_Xx = 360;
            int Vector_Xy = 0;

            //oa向量
            double Vector_ax = Point_A.X - Point_O.X;
            double Vector_ay = Point_A.Y - Point_O.Y;

            //ob向量
            double Vector_bx = Point_B.X - Point_O.X;
            double Vector_by = Point_B.Y - Point_O.Y;

            //oa和X轴上向量的点乘积
            double Point_Mul_a = (Vector_ax * Vector_Xx) + (Vector_ay * Vector_Xy);
            double Mul_a = Math.Sqrt(Vector_ax * Vector_ax + Vector_ay * Vector_ay) * Math.Sqrt(Vector_Xx * Vector_Xx + Vector_Xy * Vector_Xy);


            //计算oa和x轴夹角余弦值
            double Cos_a = Point_Mul_a / Mul_a;
            double A_Cos = Math.Acos(Cos_a);

            //求出几何坐标系中的角度，即按逆时针的方法
            double A_Angle = A_Cos * (180 / Math.PI);


            //b和X轴上向量的点乘积
            double Point_Mul_b = (Vector_bx * Vector_Xx) + (Vector_by * Vector_Xy);
            double Mul_b = Math.Sqrt(Vector_bx * Vector_bx + Vector_by * Vector_by) * Math.Sqrt(Vector_Xx * Vector_Xx + Vector_Xy * Vector_Xy);

            ////计算b和x轴夹角余弦值
            double Cos_b = Point_Mul_b / Mul_b;
            double B_Cos = Math.Acos(Cos_b);

            //求出几何坐标系中的角度，即按逆时针的方法
            double B_Angle = B_Cos * (180 / Math.PI);

            if (pstart.X - pend.X < 0) { B_Angle = -B_Angle; }

            //resetcanvas(A_Angle - B_Angle);// resetarc(Math.Abs(A_Angle -B_Angle));
            if (pstart.X - pend.X < 0) { angles = -angles; }
            resetcanvas(angles);

        }
        //绘制手势圆弧
        private void resetarc(double B_Angle)
        {
            Path path = new Path();
            PathGeometry pathGeometry = new PathGeometry();
            ArcSegment arc = new ArcSegment(pend, new Size(Math.Abs(pstart.X-pend.X), Math.Abs(pstart.Y-pend.Y)), Math.Abs(B_Angle), false, SweepDirection.Counterclockwise, true);
            PathFigure figure = new PathFigure();
            figure.StartPoint = pstart;
            figure.Segments.Add(arc);
            pathGeometry.Figures.Add(figure);
            path.Data = pathGeometry;
            path.Stroke = Brushes.Orange;
            canvas_PadControl.Children.Add(path);
        }
        //方向盘转向
        private void resetcanvas( double B_Angle)
        {
            //重构图片
            canvas_PadControl.Children.Clear();
            canvas_PadControl_wheel.Children.Clear();
            //旋转 
            RotateTransform angle = new RotateTransform();
            TransformGroup group = new TransformGroup();
            group.Children.Add(angle);
            EasingFunctionBase easeFunction = new PowerEase()
            {
                EasingMode = EasingMode.EaseInOut,
                Power = 7
            };
            DoubleAnimation angleAnimation = new DoubleAnimation()
            {
                From = 0, //起始值 
                To = B_Angle, //结束值 
                EasingFunction = easeFunction, //缓动函数 
                Duration = new TimeSpan(0, 0, 0, 3, 0), //动画播放时间 
            };

            Image image = new Image();
            image.SetValue(Canvas.LeftProperty, (double)0);
            image.SetValue(Canvas.TopProperty, (double)0);
            image.Source = new BitmapImage(new Uri(@"img/padcontrolwheel.gif", UriKind.Relative));
            image.RenderTransform = group;
            image.RenderTransformOrigin = new Point(0.5, 0.5);//定义圆心位置 0.5，0.5表示中心0，0左上角，1,1右下角

            angle.BeginAnimation(RotateTransform.AngleProperty, angleAnimation);
            //车轮操作
            RotateTransform angle2 = new RotateTransform();
            TransformGroup group2 = new TransformGroup();
            group2.Children.Add(angle2);

            DoubleAnimation angleAnimation2 = new DoubleAnimation()
            {
                From = 0, //起始值 
                To = B_Angle / 5, //结束值 
                EasingFunction = easeFunction, //缓动函数 
                Duration = new TimeSpan(0, 0, 0, 5, 0), //动画播放时间 
            };
            Image image2 = new Image();
            image2.SetValue(Canvas.LeftProperty, (double)0);
            image2.SetValue(Canvas.TopProperty, (double)0);
            image2.Source = new BitmapImage(new Uri(@"img/padcontrolcarwheel.gif", UriKind.Relative));
            image2.RenderTransform = group2;
            image2.RenderTransformOrigin = new Point(0.5, 0.5);//定义圆心位置  0.5，0.5表示中心0，0左上角，1,1右下角

            angle2.BeginAnimation(RotateTransform.AngleProperty, angleAnimation2);

            //填充方向盘转动的图片和文字
            canvas_PadControl.Children.Add(image);
            canvas_PadControl_wheel.Children.Add(image2);            

            label6.Content = B_Angle; label8.Content = B_Angle / 5;
        }
        static double speednum = 23.6;static bool mosdown = false;
        private void canvas_PadControl_speedcontrol_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //mosdown = true;
            //while (mosdown)
            //{
                speednum++;
                label9.Content = speednum + " km/h";
                Thread.Sleep(1000);
            //}
        }

        private void canvas_PadControl_speedcontrol_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mosdown = false;
        }
        
    }
}
