using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace сигнализация
{
    public partial class Form2 : Form
    {
        string p;
        public Form2(string path)
        {
            InitializeComponent();
            p = path;
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            Size = new Size(bounds.Width, bounds.Height);
        }

        private void Form2_Click(object sender, EventArgs e)
        {
            int h = MousePosition.X;
            int w = MousePosition.Y;
            string path1 = Path.GetFileName(p + "\\Point.txt");
            StreamWriter sw = new StreamWriter(path1);
            sw.WriteLine(h);
            sw.WriteLine(w);
            sw.Close();
            this.Close();
        }

    }
}
