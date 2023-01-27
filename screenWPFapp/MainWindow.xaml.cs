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


namespace screenWPFapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
