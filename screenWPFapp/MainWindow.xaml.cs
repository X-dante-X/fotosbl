using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Interop;

namespace screenWPFapp
{
    public partial class MainWindow : Window
    {
        private const int HotkeyModifiers = 0x0000;
        private const int Hotkey1 = 0x31;
        private const int Hotkey2 = 0x32;

        private HwndSource hwndSource;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public MainWindow()
        {
            
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var windowInteropHelper = new WindowInteropHelper(this);
            hwndSource = HwndSource.FromHwnd(windowInteropHelper.Handle);
            hwndSource.AddHook(WndProc);

            RegisterHotKey(hwndSource.Handle, Hotkey1, HotkeyModifiers, (int)Key.NumPad5);
            RegisterHotKey(hwndSource.Handle, Hotkey2, HotkeyModifiers, (int)Key.NumPad6);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnregisterHotKey(hwndSource.Handle, Hotkey1);
            UnregisterHotKey(hwndSource.Handle, Hotkey2);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0312)
            {
                int id = wParam.ToInt32();
                if (id == Hotkey1)
                {
                    this.Activate();
                    this.Topmost = true;
                }
                if (id == Hotkey2)
                {
                    this.Close();
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void ActiveWindowButton_Click(object sender, RoutedEventArgs e)
        {
            var rect = new RECT();
            GetWindowRect(GetForegroundWindow(), ref rect);
            using (Bitmap bmp = new Bitmap(rect.Width, rect.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rect.X, rect.Y, 0, 0, bmp.Size);
                }
                SaveScreenshot(bmp);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }
        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
            int screenHeight = (int)SystemParameters.PrimaryScreenHeight;
            using (Bitmap bmp = new Bitmap(screenWidth, screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                }
                SaveScreenshot(bmp);
            }
        }

        private void SaveScreenshot(Bitmap screenshot)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG|*.png|JPEG |.jpg;.jpeg | BMP |.bmp | GIF |.gif";
            saveFileDialog.Title = "Save Screenshot";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                screenshot.Save(saveFileDialog.FileName);
            }
        }
    }
}
