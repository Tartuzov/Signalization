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
        string path2 = "";
        private SoundPlayer simpleSound;
        int x = 0;
        int y = 0;
        int loop = 5;
        bool loopalltime = false;
        bool signal = false;
        //private CancellationTokenSource cancellationTokenSource;
        public Form1()
        {
            Size = new Size(347, 178);
            path = Path.GetFullPath(this.ToString());
            InitializeComponent();
            path.Remove(path.Length - 17);
            button3.Enabled = false;
            textBox1.Text = "5";
            path2 = Path.GetFileName(path + "\\signal.wav");
            string path1 = Path.GetFileName(path + "\\Point.txt");
            StreamReader sw = new StreamReader(path1);
            simpleSound = new SoundPlayer(path2);
            x = Int32.Parse(sw.ReadLine());
            y = Int32.Parse(sw.ReadLine());
            sw.Close();
            cancellationTokenSource = new CancellationTokenSource();
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
            if (this.checkBox1.Checked)
                this.textBox1.Enabled = false;
            else
                this.textBox1.Enabled = true;
            Rectangle rect = new Rectangle();
            Form1.GetWindowRect(handle, ref rect);
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            bounds.Width -= bounds.X;
            bounds.Height -= bounds.Y;
            string fileName = Path.GetFileName(this.path + "\\signal.wav");
            if (this.signal)
            {
                Thread.Sleep(100);
                SoundPlayer soundPlayer = new SoundPlayer(fileName);
                if (this.loopalltime)
                {
                    soundPlayer.Play();
                    Thread.Sleep(500);
                }
                else
                {
                    for (int index = 0; index < this.loop * 2; ++index)
                    {
                        soundPlayer.Play();
                        Thread.Sleep(500);
                    }
                    this.signal = false;
                    this.timer1.Stop();
                    this.button2.Enabled = true;
                    this.button3.Enabled = false;
                    this.button2.BackColor = Color.FromArgb(224, 224, 224);
                }
            }
            else
            {
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics graphics = Graphics.FromImage((Image)bitmap))
                        graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    Color pixel = bitmap.GetPixel(this.x, this.y);
                    int r = (int)pixel.R;
                    pixel = bitmap.GetPixel(this.x, this.y);
                    int g = (int)pixel.G;
                    pixel = bitmap.GetPixel(this.x, this.y);
                    int b = (int)pixel.B;
                    if (r <= 140 || r >= 200 || g <= 0 || g >= 40 || b <= 200 || b >= 260)
                    {
                        this.signal = true;
                        this.button2.BackColor = Color.Red;
                    }
                    else
                        this.signal = false;
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
            simpleSound.Stop();
            timer1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = false;
            button2.BackColor = Color.FromArgb(224, 224, 224);
            signal = false;
            cancellationTokenSource.Cancel();
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
    }
}
