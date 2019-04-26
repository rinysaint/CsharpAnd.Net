/***************************************************************Copyright (c) 2019 All Rights Reserved.
*公司名称：开沃汽车
*命名空间：SkyWellDashboard
*文件名：AGauge
*版本号：V1.0.0.0
*创建人:肖日宁
*电子邮件：a11010203@qq.com
*联系方式:QQ:164840753
*创建时间：2019/4/23 14:02:47
*描述：AGauge
*************************************************************/
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SkyWellDashboard
{
    class AGaugeRight : IValueConverter
    {
        private const double Thickness = 20;
        private const double Padding = 1;
        private const double WarnValue = 10;
        private const int SuccessRateFontSize = 34;
        private static readonly SolidColorBrush NormalBrush;
        private static readonly SolidColorBrush WarnBrush;
        private static readonly Typeface SuccessRateTypeface;

        //private string percentString;
        private Point centerPoint;
        private double radius;

        static AGaugeRight()
        {
            NormalBrush = new SolidColorBrush(Colors.Red); 
            WarnBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#30ccf5"));
            SuccessRateTypeface = new Typeface(new FontFamily("MSYH"), new FontStyle(), new FontWeight(), new FontStretch());
        }

        public AGaugeRight()
        {

        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double && !string.IsNullOrEmpty((string)parameter))
            {
                double arg = (double)value;
                double width = double.Parse((string)parameter);
                radius = width / 2;
                centerPoint = new Point(radius, radius);

                return DrawBrush(arg, 100, radius, radius, Thickness, Padding);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据角度获取坐标
        /// </summary>
        /// <param name="CenterPoint"></param>
        /// <param name="r"></param>
        /// <param name="angel"></param>
        /// <returns></returns>
        private Point GetPointByAngel(Point CenterPoint, double r, double angel)
        {
            Point p = new Point();
            p.X = Math.Sin(angel * Math.PI / 180) * r + CenterPoint.X;
            p.Y = CenterPoint.Y - Math.Cos(angel * Math.PI / 180) * r;

            return p;
        }

        /// <summary>
        /// 根据4个坐标画出扇形
        /// </summary>
        /// <param name="bigFirstPoint"></param>
        /// <param name="bigSecondPoint"></param>
        /// <param name="smallFirstPoint"></param>
        /// <param name="smallSecondPoint"></param>
        /// <param name="bigRadius"></param>
        /// <param name="smallRadius"></param>
        /// <param name="isLargeArc"></param>
        /// <returns></returns>
        private Geometry DrawingArcGeometry(Point bigFirstPoint, Point bigSecondPoint, Point smallFirstPoint, Point smallSecondPoint, double bigRadius, double smallRadius, bool isLargeArc)
        {
            PathFigure pathFigure = new PathFigure { IsClosed = true };
            pathFigure.StartPoint = bigFirstPoint;
            pathFigure.Segments.Add(
              new ArcSegment
              {
                  Point = bigSecondPoint,
                  IsLargeArc = isLargeArc,
                  Size = new Size(bigRadius, bigRadius),
                  SweepDirection = SweepDirection.Clockwise
              });
            pathFigure.Segments.Add(new LineSegment { Point = smallSecondPoint });
            pathFigure.Segments.Add(
             new ArcSegment
             {
                 Point = smallFirstPoint,
                 IsLargeArc = isLargeArc,
                 Size = new Size(smallRadius, smallRadius),
                 SweepDirection = SweepDirection.Counterclockwise
             });
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            return pathGeometry;
        }

        /// <summary>
        /// 根据当前值和最大值获取扇形
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        private Geometry GetGeometry(double value, double maxValue, double radiusX, double radiusY, double thickness, double padding)
        {
            bool isLargeArc = false;
            double percent = value / maxValue;
            double angel = percent * 360D;
            if (angel > 180) isLargeArc = true;
            double bigR = radiusX;
            double smallR = radiusX - thickness + padding;
            Point firstpoint = GetPointByAngel(centerPoint, bigR, 0);
            Point secondpoint = GetPointByAngel(centerPoint, bigR, angel);
            Point thirdpoint = GetPointByAngel(centerPoint, smallR, 0);
            Point fourpoint = GetPointByAngel(centerPoint, smallR, angel);
            return DrawingArcGeometry(firstpoint, secondpoint, thirdpoint, fourpoint, bigR, smallR, isLargeArc);
        }

        private void DrawingGeometry(DrawingContext drawingContext, double value, double maxValue, double radiusX, double radiusY, double thickness, double padding)
        {
            if (value != maxValue)
            {
                SolidColorBrush brush;
                if (value < WarnValue)
                {
                    brush = NormalBrush;
                }
                else
                {
                    brush = WarnBrush;
                }
                drawingContext.DrawEllipse(null, new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#30ccf5")), 0), centerPoint, radiusX, radiusY);//Color.FromRgb(0xdd, 0xdf, 0xe1)
                drawingContext.DrawGeometry(brush, new Pen(), GetGeometry(value, maxValue, radiusX, radiusY, thickness, padding));
            }
            drawingContext.Close();
        }

        /// <summary>
        /// 根据当前值和最大值画出进度条
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        private Visual DrawShape(double value, double maxValue, double radiusX, double radiusY, double thickness, double padding)
        {
            DrawingVisual drawingWordsVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingWordsVisual.RenderOpen();

            DrawingGeometry(drawingContext, value, maxValue, radiusX, radiusY, thickness, padding);

            return drawingWordsVisual;
        }

        /// <summary>
        /// 根据当前值和最大值画出进度条
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        private Brush DrawBrush(double value, double maxValue, double radiusX, double radiusY, double thickness, double padding)
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            DrawingContext drawingContext = drawingGroup.Open();

            DrawingGeometry(drawingContext, value, maxValue, radiusX, radiusY, thickness, padding);

            DrawingBrush brush = new DrawingBrush(drawingGroup);

            return brush;
        }

    }
}

