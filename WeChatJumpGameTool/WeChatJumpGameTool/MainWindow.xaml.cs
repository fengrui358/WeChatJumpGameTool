using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace WeChatJumpGameTool
{
    /// <summary>
    /// 微信跳一跳辅助程序
    /// </summary>
    public partial class MainWindow
    {
        #region Win32

        [DllImport("user32")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        //移动鼠标 
        const int MOUSEEVENTF_MOVE = 0x0001;

        //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;

        //模拟鼠标左键抬起 
        const int MOUSEEVENTF_LEFTUP = 0x0004;

        //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;

        //模拟鼠标右键抬起 
        const int MOUSEEVENTF_RIGHTUP = 0x0010;

        //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;

        //模拟鼠标中键抬起 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;

        //标示是否采用绝对坐标 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        #endregion

        private readonly double _coefficient = 2.77002243950134;

        private POINT _windowPoint;

        private POINT _pressedPoint;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var wp = PointToScreen(new Point(0, 0));
            _windowPoint = new POINT((int) wp.X, (int) wp.Y);
            _windowPoint.X += 20;
            _windowPoint.Y += 20;
        }

        private void MarkStartCommandExcute(object sender, ExecutedRoutedEventArgs e)
        {
            //获取当前的鼠标位置
            var pOut = new POINT();
            GetCursorPos(out pOut);

            StartPoint.Text = $"{pOut.X};{pOut.Y}";
        }

        private void JumpCommandExcute(object sender, ExecutedRoutedEventArgs e)
        {
            //获取当前的鼠标位置
            var pOut = new POINT();
            GetCursorPos(out pOut);

            EndPoint.Text = $"{pOut.X};{pOut.Y}";

            Jump();

            //返回窗体激活快捷键
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, _windowPoint.X * (65536 / 1920), _windowPoint.Y * (65536 / 1080),
                0, 0);

            //点击一下
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

            //移动到手机屏幕
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, _pressedPoint.X * (65536 / 1920), _pressedPoint.Y * (65536 / 1080),
                0, 0);
        }

        private void Jump()
        {
            var startPoint = StartPoint.Text;
            var endPoint = EndPoint.Text;
            var startPointX = Convert.ToDouble(startPoint.Split(';')[0]);
            var startPointY = Convert.ToDouble(startPoint.Split(';')[1]);
            var endPointX = Convert.ToDouble(endPoint.Split(';')[0]);
            var endPointY = Convert.ToDouble(endPoint.Split(';')[1]);

            var pressedX = Convert.ToInt32((startPointX + endPointX) / 2);
            var pressedY = Convert.ToInt32((startPointY + endPointY) / 2);
            _pressedPoint = new POINT(pressedX, pressedY);

            //按压时间
            double value = Math.Sqrt(Math.Pow(endPointY - startPointY, 2) + Math.Pow(endPointX - startPointX, 2)) *
                           _coefficient;

            //移动到手机屏幕
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, pressedX * (65536 / 1920), pressedY * (65536 / 1080),
                0, 0);

            //按压
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);

            Thread.Sleep(TimeSpan.FromMilliseconds(value));
            //await Task.Delay(TimeSpan.FromMilliseconds(value));
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
    }

    public struct POINT
    {
        public int X;
        public int Y;
        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}