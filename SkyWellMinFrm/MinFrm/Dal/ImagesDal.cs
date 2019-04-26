using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MinFrm.Dal
{
    public class ImagesDal
    {
        // 切割图片
        public BitmapSource cutImage(string imgurl, Int32Rect rc)
        {
            BitmapSource newBitmapSource = SystemUtils.CutImage(new BitmapImage(new Uri(imgurl, UriKind.Relative)), rc);
            return newBitmapSource;
        }

        // 图像工具类
        public static class SystemUtils
        {
            /// <summary>
            /// 切图
            /// </summary>
            /// <param name="bitmapSource">图源</param>
            /// <param name="cut">切割区域</param>
            /// <returns></returns>
            public static BitmapSource CutImage(BitmapSource bitmapSource, Int32Rect cut)
            {
                //计算Stride
                var stride = bitmapSource.Format.BitsPerPixel * cut.Width / 8;
                //声明字节数组
                byte[] data = new byte[cut.Height * stride];
                //调用CopyPixels
                bitmapSource.CopyPixels(cut, data, stride, 0);

                return BitmapSource.Create(cut.Width, cut.Height, 0, 0, PixelFormats.Bgr32, null, data, stride);
            }

            // ImageSource --> Bitmap
            public static System.Drawing.Bitmap ImageSourceToBitmap(ImageSource imageSource)
            {
                BitmapSource m = (BitmapSource)imageSource;

                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(m.PixelWidth, m.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                System.Drawing.Imaging.BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride); bmp.UnlockBits(data);

                return bmp;
            }

            // Bitmap --> BitmapImage
            public static BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);

                    stream.Position = 0;
                    BitmapImage result = new BitmapImage();
                    result.BeginInit();
                    // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                    // Force the bitmap to load right now so we can dispose the stream.
                    result.CacheOption = BitmapCacheOption.OnLoad;
                    result.StreamSource = stream;
                    result.EndInit();
                    result.Freeze();

                    return result;
                }
            }
        }
        //窗口重绘,在窗体上显示图像,重载Paint
        private System.Windows.Controls.Image movingObject;  // 记录当前被拖拽移动的图片
        private System.Windows.Point StartPosition; // 本次移动开始时的坐标点位置
        private System.Windows.Point EndPosition;   // 本次移动结束时的坐标点位置

        /// <summary>
        /// 按下鼠标左键，准备开始拖动图片
        /// </summary>
        /// <param name="p"></param>
        private void MouseLeftButtonDownCommand(object[] p)
        {
            object sender = p[0];
            MouseButtonEventArgs e = p[1] as MouseButtonEventArgs;
            System.Windows.Controls.Image img = sender as System.Windows.Controls.Image;

            movingObject = img;
            StartPosition = e.GetPosition(img);
        }

        /// <summary>
        /// 按住鼠标左键，拖动图片平移
        /// </summary>
        /// <param name="p"></param>
        private void MouseMoveCommand(object[] p)
        {
            object sender = p[0];
            MouseEventArgs e = p[1] as MouseEventArgs;
            System.Windows.Controls.Image img = sender as System.Windows.Controls.Image;

            if (e.LeftButton == MouseButtonState.Pressed && sender == movingObject)
            {
                EndPosition = e.GetPosition(img);

                TransformGroup tg = img.RenderTransform as TransformGroup;
                var tgnew = tg.CloneCurrentValue();
                if (tgnew != null)
                {
                    TranslateTransform tt = tgnew.Children[0] as TranslateTransform;

                    var X = EndPosition.X - StartPosition.X;
                    var Y = EndPosition.Y - StartPosition.Y;
                    tt.X += X;
                    tt.Y += Y;
                }

                // 重新给图像赋值Transform变换属性
                img.RenderTransform = tgnew;
            }
        }

        /// <summary>
        /// 鼠标左键弹起，结束图片的拖动
        /// </summary>
        /// <param name="p"></param>
        private void MouseLeftButtonUpCommand(object[] p)
        {
            object sender = p[0];
            MouseButtonEventArgs e = p[1] as MouseButtonEventArgs;
            System.Windows.Controls.Image img = sender as System.Windows.Controls.Image;
            movingObject = null;
        }

        /// <summary>
        /// 图片左转
        /// </summary>
        /// <param name="img">被操作的前台Image控件</param>
        public void RotateLeft(System.Windows.Controls.Image img)
        {
            TransformGroup tg = img.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                RotateTransform rt = tgnew.Children[2] as RotateTransform;
                img.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                rt.Angle -= 5;
            }

            // 重新给图像赋值Transform变换属性
            img.RenderTransform = tgnew;
        }

        /// <summary>
        /// 图片右转
        /// </summary>
        /// <param name="img">被操作的前台Image控件</param>
        public void RotateRight(System.Windows.Controls.Image img)
        {
            TransformGroup tg = img.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                RotateTransform rt = tgnew.Children[2] as RotateTransform;
                img.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                rt.Angle += 5;
            }

            // 重新给图像赋值Transform变换属性
            img.RenderTransform = tgnew;
        }
    }
}
