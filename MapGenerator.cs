using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace SeaBattle
{
    public class MapGenerator
    {
        private const int N = 12;
        private readonly int[,] map = new int[N, N];
        private List<Point[]> ships = new List<Point[]>();
        private int four, three, two, one;
        private Random rand = new Random();

        private enum Rotate
        {
            TOP,
            RIGHT,
            BOTTOM,
            LEFT
        }

        public int[,] GetMap
        {
            get
            {
                return map;
            }
        }

        public List<Point[]> GetCoord
        {
            get
            {
                return ships;
            }
        }

        public MapGenerator()
        {
            four = 1;
            three = 2;
            two = 3;
            one = 4;
        }

        public void Generate()
        {
            //Preparing matrix
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i == 0 || j == 0 || j == N - 1 || i == N - 1)
                        map[i, j] = 9;
                    else
                        map[i, j] = 0;
                }
            }



            while (one > 0 || two > 0 || three > 0 || four > 0)
            {
                int type = rand.Next(1, 5);

                while (true)
                {
                    int i = rand.Next(1, 11);
                    int j = rand.Next(1, 11);
                    if (map[i, j] == 1)
                        continue;

                    int rotate = rand.Next(0, 4);

                    if (type == 1 && one > 0)
                    {
                        try
                        {
                            if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1)
                            {
                                map[i, j] = 1;
                                ships.Add(new Point[] { new Point(i, j) });
                                one--;
                                continue;
                            }
                            else
                                continue;
                        }
                        catch
                        {
                            break;
                        }
                    }
                    else if (type == 2 && two > 0)
                    {
                        if (rotate == 0)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i - 2, j] != 1 && map[i - 2, j - 1] != 1 && map[i - 2, j + 1] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i - 1, j] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i - 1, j) });
                                    two--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                        else if (rotate == 1)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i - 1, j + 2] != 1 && map[i, j + 2] != 1 && map[i + 1, j + 2] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i, j + 1] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i, j + 1) });
                                    two--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                        else if (rotate == 2)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i + 2, j + 1] != 1 && map[i + 2, j] != 1 && map[i + 2, j - 1] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i + 1, j] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i + 1, j) });
                                    two--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                        else if (rotate == 3)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i - 1, j - 2] != 1 && map[i, j - 2] != 1 && map[i + 1, j - 2] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i, j - 1] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i, j - 1) });
                                    two--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                    else if (type == 3 && three > 0)
                    {
                        if (rotate == 0)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i - 2, j] != 1 && map[i - 2, j - 1] != 1 &&
                                    map[i - 2, j + 1] != 1 && map[i - 3, j - 1] != 1 && map[i - 3, j] != 1 && map[i - 3, j + 1] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i - 1, j] = 1;
                                    map[i - 2, j] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i - 1, j), new Point(i - 2, j) });
                                    three--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                        else if (rotate == 1)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i - 1, j + 2] != 1 && map[i, j + 2] != 1 &&
                                    map[i + 1, j + 2] != 1 && map[i - 1, j + 3] != 1 && map[i, j + 3] != 1 && map[i + 1, j + 3] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i, j + 1] = 1;
                                    map[i, j + 2] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i, j + 1), new Point(i, j + 2) });
                                    three--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                        else if (rotate == 2)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i + 2, j + 1] != 1 && map[i + 2, j] != 1 &&
                                    map[i + 2, j - 1] != 1 && map[i + 3, j - 1] != 1 && map[i + 3, j] != 1 && map[i + 3, j + 1] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i + 1, j] = 1;
                                    map[i + 2, j] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i + 1, j), new Point(i + 2, j) });
                                    three--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                        else if (rotate == 3)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i - 1, j - 2] != 1 && map[i, j - 2] != 1 &&
                                    map[i + 1, j - 2] != 1 && map[i - 1, j - 3] != 1 && map[i, j - 3] != 1 && map[i + 1, j - 3] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i, j - 1] = 1;
                                    map[i, j - 2] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i, j - 1), new Point(i, j - 2) });
                                    three--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                    else if (type == 4 && four > 0)
                    {
                        if (rotate == 0)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i - 2, j] != 1 && map[i - 2, j - 1] != 1 &&
                                    map[i - 2, j + 1] != 1 && map[i - 3, j - 1] != 1 && map[i - 3, j] != 1 && map[i - 3, j + 1] != 1 && map[i - 4, j - 1] != 1 &&
                                    map[i - 4, j] != 1 && map[i - 4, j + 1] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i - 1, j] = 1;
                                    map[i - 2, j] = 1;
                                    map[i - 3, j] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i - 1, j), new Point(i - 2, j), new Point(i - 3, j) });
                                    four--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                        else if (rotate == 1)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i - 1, j + 2] != 1 && map[i, j + 2] != 1 &&
                                    map[i + 1, j + 2] != 1 && map[i - 1, j + 3] != 1 && map[i, j + 3] != 1 && map[i + 1, j + 3] != 1 && map[i - 1, j + 4] != 1 &&
                                    map[i, j + 4] != 1 && map[i + 1, j + 4] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i, j + 1] = 1;
                                    map[i, j + 2] = 1;
                                    map[i, j + 3] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i, j + 1), new Point(i, j + 2), new Point(i, j + 3) });
                                    four--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                        else if (rotate == 2)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i + 2, j + 1] != 1 && map[i + 2, j] != 1 &&
                                    map[i + 2, j - 1] != 1 && map[i + 3, j - 1] != 1 && map[i + 3, j] != 1 && map[i + 3, j + 1] != 1 && map[i + 4, j - 1] != 1 &&
                                    map[i + 4, j] != 1 && map[i + 4, j + 1] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i + 1, j] = 1;
                                    map[i + 2, j] = 1;
                                    map[i + 3, j] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i + 1, j), new Point(i + 2, j), new Point(i + 3, j) });
                                    four--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                        else if (rotate == 3)
                        {
                            try
                            {
                                if (map[i, j] != 1 && map[i - 1, j] != 1 && map[i + 1, j] != 1 && map[i, j - 1] != 1 && map[i, j + 1] != 1 && map[i - 1, j - 1] != 1 &&
                                    map[i + 1, j + 1] != 1 && map[i - 1, j + 1] != 1 && map[i + 1, j - 1] != 1 && map[i - 1, j - 2] != 1 && map[i, j - 2] != 1 &&
                                    map[i + 1, j - 2] != 1 && map[i - 1, j - 3] != 1 && map[i, j - 3] != 1 && map[i + 1, j - 3] != 1 && map[i - 1, j - 4] != 1 &&
                                    map[i, j - 4] != 1 && map[i + 1, j - 4] != 1)
                                {
                                    map[i, j] = 1;
                                    map[i, j - 1] = 1;
                                    map[i, j - 2] = 1;
                                    map[i, j - 3] = 1;
                                    ships.Add(new Point[] { new Point(i, j), new Point(i, j - 1), new Point(i, j - 2), new Point(i, j - 3) });
                                    four--;
                                    continue;
                                }
                                else
                                    continue;
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                    else
                        break;
                }
            }


            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (map[i, j] == 1)
                        map[i, j] = 5;
                }
            }

        }
    }
}
