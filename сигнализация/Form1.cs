using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;

namespace сигнализация
{
    public partial class Form1 : Form
    {
        string path;
        string path2;
        int x = 0;
        int y = 0;
        int loop = 5;
        bool loopalltime = false;
        bool signal = false;
        SoundPlayer simpleSound;
        private object soundLock = new object();
        private CancellationTokenSource cancellationTokenSource;
        public Form1()
        {
            Size = new Size(347, 178);
            path = Path.GetFullPath(this.ToString());
            InitializeComponent();
            path.Remove(path.Length - 17);
            button3.Enabled = false;
            textBox1.Text = "5";
            string path1 = Path.GetFileName(path + "\\Point.txt");
            path2 = Path.GetFileName(path + "\\signal.wav");
            StreamReader sw = new StreamReader(path1);
            cancellationTokenSource = new CancellationTokenSource();
            x = Int32.Parse(sw.ReadLine());
            y = Int32.Parse(sw.ReadLine());
            sw.Close();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            IntPtr hwnd = FindWindow(null, "Viber");
            CaptureWindow(hwnd);
        }
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr handle, ref Rectangle rect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        public void CaptureWindow(IntPtr handle)
        {
            if (checkBox1.Checked)
            {
                textBox1.Enabled = false;
            }
            else
                textBox1.Enabled = true;
            // Get the size of the window to capture
            Rectangle rect = new Rectangle();
            GetWindowRect(handle, ref rect);
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            rect = bounds;
            rect.Width = rect.Width - rect.X;
            rect.Height = rect.Height - rect.Y;
            simpleSound = new SoundPlayer(path2);
            if (signal)
            {
                Task.Run(() => PlaySound(cancellationTokenSource.Token));
            }
            else
            {
                // Create a bitmap to draw the capture into
                using (Bitmap bitmap = new Bitmap(rect.Width, rect.Height))
                {
                    // Use PrintWindow to draw the window into our bitmap
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, rect.Size);
                    }
                    int pixelColor = bitmap.GetPixel(x, y).R;
                    int pixelColor1 = bitmap.GetPixel(x, y).G;
                    int pixelColor2 = bitmap.GetPixel(x, y).B;
                    if ((pixelColor <= 140 || pixelColor >= 200) || (pixelColor1 <= 0 || pixelColor1 >= 40) || (pixelColor2 <= 200 || pixelColor2 >= 260))
                    {
                        signal = true;
                        button2.BackColor = Color.Red;
                    }
                    else
                    {
                        signal = false;
                    }
                    // Save it as a .png just to demo this
                    //bitmap.Save("Example.png");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = true;
            button2.BackColor = Color.Green;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            simpleSound = new SoundPlayer();
            simpleSound.Stop();
            cancellationTokenSource.Cancel();
            timer1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = false;
            button2.BackColor = Color.FromArgb(224, 224, 224);
            signal = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Rectangle rect = Screen.GetBounds(Point.Empty);
            Bitmap bitmap;
            using (bitmap = new Bitmap(rect.Width, rect.Height))
            {
                // Use PrintWindow to draw the window into our bitmap
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, rect.Size);
                    //IntPtr hdc = g.GetHdc();
                    //if (!PrintWindow(handle, hdc, 0))
                    //{
                    //    int error = Marshal.GetLastWin32Error();
                    //    var exception = new System.ComponentModel.Win32Exception(error);
                    //    MessageBox.Show("ERROR: " + error + ": " + exception.Message);
                    //    // TODO: Throw the exception?
                    //}
                    //g.ReleaseHdc(hdc);
                }
            }
            Form2 f = new Form2(path);
            f.ShowDialog();
            string path1 = Path.GetFileName(path + "\\Point.txt");
            StreamReader sw = new StreamReader(path1);
            x = Int32.Parse(sw.ReadLine());
            y = Int32.Parse(sw.ReadLine());
            sw.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                loop = Int32.Parse(textBox1.Text);
            }
            else
            {
                textBox1.Text = "1";
                loop = 1;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                loopalltime = true;
            else
            {
                loopalltime = false;
            }
        }
        private void PlaySound(CancellationToken cancellationToken)
        {
            try
            {
                if (loopalltime)
                {
                    simpleSound.Play();
                    Thread.Sleep(500);
                }
                else
                {
                    lock (soundLock)
                    {
                        for (int i = 0; i < loop*2; i++)
                        {
                            simpleSound.Play();
                            Thread.Sleep(500);
                        }
                        signal = false;
                        timer1.Stop();
                        button2.Enabled = true;
                        button3.Enabled = false;
                        button2.BackColor = Color.FromArgb(224, 224, 224);
                        if (button2.BackColor == Color.FromArgb(224, 224, 224))
                        {
                            simpleSound = new SoundPlayer();
                            simpleSound.Stop();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Вывести информацию об ошибке в консоль
                MessageBox.Show("Ошибка воспроизведения звука: " + ex.Message);
            }
        }
    }
}
