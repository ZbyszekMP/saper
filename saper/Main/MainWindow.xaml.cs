using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.Xml;
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
using System.Windows.Threading;
using System.Timers;


namespace saper
{

    public partial class MainWindow : Window
    {



        public int BoardSize = 7;
        public int Bomb = 5;
        public int Flags1 = 0;
        public int[,] ABoard;
        public Button[,] Buttons;
        public Style emptyStyle;
        public Style bombStyle;
        public Style flagStyle;
        public Style styl1;
        
        public int licznik = 1;
        public int czas = 0;
        public int odkryte = 0;

        System.Windows.Threading.DispatcherTimer Timer1 = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()

        {
            InitializeComponent();

            //aaa.Content = "ddd";

            styl1 = this.FindResource("But1") as Style;
            emptyStyle = this.FindResource("Img1") as Style;
            bombStyle = this.FindResource("Img2") as Style;
            flagStyle = this.FindResource("Img3") as Style;
        
            Timer1 = new System.Windows.Threading.DispatcherTimer();
            Timer1.Start();
            Timer1.Interval = new TimeSpan(0, 0, 1);

            RenderBoard(BoardSize);
            
            



        }

        public Vector2 CorName(string name)
        {

            Vector2 cor = new Vector2();
            string sx = "";
            string sy = "";
            for (int i = 1; i < name.Length; i++)
            {
                if (sy == "")
                {
                    if (name[i] != 'b')
                    {
                        sx += name[i];
                    }
                    else
                    {
                        i++;
                        sy += name[i];
                    }
                }
                else
                {
                    sy += name[i];
                }

            }
            cor.X = Convert.ToInt32(sx);
            cor.Y = Convert.ToInt32(sy);

            return cor;
        }
        public void click(object sender, RoutedEventArgs e)
        {
            Button b = e.Source as Button;
            
            String name = b.Name;
            Vector2 cor = CorName(name);
            Timer1.Tick -= Licznik;
            Timer1.Tick += Licznik;
            Checking((int)cor.X, (int)cor.Y);
           
           
            


            WinCheck();
                
        }
        public void Licznik(object sender, EventArgs e)
        {
            czas++;
            TimeLabel.Content = czas.ToString();
        }
        public void ClickRight(object sender, MouseButtonEventArgs e)
        {
            Button b = sender as Button;
            Image img = new Image();

            String name = b.Name;
            Vector2 cor = CorName(name);
            int x = (int)cor.X;
            int y = (int)cor.Y;
            if (ABoard[x, y] < 100)
            {
                ABoard[x, y] += 100;
                img.Style = flagStyle;
                Flags1--;
                b.Click -= new RoutedEventHandler(click);
            }
            else
            {
                ABoard[x, y] -= 100;
                Flags1++;
                img.Style = emptyStyle;
                b.Click += new RoutedEventHandler(click);
            }
         //   FlagsLabel1.Content=Flags1.ToString();
            b.Content = img;

        }

        public void RenderBoard(int Size)
        {
            int bomb = Bomb;
            
            Button pole;
            Image img;
            GridLength Wh = new GridLength(2, GridUnitType.Star);
            Buttons = new Button[Size, Size];
            ABoard = new int[Size, Size];
            Flags1 = bomb;
           // FlagsLabel1.Content = Flags1.ToString();
            //Generowanie planszy
            for (int i = 0; i < BoardSize; i++)
            {

                Board.RowDefinitions.Add(new RowDefinition() { Height = Wh });
                Board.ColumnDefinitions.Add(new ColumnDefinition { Width = Wh });




                for (int j = 0; j < BoardSize; j++)
                {
                    pole = new Button();
                    img = new Image();

                    //img.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath("Assests/Images/empty.png"), UriKind.Absolute));
                    img.Style = emptyStyle;
                    pole.Content = img;
                    pole.Click += new RoutedEventHandler(click);
                    pole.MouseRightButtonDown += new MouseButtonEventHandler(ClickRight);
                    pole.MouseEnter += (new MouseEventHandler(MEnter));
                    pole.Name = "b" + i.ToString() + "b" + j.ToString();
                   
                    Grid.SetColumn(pole, j);
                    Grid.SetRow(pole, i);
                    pole.Style = styl1;
                    Board.Children.Add(pole);
                    Buttons[i, j] = Board.Children[Board.Children.Count - 1] as Button;

                }

            }


            //Losownaie bomb
            
            while (bomb > 0)
            {
                Random rand = new Random();
                int x = rand.Next(0, Size);
                int y = rand.Next(0, Size);

                if (ABoard[x, y] != 9)
                {
                    ABoard[x, y] = 9;
                    bomb--;
                }
            }

            //Nadawanie wartosci polom
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    int value = 0;
                    if (ABoard[i, j] != 9)
                    {
                        for (int a = 0; a < 3; a++)
                        {
                            for (int b = 0; b < 3; b++)
                            {
                                if ((i - 1) + a >= 0 && (j - 1) + b >= 0 && (i - 1) + a < Size && (j - 1) + b < Size)
                                {
                                    if (ABoard[(i - 1) + a, (j - 1) + b] == 9)
                                    {
                                        value++;
                                    }
                                }
                            }
                        }
                        ABoard[i, j] = value;
                    }
                }
            }

           

        }

        public void Checking(int x, int y)
        {
            if (ABoard[x, y] != 9)
            {
                if (ABoard[x, y] != 10 && ABoard[x, y] < 100)
                {
                    odkryte++;
                    Buttons[x, y].Click -= new RoutedEventHandler(click);
                    Buttons[x, y].MouseRightButtonDown -= new MouseButtonEventHandler(ClickRight);
                    Buttons[x, y].Content = ABoard[x, y];
                    ABoard[x, y] += 100;
                    if (ABoard[x, y] == 100)
                        Buttons[x, y].Content = "";
                    if (ABoard[x, y] == 100)
                    {
                       
                        for (int a = 0; a < 3; a++)
                        {
                            for (int b = 0; b < 3; b++)
                            {
                                if ((x - 1) + a >= 0 && (y - 1) + b >= 0 && (x - 1) + a < BoardSize && (y - 1) + b < BoardSize)
                                {
                                    
                                    
                                    Checking((x - 1) + a, (y - 1) + b);
                                    
                                  
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Image img = new Image();
                Timer1.Tick -= Licznik;
                
                Board.IsEnabled = false;

                for (int i = 0; i < BoardSize; i++) {
                    for (int j = 0; j < BoardSize; j++)
                    {
                        if (ABoard[i, j] == 9)
                        {
                            img = new Image();
                            img.Style = bombStyle;
                            Buttons[i, j].Content = img;
                        }
                    }
                }
                LoseLabel1.Visibility= Visibility.Visible;


            }


        }
        public void WinCheck()
        {
            if (odkryte == (BoardSize * BoardSize) - Bomb)
            {
                
                Timer1.Tick -= Licznik;               
                Board.IsEnabled = false;
                ScoreLabel.Content="Wynik:"+czas.ToString();
                WinLabels.Visibility=Visibility.Visible;
                

            }
        }


        private void MEnter(object sender, MouseEventArgs e)
        {
            Button b = sender as Button; if (b != null)
            {
                Vector2 cor = CorName(b.Name);
                //Cords.Content = (cor.X+1).ToString() + "|" + (cor.Y+1).ToString();
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Board.Children.Clear();
            Board.RowDefinitions.Clear();
            Board.ColumnDefinitions.Clear();


            
            RenderBoard(BoardSize);
            LoseLabel1.Visibility = Visibility.Hidden;
            WinLabels.Visibility = Visibility.Hidden;
            czas = 0;
            odkryte = 0;
            Board.IsEnabled = true;
            TimeLabel.Content = 0;
            Timer1.Tick -= Licznik;
           

        }
    }
}
