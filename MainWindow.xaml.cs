using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Threading;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Net.Sockets;
using System.Net;
using NATUPNPLib;

namespace SeaBattle
{
    public partial class MainWindow : Window
    {
        private const int N = 12;                               //размер
        private int transform = 1;                              //поворот
        private Point[] points;
        private bool GameMode = false;
        private int four, three, two, one;
        private bool step,uPhp;
        private BitmapSource Strike, Dead;
        private Point begin;
        Random rand = new Random();
        Point currentPoint;
        int currentRotate = 0;
        DateTime current = new DateTime();
        bool connection = false;
        

        private List<int> memory_ships = new List<int>();
        private int[,] flag_ships = new int[N, N];
        private Label[,] labels = new Label[N - 2, N - 2];
        private List<Point[]> ships = new List<Point[]>();
        private List<Point[]> shipsMemory = new List<Point[]>();

        private Label[,] enemy = new Label[N - 2, N - 2];
        private int[,] flag_ships_enemy = new int[N, N];
        private List<Point[]> ships_e = new List<Point[]>();
        private List<Point[]> shipsMemory_e = new List<Point[]>();
        private List<Point> memory_AI = new List<Point>();

        private List<int> memory_pos = new List<int>();
        private List<int> memory_pos_e = new List<int>();

        private delegate void EventArgs();
        private event EventArgs StepEnemy;
        private System.Timers.Timer counter_time;
        private Thread listen;

        static int port;
        static string address;

        private AI ai;
        UPnPNATClass upnpnat = new UPnPNATClass();


        private enum Transformation
        {
            LEFT,
            TOP,
            RIGHT,
            BOTTOM
        }

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i == 0 || j == 0 || i == N - 1 || j == N - 1)
                        flag_ships[i, j] = 9;
                    else
                        flag_ships[i, j] = 0;
                }
            }

            four = 1;
            three = 2;
            two = 3;
            one = 4;

            step = true;
            begin = new Point(-1, -1);
            currentRotate = 0;
            listen = new Thread(Listen);

            StepEnemy += StepEnemy_Click;
            connect.Text = "Адрес прослушиваемого";
            port = 82;

            counter_time = new System.Timers.Timer();
            counter_time.Elapsed += Counter_time_Elapsed;
            counter_time.Interval = 1000;
            
            uPhp = false;
        }

        ~MainWindow()
        {
            if (listen != null)
            {
                listen = null;
            }
            Button_Click(null, null);
        }

        private void Counter_time_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                timer.Text = current.ToLongTimeString();
                current = current.AddSeconds(1);
            });
        }

        private Point AI(bool strike)
        {
            Point point = new Point();
            try
            {
                if (strike && currentPoint.X > 0 && currentPoint.X < 11 && currentPoint.Y > 0 && currentPoint.Y < 11)
                {
                    switch (currentRotate)
                    {
                        case 0:
                            {
                                //if (line && flag_ships[(int)currentPoint.X-1, (int)currentPoint.Y+1] != 9)
                                //{
                                //    point = new Point(currentPoint.X - 1, currentPoint.Y);
                                //}

                                if (flag_ships[(int)currentPoint.X, (int)currentPoint.Y + 1] != 9)
                                {
                                    point = new Point(currentPoint.X - 1, currentPoint.Y);
                                    currentRotate++;
                                }
                                else
                                {
                                    point = new Point(currentPoint.X, currentPoint.Y + 1);
                                    currentRotate += 2;
                                    break;
                                }

                                //if (flag_ships[(int)currentPoint.X + 1, (int)currentPoint.Y + 1] == 3 && flag_ships[(int)currentPoint.X, (int)currentPoint.Y + 1] == 3 && !line)
                                //{
                                //    line = true;
                                //    point = new Point(currentPoint.X + 1, currentPoint.Y + 1);
                                //    currentRotate = 0;
                                //}
                                //else if (flag_ships[(int)currentPoint.X + 1, (int)currentPoint.Y + 1] == 3 && flag_ships[(int)currentPoint.X + 2, (int)currentPoint.Y + 1] == 3 && !line)
                                //{
                                //    line = true;
                                //    point = new Point(currentPoint.X + 2, currentPoint.Y + 1);
                                //    currentRotate = 0;
                                //}
                                MessageBox.Show("AI: " + point.ToString());
                                break;
                            }
                        case 1:
                            {
                                if (flag_ships[(int)currentPoint.X + 1, (int)currentPoint.Y + 2] != 9)
                                {
                                    point = new Point(currentPoint.X, currentPoint.Y + 1);
                                    currentRotate++;
                                }
                                else
                                {
                                    point = new Point(currentPoint.X + 1, currentPoint.Y);
                                    currentRotate += 2;
                                }
                                MessageBox.Show("AI: " + point.ToString());
                                break;
                            }
                        case 2:
                            {
                                if (flag_ships[(int)currentPoint.X + 2, (int)currentPoint.Y + 1] != 9)
                                {
                                    point = new Point(currentPoint.X + 1, currentPoint.Y);
                                    currentRotate++;
                                }
                                else
                                {
                                    point = new Point(currentPoint.X, currentPoint.Y - 1);
                                    currentRotate += 2;
                                }
                                MessageBox.Show("AI: " + point.ToString());
                                break;
                            }
                        case 3:
                            {
                                if (flag_ships[(int)currentPoint.X + 1, (int)currentPoint.Y] != 9)
                                {
                                    point = new Point(currentPoint.X, currentPoint.Y - 1);
                                    currentRotate++;
                                }
                                else
                                {
                                    point = new Point(currentPoint.X - 1, currentPoint.Y);
                                    currentRotate += 2;
                                }
                                MessageBox.Show("AI: " + point.ToString());
                                break;
                            }
                        default:
                            break;
                    }
                    currentRotate = currentRotate > 3 ? 0 : currentRotate;
                    return point;
                }
                else
                {
                    currentPoint = new Point(5, 5);
                    point = new Point(rand.Next(0, N - 2), rand.Next(0, N - 2));
                    return point;
                }
            }
            catch (Exception ex)
            {
                currentRotate++;
                MessageBox.Show("ERROR: " + ex.Message);
                point = new Point(rand.Next(0, N - 2), rand.Next(0, N - 2));
                return currentPoint;
            }
        }

        async private void StepEnemy_Click()
        {
            bool flag = true;
            Point CurrentSender = new Point(5, 5);

            while (flag)
            {
                CurrentSender = currentPoint;

                //CurrentSender = Response();
                //CurrentSender = new Point(rand.Next(0, N - 2), rand.Next(0, N - 2));
                begin = CurrentSender;
                ViewCoord(CurrentSender, Enemy);
                // MessageBox.Show((CurrentSender.X + 1) + ":" + (CurrentSender.Y + 1) + "\n" + currentRotate.ToString() + "\n" + currentPoint.ToString());
                int value = flag_ships[(int)CurrentSender.X + 1, (int)CurrentSender.Y + 1];

                if (GameMode)
                {
                    if (value == 5)
                    {
                        flag_ships[(int)CurrentSender.X + 1, (int)CurrentSender.Y + 1] = 3;
                        Append(chat, "Компьютер стреляет в " + Enemy.Text + " и попадает." + Environment.NewLine, "Red", FontWeights.Normal);
                        chat.ScrollToEnd();
                        WriteShipCoord(CurrentSender, 1);
                        WriteShipCoord(CurrentSender, 2);
                        OnRender(1);
                        continue;
                    }
                    else if (value == 0)
                    {
                        flag_ships[(int)CurrentSender.X + 1, (int)CurrentSender.Y + 1] = 4;
                        flag = false;
                        Append(chat, "Компьютер стреляет в " + Enemy.Text + " и промахивается." + Environment.NewLine, "Red", FontWeights.Normal);
                        chat.ScrollToEnd();
                    }
                    else if (value == 3 || value == 4)
                    {
                        continue;
                    }
                    OnRender(1);
                    step = true;
                }
            }

            DoubleAnimation anim_w = new DoubleAnimation();
            anim_w.To = 40;
            anim_w.From = 60;
            anim_w.Duration = TimeSpan.FromSeconds(2);

            DoubleAnimation anim_h = new DoubleAnimation();
            anim_h.To = 40;
            anim_h.From = 60;
            anim_h.Duration = TimeSpan.FromSeconds(2);

            //labels[(int)CurrentSender.X, (int)CurrentSender.Y].BeginAnimation(Label.WidthProperty, anim_w);
            //labels[(int)CurrentSender.X, (int)CurrentSender.Y].BeginAnimation(Label.HeightProperty, anim_h);
            await Task.Delay(10);

            CheckEndGame(1);
            if (!GameMode)
                return;
            step = true;
            textBlock6.Text = "Ваш ход";
        }

        private void ViewCoord(Point point, TextBlock text)
        {
            text.Text = (point.X + 1).ToString();
            char temp = (char)(point.Y + 'А');
            if (temp == 'Й')
            {
                temp++;
            }
            text.Text += temp;
        }

        private void L1_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (transform == (int)Transformation.LEFT)
            {
                transform = (int)Transformation.TOP;
                if ((bool)b_four.IsChecked)
                {
                    FourShipsClear(sender, e);
                }
                else if ((bool)b_three.IsChecked)
                {
                    ThreeShipClear(sender, e);
                }
                else if ((bool)b_two.IsChecked)
                {
                    TwoShipClear(sender, e);
                }
            }
            else if (transform == (int)Transformation.TOP)
            {
                transform = (int)Transformation.RIGHT;
                if ((bool)b_four.IsChecked)
                {
                    FourShipsClear(sender, e);
                }
                else if ((bool)b_three.IsChecked)
                {
                    ThreeShipClear(sender, e);
                }
                else if ((bool)b_two.IsChecked)
                {
                    TwoShipClear(sender, e);
                }
            }
            else if (transform == (int)Transformation.RIGHT)
            {
                transform = (int)Transformation.BOTTOM;
                if ((bool)b_four.IsChecked)
                {
                    FourShipsClear(sender, e);
                }
                else if ((bool)b_three.IsChecked)
                {
                    ThreeShipClear(sender, e);
                }
                else if ((bool)b_two.IsChecked)
                {
                    TwoShipClear(sender, e);
                }
            }
            else if (transform == (int)Transformation.BOTTOM)
            {
                transform = (int)Transformation.LEFT;
                if ((bool)b_four.IsChecked)
                {
                    FourShipsClear(sender, e);
                }
                else if ((bool)b_three.IsChecked)
                {
                    ThreeShipClear(sender, e);
                }
                else if ((bool)b_two.IsChecked)
                {
                    TwoShipClear(sender, e);
                }
            }
        }

        private void Enemy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            bool flag = false;

            Point CurrentSender = SearchSender(sender, 2);
            begin = CurrentSender;
            int value = flag_ships_enemy[(int)CurrentSender.X + 1, (int)CurrentSender.Y + 1];

            if (value == 4 || value == 8 || value == 3)
            {
                return;
            }
            ViewCoord(CurrentSender, My);
            if (GameMode && e.LeftButton == MouseButtonState.Pressed && step)
            {
                if (value == 5)
                {
                    flag_ships_enemy[(int)CurrentSender.X + 1, (int)CurrentSender.Y + 1] = 3;
                    Append(chat, "Батька стреляет в " + My.Text + " и попадает." + Environment.NewLine, "Green", FontWeights.Normal);
                    chat.ScrollToEnd();
                    WriteShipCoord(CurrentSender, 1);
                    WriteShipCoord(CurrentSender, 2);
                    flag = true;
                    Request(CurrentSender, 1);
                }
                else if (value == 0)
                {
                    flag_ships_enemy[(int)CurrentSender.X + 1, (int)CurrentSender.Y + 1] = 4;
                    Append(chat, "Батька стреляет в " + My.Text + " и промахивается." + Environment.NewLine, "Green", FontWeights.Normal);
                    chat.ScrollToEnd();
                }
                CheckEndGame(2);
                Regeneration();
                OnRender(2);


                if (flag)
                    return;

                textBlock6.Text = "Ход противника";
                step = false;
                Request(CurrentSender, 0);
            }
        }

        private void Request(Point currentSender, int param)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);

                string message = "";
                if (param == 0)
                    message = currentSender.X + "#" + currentSender.Y + "#23h2j19";
                else
                    message = currentSender.X + "#" + currentSender.Y + "#23h2j19upd";
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);

                data = new byte[1024];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);


                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void L1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point CurrentSender = SearchSender(sender, 1);

            int value = flag_ships[(int)CurrentSender.X + 1, (int)CurrentSender.Y + 1];
            bool Check = false;

            if (!GameMode && e.LeftButton == MouseButtonState.Pressed)
            {
                if (points != null)
                {
                    foreach (Point item in points)
                    {
                        if (item.X != 0 || item.Y != 0)
                        {
                            Check = true;
                            break;
                        }
                    }
                }
                else
                    return;

                if (four > 0 && (bool)b_four.IsChecked && value != 5 && Check)
                {
                    four--;
                    ships.Add(points);
                    shipsMemory.Add((Point[])points.Clone());
                }
                else if (three > 0 && (bool)b_three.IsChecked && value != 5 && Check)
                {
                    three--;
                    ships.Add(points);
                    shipsMemory.Add((Point[])points.Clone());
                }
                else if (two > 0 && (bool)b_two.IsChecked && value != 5 && Check)
                {
                    two--;
                    ships.Add(points);
                    shipsMemory.Add((Point[])points.Clone());
                }
                else if (one > 0 && (bool)b_one.IsChecked && value != 5 && Check)
                {
                    one--;
                    ships.Add(points);
                    shipsMemory.Add((Point[])points.Clone());
                }

                foreach (Point item in points)
                {
                    flag_ships[(int)item.X, (int)item.Y] = 5;
                }
            }
            //else if (GameMode && e.LeftButton == MouseButtonState.Pressed)
            //{
            //    if (value == 5)
            //    {
            //        flag_ships[(int)CurrentSender.X + 1, (int)CurrentSender.Y + 1] = 3;
            //        WriteShipCoord(CurrentSender, 1);
            //    }
            //    else if (value == 0)
            //    {
            //        flag_ships[(int)CurrentSender.X + 1, (int)CurrentSender.Y + 1] = 4;
            //    }
            //    CheckEndGame(1);
            //}
            Regeneration();
            OnRender(1);
        }

        private void Game_run_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (four + three + two + one == 0 && connection)
                {
                    current = new DateTime(1970, 1, 1, 0, 0, 0);

                    GameMode = step = true;
                    ai = new AI();
                    ai.GetMap = flag_ships;
                    counter_time.Start();
                    game_run.IsEnabled = false;
                    MainPanel.IsEnabled = false;
                    b_four.Content = "Четырёхпалубный";
                    b_three.Content = "Трёхпалубный";
                    b_two.Content = "Двухпалубный";
                    b_one.Content = "Однопалубный";
                    textBlock6.Text = "Ваш ход";
                    OnRender(1);
                    OnRender(2);
                    Send();
                }
                else
                {
                    if (four + three + two + one > 0)
                    {
                        MessageBox.Show("Расставлены не все корабли!");
                    }
                    else if (!connection)
                    {
                        MessageBox.Show("Подключение не выполнено!");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Неизвестная ошибка!");
            }
        }

        private void L1_MouseLeave(object sender, MouseEventArgs e)
        {
            if ((bool)b_four.IsChecked && four > 0)
            {
                FourShipLeave(sender, e);
            }
            else if ((bool)b_three.IsChecked && three > 0)
            {
                ThreeShipLeave(sender, e);
            }
            else if ((bool)b_two.IsChecked && two > 0)
            {
                TwoShipLeave(sender, e);
            }
            else if ((bool)b_one.IsChecked && one > 0)
            {
                OneShipLeave(sender, e);
            }
            OnRender(1);
        }

        private void L1_MouseEnter(object sender, MouseEventArgs e)
        {
            Point CurrentSender = SearchSender(sender, 1);

            if ((bool)b_four.IsChecked && four > 0)
            {
                FourShipEnter(sender, e, out points);
            }
            else if ((bool)b_three.IsChecked && three > 0)
            {
                ThreeShipEnter(sender, e, out points);
            }
            else if ((bool)b_two.IsChecked && two > 0)
            {
                TwoShipEnter(sender, e, out points);
            }
            else if ((bool)b_one.IsChecked && one > 0)
            {
                OneShipEnter(sender, e, out points);
            }
            OnRender(1);
        }

        /// <summary>
        /// OneShips
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region OneShips
        public void OneShipEnter(object sender, MouseEventArgs e, out Point[] points)
        {
            points = new Point[1];

            if (one == 0)
                return;

            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if ((Label)sender == labels[i, j] && (bool)b_one.IsChecked && flag_ships[i + 1, j + 1] != 1 && flag_ships[i + 1, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 1;

                            points[0].X = i + 1;
                            points[0].Y = j + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                }
            }
        }

        public void OneShipLeave(object sender, MouseEventArgs e)
        {
            if (one == 0)
                return;

            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if ((Label)sender == labels[i, j] && (bool)b_one.IsChecked && flag_ships[i + 1, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// TwoShips
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region TwoShips
        public void TwoShipEnter(object sender, MouseEventArgs e, out Point[] points)
        {
            points = new Point[2];

            if (two == 0)
                return;

            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if (sender == labels[i, j] && (bool)b_two.IsChecked && transform == (int)Transformation.TOP && i - 1 >= 0 &&
                        flag_ships[i, j + 1] != 5 && flag_ships[i + 1, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i, j + 1] = 1;
                            flag_ships[i + 1, j + 1] = 1;

                            points[0].X = i;
                            points[1].X = i + 1;
                            points[1].Y = points[0].Y = j + 1;


                            //flag_ships[i - 1, j + 2] = 2;
                            //flag_ships[i - 1, j + 1] = 2;
                            //flag_ships[i - 1, j] = 2;

                            //flag_ships[i, j + 2] = 2;
                            //flag_ships[i, j] = 2;

                            //flag_ships[i + 1, j + 2] = 2;
                            //flag_ships[i + 1, j] = 2;

                            //flag_ships[i + 2, j + 2] = 2;
                            //flag_ships[i + 2, j + 1] = 2;
                            //flag_ships[i + 2, j] = 2;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_two.IsChecked && transform == (int)Transformation.RIGHT && j + 2 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j + 2] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 1;
                            flag_ships[i + 1, j + 2] = 1;

                            points[0].Y = j + 1;
                            points[1].Y = j + 2;
                            points[1].X = points[0].X = i + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_two.IsChecked && transform == (int)Transformation.BOTTOM && i + 2 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 2, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 1;
                            flag_ships[i + 2, j + 1] = 1;

                            points[0].X = i + 1;
                            points[1].X = i + 2;
                            points[1].Y = points[0].Y = j + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_two.IsChecked && transform == (int)Transformation.LEFT && j - 1 >= 0 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 1;
                            flag_ships[i + 1, j] = 1;

                            points[0].Y = j + 1;
                            points[1].Y = j;
                            points[1].X = points[0].X = i + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                }
            }
        }

        public void TwoShipLeave(object sender, MouseEventArgs e)
        {
            if (two == 0)
                return;

            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if (sender == labels[i, j] && (bool)b_two.IsChecked && transform == (int)Transformation.TOP && i - 1 >= 0 &&
                        flag_ships[i, j + 1] != 5 && flag_ships[i + 1, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i, j + 1] = 0;


                            //flag_ships[i - 1, j + 2] = 0;
                            //flag_ships[i - 1, j + 1] = 0;
                            //flag_ships[i - 1, j] = 0;

                            //flag_ships[i, j + 2] = 0;
                            //flag_ships[i, j] = 0;

                            //flag_ships[i + 1, j + 2] = 0;
                            //flag_ships[i + 1, j] = 0;

                            //flag_ships[i + 2, j + 2] = 0;
                            //flag_ships[i + 2, j + 1] = 0;
                            //flag_ships[i + 2, j] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_two.IsChecked && transform == (int)Transformation.RIGHT && j + 2 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j + 2] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j + 2] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_two.IsChecked && transform == (int)Transformation.BOTTOM && i + 2 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 2, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 2, j + 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_two.IsChecked && transform == (int)Transformation.LEFT && j - 1 >= 0 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                }
            }
        }

        public void TwoShipClear(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if (sender == labels[i, j] && transform == (int)Transformation.RIGHT && i - 1 >= 0 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i, j + 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && transform == (int)Transformation.BOTTOM && j + 2 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j + 2] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j + 2] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && transform == (int)Transformation.LEFT && i + 2 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 2, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 2, j + 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && transform == (int)Transformation.TOP && j - 1 >= 0 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    TwoShipEnter(sender, e, out points);
                }
            }
        }
        #endregion

        /// <summary>
        /// ThreeShips
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="points"></param>
        #region ThreeShips
        public void ThreeShipEnter(object sender, MouseEventArgs e, out Point[] points)
        {
            points = new Point[3];

            if (three == 0)
                return;

            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if (sender == labels[i, j] && (bool)b_three.IsChecked && transform == (int)Transformation.TOP && i - 2 >= 0 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i, j + 1] != 5 && flag_ships[i - 1, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i - 1, j + 1] = 1;
                            flag_ships[i, j + 1] = 1;
                            flag_ships[i + 1, j + 1] = 1;

                            points[0].X = i - 1;
                            points[1].X = i;
                            points[2].X = i + 1;
                            points[2].Y = points[1].Y = points[0].Y = j + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_three.IsChecked && transform == (int)Transformation.RIGHT && j + 3 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j + 2] != 5 && flag_ships[i + 1, j + 3] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 1;
                            flag_ships[i + 1, j + 2] = 1;
                            flag_ships[i + 1, j + 3] = 1;

                            points[0].Y = j + 1;
                            points[1].Y = j + 2;
                            points[2].Y = j + 3;
                            points[2].X = points[1].X = points[0].X = i + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_three.IsChecked && transform == (int)Transformation.BOTTOM && i + 3 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 2, j + 1] != 5 && flag_ships[i + 3, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 1;
                            flag_ships[i + 2, j + 1] = 1;
                            flag_ships[i + 3, j + 1] = 1;

                            points[0].X = i + 1;
                            points[1].X = i + 2;
                            points[2].X = i + 3;
                            points[2].Y = points[1].Y = points[0].Y = j + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_three.IsChecked && transform == (int)Transformation.LEFT && j - 2 >= 0 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j] != 5 && flag_ships[i + 1, j - 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 1;
                            flag_ships[i + 1, j] = 1;
                            flag_ships[i + 1, j - 1] = 1;

                            points[0].Y = j + 1;
                            points[1].Y = j;
                            points[2].Y = j - 1;
                            points[2].X = points[1].X = points[0].X = i + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                }
            }
        }

        public void ThreeShipLeave(object sender, MouseEventArgs e)
        {
            if (three == 0)
                return;

            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if (sender == labels[i, j] && (bool)b_three.IsChecked && transform == (int)Transformation.TOP && i - 2 >= 0 &&
                         flag_ships[i + 1, j + 1] != 5 && flag_ships[i, j + 1] != 5 && flag_ships[i - 1, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i, j + 1] = 0;
                            flag_ships[i - 1, j + 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_three.IsChecked && transform == (int)Transformation.RIGHT && j + 3 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j + 2] != 5 && flag_ships[i + 1, j + 3] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j + 2] = 0;
                            flag_ships[i + 1, j + 3] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_three.IsChecked && transform == (int)Transformation.BOTTOM && i + 3 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 2, j + 1] != 5 && flag_ships[i + 3, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 2, j + 1] = 0;
                            flag_ships[i + 3, j + 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_three.IsChecked && transform == (int)Transformation.LEFT && j - 2 >= 0 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j] != 5 && flag_ships[i + 1, j - 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j] = 0;
                            flag_ships[i + 1, j - 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                }
            }
        }

        public void ThreeShipClear(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if (sender == labels[i, j] && transform == (int)Transformation.RIGHT && i - 2 >= 0 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i, j + 1] != 5 && flag_ships[i - 1, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i, j + 1] = 0;
                            flag_ships[i - 1, j + 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && transform == (int)Transformation.BOTTOM && j + 3 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j + 2] != 5 && flag_ships[i + 1, j + 3] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j + 2] = 0;
                            flag_ships[i + 1, j + 3] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && transform == (int)Transformation.LEFT && i + 3 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 2, j + 1] != 5 && flag_ships[i + 3, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 2, j + 1] = 0;
                            flag_ships[i + 3, j + 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && transform == (int)Transformation.TOP && j - 2 >= 0 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j] != 5 && flag_ships[i + 1, j - 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j] = 0;
                            flag_ships[i + 1, j - 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    ThreeShipEnter(sender, e, out points);
                }
            }
        }

        #endregion

        /// <summary>
        /// FourShips
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="points"></param>
        #region FourShips
        public void FourShipEnter(object sender, MouseEventArgs e, out Point[] points)
        {
            points = new Point[4];

            if (four == 0)
                return;

            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if (sender == labels[i, j] && (bool)b_four.IsChecked && transform == (int)Transformation.TOP && i - 3 >= 0 &&
                        flag_ships[i - 2, j + 1] != 5 && flag_ships[i - 1, j + 1] != 5 && flag_ships[i, j + 1] != 5 && flag_ships[i + 1, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i - 2, j + 1] = 1;
                            flag_ships[i - 1, j + 1] = 1;
                            flag_ships[i, j + 1] = 1;
                            flag_ships[i + 1, j + 1] = 1;

                            points[0].X = i - 2;
                            points[1].X = i - 1;
                            points[2].X = i;
                            points[3].X = i + 1;
                            points[3].Y = points[2].Y = points[1].Y = points[0].Y = j + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_four.IsChecked && transform == (int)Transformation.RIGHT && j + 4 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j + 2] != 5 && flag_ships[i + 1, j + 3] != 5 && flag_ships[i + 1, j + 4] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 1;
                            flag_ships[i + 1, j + 2] = 1;
                            flag_ships[i + 1, j + 3] = 1;
                            flag_ships[i + 1, j + 4] = 1;

                            points[0].Y = j + 1;
                            points[1].Y = j + 2;
                            points[2].Y = j + 3;
                            points[3].Y = j + 4;
                            points[3].X = points[2].X = points[1].X = points[0].X = i + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_four.IsChecked && transform == (int)Transformation.BOTTOM && i + 4 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 2, j + 1] != 5 && flag_ships[i + 3, j + 1] != 5 && flag_ships[i + 4, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 1;
                            flag_ships[i + 2, j + 1] = 1;
                            flag_ships[i + 3, j + 1] = 1;
                            flag_ships[i + 4, j + 1] = 1;

                            points[0].X = i + 1;
                            points[1].X = i + 2;
                            points[2].X = i + 3;
                            points[3].X = i + 4;
                            points[3].Y = points[2].Y = points[1].Y = points[0].Y = j + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_four.IsChecked && transform == (int)Transformation.LEFT && j - 3 >= 0 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j] != 5 && flag_ships[i + 1, j - 1] != 5 && flag_ships[i + 1, j - 2] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 1;
                            flag_ships[i + 1, j] = 1;
                            flag_ships[i + 1, j - 1] = 1;
                            flag_ships[i + 1, j - 2] = 1;

                            points[0].Y = j + 1;
                            points[1].Y = j;
                            points[2].Y = j - 1;
                            points[3].Y = j - 2;
                            points[3].X = points[2].X = points[1].X = points[0].X = i + 1;
                        }
                        catch { }
                        OnRender(1);
                    }
                }
            }
        }

        public void FourShipLeave(object sender, MouseEventArgs e)
        {
            if (four == 0)
                return;

            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if (sender == labels[i, j] && (bool)b_four.IsChecked && transform == (int)Transformation.TOP && i - 3 >= 0 &&
                        flag_ships[i - 2, j + 1] != 5 && flag_ships[i - 1, j + 1] != 5 && flag_ships[i, j + 1] != 5 && flag_ships[i + 1, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i, j + 1] = 0;
                            flag_ships[i - 1, j + 1] = 0;
                            flag_ships[i - 2, j + 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_four.IsChecked && transform == (int)Transformation.RIGHT && j + 4 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j + 2] != 5 && flag_ships[i + 1, j + 3] != 5 && flag_ships[i + 1, j + 4] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j + 2] = 0;
                            flag_ships[i + 1, j + 3] = 0;
                            flag_ships[i + 1, j + 4] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_four.IsChecked && transform == (int)Transformation.BOTTOM && i + 4 <= N - 2 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 2, j + 1] != 5 && flag_ships[i + 3, j + 1] != 5 && flag_ships[i + 4, j + 1] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 2, j + 1] = 0;
                            flag_ships[i + 3, j + 1] = 0;
                            flag_ships[i + 4, j + 1] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                    else if (sender == labels[i, j] && (bool)b_four.IsChecked && transform == (int)Transformation.LEFT && j - 3 >= 0 &&
                        flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j] != 5 && flag_ships[i + 1, j - 1] != 5 && flag_ships[i + 1, j - 2] != 5)
                    {
                        try
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j] = 0;
                            flag_ships[i + 1, j - 1] = 0;
                            flag_ships[i + 1, j - 2] = 0;
                        }
                        catch { }
                        OnRender(1);
                    }
                }
            }
        }

        private void FourShipsClear(object sender, MouseEventArgs e)
        {
            try
            {
                for (int i = 0; i < N - 2; i++)
                {
                    for (int j = 0; j < N - 2; j++)
                    {
                        if (sender == labels[i, j] && transform == (int)Transformation.RIGHT && i - 3 >= 0 &&
                            flag_ships[i + 1, j + 1] != 5 && flag_ships[i, j + 1] != 5 && flag_ships[i - 1, j + 1] != 5 && flag_ships[i - 2, j + 1] != 5)
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i, j + 1] = 0;
                            flag_ships[i - 1, j + 1] = 0;
                            flag_ships[i - 2, j + 1] = 0;
                            OnRender(1);
                        }
                        else if (sender == labels[i, j] && transform == (int)Transformation.BOTTOM && i + 4 <= N - 2 &&
                            flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j + 2] != 5 && flag_ships[i + 1, j + 3] != 5 && flag_ships[i + 1, j + 4] != 5)
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j + 2] = 0;
                            flag_ships[i + 1, j + 3] = 0;
                            flag_ships[i + 1, j + 4] = 0;
                            OnRender(1);
                        }
                        else if (sender == labels[i, j] && transform == (int)Transformation.LEFT && i + 4 <= N - 2 &&
                            flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 2, j + 1] != 5 && flag_ships[i + 3, j + 1] != 5 && flag_ships[i + 4, j + 1] != 5)
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 2, j + 1] = 0;
                            flag_ships[i + 3, j + 1] = 0;
                            flag_ships[i + 4, j + 1] = 0;
                            OnRender(1);
                        }
                        else if (sender == labels[i, j] && transform == (int)Transformation.TOP && j - 3 >= 0 &&
                            flag_ships[i + 1, j + 1] != 5 && flag_ships[i + 1, j] != 5 && flag_ships[i + 1, j - 1] != 5 && flag_ships[i + 1, j - 2] != 5)
                        {
                            flag_ships[i + 1, j + 1] = 0;
                            flag_ships[i + 1, j] = 0;
                            flag_ships[i + 1, j - 1] = 0;
                            flag_ships[i + 1, j - 2] = 0;
                            OnRender(1);
                        }
                        FourShipEnter(sender, e, out points);
                    }
                }
            }
            catch { }
        }
        #endregion

        /// <summary>
        ///  Rendering area of labels
        /// </summary>
        private void OnRender(int param)
        {
            Label[,] label = param == 1 ? labels : enemy;
            int[,] flag = param == 1 ? flag_ships : flag_ships_enemy;

            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if (flag[i + 1, j + 1] == 1 || flag[i + 1, j + 1] == 5 && !GameMode && param == 1)
                    {
                        label[i, j].Background = Brushes.White;
                        label[i, j].BorderThickness = new Thickness(5);
                    }
                    else if (flag[i + 1, j + 1] == 0)
                    {
                        label[i, j].Background = Brushes.White;
                        label[i, j].BorderThickness = new Thickness(1);
                    }
                    else if (flag[i + 1, j + 1] == 2)
                    {
                        label[i, j].Background = Brushes.Green;
                        label[i, j].BorderThickness = new Thickness(1);
                    }
                    else if (flag[i + 1, j + 1] == 3)
                    {
                        label[i, j].Background = new ImageBrush(Dead);
                        label[i, j].BorderThickness = new Thickness(5);
                    }
                    else if (flag[i + 1, j + 1] == 4)
                    {
                        label[i, j].Background = new ImageBrush(Strike);
                        label[i, j].BorderThickness = new Thickness(1);
                    }
                    else if (flag[i + 1, j + 1] == 8)
                    {
                        label[i, j].Background = new ImageBrush(Dead);
                        label[i, j].BorderThickness = new Thickness(5);
                    }
                }
            }

            b_four.Content = "Четырёхпалубный (" + four + ")";
            b_three.Content = "Трёхпалубный (" + three + ")";
            b_two.Content = "Двухпалубный (" + two + ")";
            b_one.Content = "Однопалубный (" + one + ")";

            if (GameMode)
                count.Text = memory_pos_e.Count.ToString() + " : " + memory_pos.Count.ToString();

            Regeneration();
        }

        public void Regeneration()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i == 0 || j == 0 || i == N - 1 || j == N - 1)
                        flag_ships[i, j] = 9;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Drawing.Bitmap br = Properties.Resources.strike;

                Strike =
                    Imaging.CreateBitmapSourceFromHBitmap(
                           br.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());


                System.Drawing.Bitmap br2 = Properties.Resources.dead;

                Dead =
                    Imaging.CreateBitmapSourceFromHBitmap(
                           br2.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());

                for (int i = 0; i < 400; i += 40)
                {
                    for (int j = 0; j < 400; j += 40)
                    {
                        Label l1 = new Label
                        {

                            Margin = new Thickness(j + 50, i + 50, 0, 0),
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                            VerticalAlignment = System.Windows.VerticalAlignment.Top,
                            Width = 40,
                            Height = 40,
                            FontSize = 8,
                            FontWeight = FontWeights.Bold,
                            Background = Brushes.White,
                            BorderBrush = new SolidColorBrush(Colors.Blue),
                            BorderThickness = new Thickness(1)
                        };

                        l1.MouseRightButtonDown += L1_MouseRightButtonDown;
                        l1.MouseDown += L1_MouseDown;
                        l1.MouseEnter += L1_MouseEnter;
                        l1.MouseLeave += L1_MouseLeave;
                        labels[i / 40, j / 40] = l1;
                        Grid.SetColumn(l1, 0);
                        grid.Children.Add(l1);
                    }
                }

                char a = 'А';
                for (int i = 0; i < 400; i += 40)
                {
                    Label l1 = new Label
                    {
                        Margin = new Thickness(i + 58, 10, 0, 0),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        Width = 40,
                        Height = 40,
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Background = Brushes.Transparent,
                        Content = a.ToString()
                    };
                    Grid.SetColumn(l1, 0);
                    grid.Children.Add(l1);
                    a++;

                    if (a == 'Й')
                    {
                        a++;
                    }

                    Label l2 = new Label
                    {
                        Margin = new Thickness(10, i + 50, 0, 0),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        Width = 40,
                        Height = 40,
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Background = Brushes.Transparent,
                        Content = i / 40 + 1
                    };
                    Grid.SetColumn(l2, 0);
                    grid.Children.Add(l2);
                }

                a = 'А';

                for (int i = 0; i < 400; i += 40)
                {
                    Label l1 = new Label
                    {
                        Margin = new Thickness(i + 58, 10, 0, 0),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        Width = 40,
                        Height = 40,
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Background = Brushes.Transparent,
                        Content = a.ToString()
                    };
                    Grid.SetColumn(l1, 2);
                    grid.Children.Add(l1);
                    a++;

                    if (a == 'Й')
                    {
                        a++;
                    }

                    Label l2 = new Label
                    {
                        Margin = new Thickness(10, i + 50, 0, 0),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        Width = 40,
                        Height = 40,
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Background = Brushes.Transparent,
                        Content = i / 40 + 1
                    };
                    Grid.SetColumn(l2, 2);
                    grid.Children.Add(l2);
                }

                for (int i = 0; i < 400; i += 40)
                {
                    for (int j = 0; j < 400; j += 40)
                    {
                        Label l1 = new Label
                        {
                            Margin = new Thickness(j + 50, i + 50, 0, 0),
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                            VerticalAlignment = System.Windows.VerticalAlignment.Top,
                            Width = 40,
                            Height = 40,
                            FontSize = 8,
                            FontWeight = FontWeights.Bold,
                            Background = Brushes.White,
                            BorderBrush = new SolidColorBrush(Colors.Blue),
                            BorderThickness = new Thickness(1)
                        };

                        l1.MouseDown += Enemy_MouseDown;
                        enemy[i / 40, j / 40] = l1;
                        Grid.SetColumn(l1, 2);
                        grid.Children.Add(l1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RandomMap(object sender, RoutedEventArgs e)
        {
            MapGenerator mg = new MapGenerator();
            mg.Generate();
            flag_ships = mg.GetMap;
            ships = mg.GetCoord;
            for (int i = 0; i < ships.Count; i++)
            {
                shipsMemory.Add((Point[])ships[i].Clone());
            }
            mg = null;

            four = three = two = one = 0;
            MainPanel.IsEnabled = false;

            OnRender(1);
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);

                Append(chat, "\nВы: " + message_t.Text, "Black", FontWeights.Light);
                string message = message_t.Text + "#msg#23h2j7";
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);

                data = new byte[1024];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                message_t.Text = string.Empty;
            }
        }

        private void Send()
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);

                string message = Zipping(flag_ships, ships) + "#23h2j3";
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);

                data = new byte[1024];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);

                string[] response = builder.ToString().Split('#');
                if (response[2].Equals("23h2j4"))
                {
                    Unzipping(response[0], response[1]);
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                else if (response[2].Equals("23h2j9"))
                {
                    currentPoint = new Point(Convert.ToInt32(response[0]), Convert.ToInt32(response[1]));
                    Dispatcher.Invoke(() => StepEnemy?.Invoke());

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                else
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void mouse_ClickUp(object sender, MouseButtonEventArgs e)
        {
            message_t.Text = String.Empty;
        }

        private void mouse_ClickUpConnect(object sender, MouseButtonEventArgs e)
        {
            connect.Text = String.Empty;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void StartServer(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            try
            {
                button.IsEnabled = false;
                string[] temp = connect.Text.Split(':');
                address = temp[0];
                port = int.Parse(temp[1]);
                listen = new Thread(Listen);
                listen.Start();
                listen.IsBackground = true;
                connection = true;
                currentIp.IsEnabled = false;

                connect.Text = String.Concat(address, ":", port);
                IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
                mappings.Add(port, "TCP", port, currentIp.Text, true, "Sea Battle S Open Port");
                MessageBox.Show("Сервер запущен");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
                button.IsEnabled = true;
            }
        }

        private void Listen()
        {
            IPEndPoint ipPoint = null;
            Dispatcher.Invoke(() => ipPoint = new IPEndPoint(IPAddress.Parse(currentIp.Text), port));

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);

                while (true)
                {
                    Socket handler = listenSocket.Accept();

                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    byte[] data = new byte[2048];

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    string[] response = builder.ToString().Split('#');
                    string message = "ok";

                    if (response[2].Equals("23h2j9"))
                    {
                        currentPoint = new Point(Convert.ToInt32(response[0]), Convert.ToInt32(response[1]));
                        Dispatcher.Invoke(() =>
                        {
                            step = false;
                            StepEnemy?.Invoke();
                        });
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    else if (response[2].Equals("23h2j9upd"))
                    {
                        currentPoint = new Point(Convert.ToInt32(response[0]), Convert.ToInt32(response[1]));
                        Dispatcher.Invoke(() =>
                        {
                            flag_ships[(int)currentPoint.X + 1, (int)currentPoint.Y + 1] = 3;
                            WriteShipCoord(currentPoint, 1);
                            WriteShipCoord(currentPoint, 2);
                            CheckEndGame(2);
                            Regeneration();
                            OnRender(1);
                            OnRender(2);
                        });

                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    else if (response[1].Equals("msg") && response[2].Equals("23h2j0"))
                    {
                        Dispatcher.Invoke(() => Append(chat, "\nПротивник: " + response[0], "Black", FontWeights.Light));
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    else if (response[1].Equals("2") && response[2].Equals("test"))
                    {
                        MessageBox.Show(response[0]);
                        data = Encoding.Unicode.GetBytes("Подключение выполнено 2");
                        handler.Send(data);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                    else
                    {
                        MessageBox.Show(builder.ToString() + "#S");
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Thread.Sleep(5000);
            }
        }

        public Point SearchSender(object sender, int param)
        {
            Label[,] label = param == 1 ? labels : enemy;

            for (int i = 0; i < N - 2; i++)
            {
                for (int j = 0; j < N - 2; j++)
                {
                    if ((Label)sender == label[i, j])
                    {
                        return new Point(i, j);
                    }
                }
            }
            return new Point(0, 0);
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            if (listen.IsAlive)
            {
                try
                {
                    listen.Join(500);
                    listen = null;
                    run.IsEnabled = true;
                    connection = false;
                    NATUPNPLib.IStaticPortMappingCollection mappings = upnpnat.StaticPortMappingCollection;
                    mappings.Remove(port, "TCP");
                    MessageBox.Show("Сервер остановлен");
                }
                catch (ThreadAbortException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Сервер не запущен.");
            }
        }

        public void WriteShipCoord(Point CurrentSender, int param)
        {
            List<Point[]> s_list = param == 1 ? ships : ships_e;
            List<Point[]> sm_list = param == 1 ? shipsMemory : shipsMemory_e;
            int[,] flag = param == 1 ? flag_ships : flag_ships_enemy;
            for (int i = 0; i < s_list.Count; i++)
            {
                for (int j = 0; j < s_list[i].Length; j++)
                {
                    if (s_list[i][j].X == CurrentSender.X + 1 && s_list[i][j].Y == CurrentSender.Y + 1)
                    {
                        s_list[i][j].X = s_list[i][j].Y = 0;
                    }
                }
            }

            bool fall = true;
            int k = 0;

            foreach (Point[] item in s_list)
            {
                foreach (Point ship in item)
                {
                    if (ship.X != 0 && ship.Y != 0)
                    {
                        fall = false;
                        break;
                    }
                }

                if (fall)
                {
                    try
                    {
                        foreach (Point ship in sm_list[k])
                        {
                            flag[(int)ship.X - 1, (int)ship.Y - 1] = 4;
                            flag[(int)ship.X - 1, (int)ship.Y] = 4;
                            flag[(int)ship.X - 1, (int)ship.Y + 1] = 4;

                            flag[(int)ship.X, (int)ship.Y - 1] = 4;
                            flag[(int)ship.X, (int)ship.Y + 1] = 4;

                            flag[(int)ship.X + 1, (int)ship.Y - 1] = 4;
                            flag[(int)ship.X + 1, (int)ship.Y] = 4;
                            flag[(int)ship.X + 1, (int)ship.Y + 1] = 4;
                        }
                        foreach (Point ship in sm_list[k])
                        {
                            flag[(int)ship.X, (int)ship.Y] = 8;
                        }

                        switch (sm_list[k].Length)
                        {
                            case 1:
                                {
                                    if (param != 1 && !memory_pos.Contains<int>(k))
                                    {
                                        Append(chat, "Батька уничтожил однопалубный." + Environment.NewLine, "Green", FontWeights.Bold);
                                        memory_pos.Add(k);
                                    }
                                    else if (param == 1 && !memory_pos_e.Contains<int>(k))
                                    {
                                        Append(chat, "Компьютер уничтожил однопалубный." + Environment.NewLine, "Red", FontWeights.Bold);
                                        ai.Dead = false;
                                        memory_pos_e.Add(k);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (param != 1 && !memory_pos.Contains<int>(k))
                                    {
                                        Append(chat, "Батька уничтожил двухпалубный." + Environment.NewLine, "Green", FontWeights.Bold);
                                        memory_pos.Add(k);
                                    }
                                    else if (param == 1 && !memory_pos_e.Contains<int>(k))
                                    {
                                        Append(chat, "Компьютер уничтожил двухпалубный." + Environment.NewLine, "Red", FontWeights.Bold);
                                        ai.Dead = false;
                                        memory_pos_e.Add(k);
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (param != 1 && !memory_pos.Contains<int>(k))
                                    {
                                        Append(chat, "Батька уничтожил трёхпалубный." + Environment.NewLine, "Green", FontWeights.Bold);
                                        memory_pos.Add(k);
                                    }
                                    else if (param == 1 && !memory_pos_e.Contains<int>(k))
                                    {
                                        Append(chat, "Компьютер уничтожил трёхпалубный." + Environment.NewLine, "Red", FontWeights.Bold);
                                        ai.Dead = false;
                                        memory_pos_e.Add(k);
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    if (param != 1 && !memory_pos.Contains<int>(k))
                                    {
                                        Append(chat, "Батька уничтожил четырёхпалубный." + Environment.NewLine, "Green", FontWeights.Bold);
                                        memory_pos.Add(k);
                                    }
                                    else if (param == 1 && !memory_pos_e.Contains<int>(k))
                                    {
                                        Append(chat, "Компьютер уничтожил четырёхпалубный." + Environment.NewLine, "Red", FontWeights.Bold);
                                        ai.Dead = false;
                                        memory_pos_e.Add(k);
                                    }
                                    break;
                                }
                            default:
                                throw new Exception();
                        }
                    }
                    catch { }
                }
                fall = true;
                k++;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string host = Dns.GetHostName();
            currentIp.Text = Dns.GetHostEntry(host).AddressList[0].ToString();
        }

        private void bot_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("AI в разработке");
        }

        public void CheckEndGame(int param)
        {
            int[,] flag = param == 1 ? flag_ships : flag_ships_enemy;

            bool end = true;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (flag[i, j] == 5)
                    {
                        end = false;
                        break;
                    }
                }
            }

            if (end)
            {
                GameMode = false;
                if (param == 1)
                {
                    textBlock6.Text = "Вы проиграли!";
                    Append(chat, textBlock6.Text, "Red", FontWeights.Heavy);
                }
                else
                {
                    textBlock6.Text = "Вы выиграли!";
                    Append(chat, textBlock6.Text, "Green", FontWeights.Heavy);
                }
                counter_time.Stop();
                game_run.IsEnabled = true;
                MainPanel.IsEnabled = true;
                return;
            }
        }

        public void Append(RichTextBox box, string text, string color, FontWeight font)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);
            tr.Text = text;
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                    bc.ConvertFromString(color));
                tr.ApplyPropertyValue(TextElement.FontWeightProperty,
                    font);
            }
            catch (FormatException) { }
        }

        public string Zipping(int[,] array, List<Point[]> list)
        {
            string temp = "";
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    temp += array[i, j] + " ";
                }
            }

            temp += "#";

            foreach (Point[] item in list)
            {
                foreach (Point point in item)
                {
                    temp += point.X + ":" + point.Y + "$";
                }
                temp += "%";
            }
            return temp;
        }

        public void Unzipping(string map, string coord)
        {
            string[] main = map.Split('#');
            string[] s_map = main[0].Split(' ');

            int k = 0;
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    flag_ships_enemy[i, j] = int.Parse(s_map[k]);
                    k++;
                }
            }

            main = coord.Split(new char[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in main)
            {
                string[] point = item.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries);
                Point[] temp = new Point[point.Length];
                int i = 0;
                foreach (var pt in point)
                {
                    string[] xy = pt.Split(':');
                    temp[i] = new Point(Convert.ToInt32(xy[0]), Convert.ToInt32(xy[1]));
                    i++;
                }
                ships_e.Add(temp);
            }

            for (int i = 0; i < ships_e.Count; i++)
            {
                shipsMemory_e.Add((Point[])ships_e[i].Clone());
            }
        }
    }
}
