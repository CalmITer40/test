using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace SeaBattle
{
    class Drawing
    {
        private Graphics g;
        private Bitmap bmp;

        public Graphics GetFall
        {
            get
            {
                bmp = new Bitmap(40, 40);
                g = Graphics.FromImage(bmp);
                g.Clear(Color.White);
                g.DrawLine(new Pen(Brushes.Red, 3), 8, 8, 32, 32);
                g.DrawLine(new Pen(Brushes.Red, 3), 8, 32, 32, 8);

                return g;
            }
        }

    }
}
