using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle
{
    class AI
    {
        const int N = 12;
        //private int Rotate;
        private Point BeginPoint;
        private int[,] map = null;
        private Point CurrentPoint;
        private Random rand = new Random();

        public AI()
        {
           // Rotate = 1;
            Dead = false;
        }

        public bool Strike
        {
            get;
            set;
        }

        public bool Dead
        {
            get;
            set;
        }

        public int[,] GetMap
        {
            set
            {
                map = value;
            }
        }

        public Point GetPoint(bool strike)
        {
            if (map == null)
                throw new Exception("Карта не задана!");
            if (!Dead)
            {
                Dead = true;
                BeginPoint = CurrentPoint;
            }


            CurrentPoint = new Point(rand.Next(0, N - 2), rand.Next(0, N - 2));
            return CurrentPoint;
        }
    }
}
