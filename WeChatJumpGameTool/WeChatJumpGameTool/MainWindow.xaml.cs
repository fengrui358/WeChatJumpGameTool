using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

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

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(int hWnd);

        private int _mainHandle;

        #endregion

        private double _coefficient = 2.77002243950134;

        public double Coefficient
        {
            get { return _coefficient; }
            set { _coefficient = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainHandle = (new WindowInteropHelper(this).Handle).ToInt32();
        }

        private void MarkStartCommandExcute(object sender, ExecutedRoutedEventArgs e)
        {
            //获取当前的鼠标位置
            var pOut = new POINT();
            GetCursorPos(out pOut);

            StartPoint.Text = $"{pOut.X};{pOut.Y}";
        }

        private async void JumpCommandExcute(object sender, ExecutedRoutedEventArgs e)
        {
            //获取当前的鼠标位置
            var pOut = new POINT();
            GetCursorPos(out pOut);

            EndPoint.Text = $"{pOut.X};{pOut.Y}";

            Jump(pOut);

            SetForegroundWindow(_mainHandle);
        }

        private void Jump(POINT endPoint)
        {
            var startPoint = StartPoint.Text;
            var startPointX = Convert.ToDouble(startPoint.Split(';')[0]);
            var startPointY = Convert.ToDouble(startPoint.Split(';')[1]);
            var endPointX = Convert.ToDouble(endPoint.X);
            var endPointY = Convert.ToDouble(endPoint.Y);

            var pressedX = Convert.ToInt32((startPointX + endPointX) / 2);
            var pressedY = Convert.ToInt32((startPointY + endPointY) / 2);

            //按压时间
            double value = Math.Sqrt(Math.Pow(endPointY - startPointY, 2) + Math.Pow(endPointX - startPointX, 2)) *
                           _coefficient;

            //移动到手机屏幕
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, pressedX * (65536 / 1920), pressedY * (65536 / 1080),
                0, 0);

            //按压
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);

            Thread.Sleep(TimeSpan.FromMilliseconds(value));

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