using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace WpfApp2
{
    public enum Picture
    {
        lingdang = 1,
        maoqiu = 2,
        naiping = 3,
        yumi = 4,
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void Workover();
        public event Workover Complet;
        public ButtonContainer container = new ButtonContainer();
        public YangButtonList tablebuttons = new YangButtonList();
        public class YangButtonList
        {
            public List<YangButton> tablebuttons = new List<YangButton>();
            public void Add(YangButton button)
            {
                int zindex = Grid.GetZIndex((Button)(button));
                foreach (YangButton s in tablebuttons)
                {
                    if (zindex > Grid.GetZIndex((Button)(s)) && s.IsCoveredCheck(button))
                    {
                        ((Button)(s)).IsEnabled = false;
                    }
                }
                tablebuttons.Add(button);

            }
            public YangButton? Find(Button button)
            {

                foreach (YangButton s in tablebuttons)
                {
                    if (button == (Button)s)
                    {
                        return s;
                    }
                }
                return null;
            }
            public void Remove(YangButton s)
            {
                List<YangButton> buttons = new List<YangButton>();
                int zindex = Grid.GetZIndex((Button)(s));
                foreach (YangButton w in tablebuttons)
                {
                    if (s == w)
                    {
                        continue;
                    }
                    if (w.IsCoveredCheck(s) && zindex > Grid.GetZIndex((Button)(w)))
                    {
                        buttons.Add(w);
                        ((Button)w).IsEnabled = true;
                    }
                }
                foreach (YangButton b in buttons)
                {
                    foreach (YangButton c in tablebuttons)
                    {
                        if (c == b || c == s || s == b)
                        {
                            continue;
                        }
                        else
                        {
                            if (Grid.GetZIndex((Button)(c)) > Grid.GetZIndex((Button)(b)) && c.IsCoveredCheck(b))
                            {

                                ((Button)b).IsEnabled = false;
                            }
                        }
                    }
                }


                ((Button)(s)).IsEnabled = false;
                tablebuttons.Remove(s);
            }
        }
        public void Add(YangButton yang)
        {
            myGrid.Children.Add((Button)yang);
        }
        public class YangButton
        {
            public const double ButtonWidth = 50;
            public const double ButtonHeight = 50;
            Button button;
            public Picture picture;
            static Dictionary<Picture, String> pictured;
            static YangButton()
            {

                pictured = new Dictionary<Picture, String>();
                pictured.Add(Picture.lingdang, "image\\lingdang.PNG");
                pictured.Add(Picture.maoqiu, "image\\maoqiu.PNG");
                pictured.Add(Picture.yumi, "image\\yumi.PNG");
                pictured.Add(Picture.naiping, "image\\naiping.PNG");
            }
            public YangButton(double x, double y, int index, MainWindow fatherwindow, bool isenabled, Picture picture)
            {
                this.picture = picture;
                Button moveButton = new Button();
                moveButton.Name = "moveButton";
                moveButton.Style = (Style)fatherwindow.FindResource("CustomButtonStyle");
                moveButton.Click += fatherwindow.moveButton_Click;
                moveButton.Margin = new Thickness(x, y, 0, 0);
                moveButton.HorizontalAlignment = HorizontalAlignment.Left;
                moveButton.Width = ButtonWidth;
                moveButton.Height = ButtonHeight;
                moveButton.VerticalAlignment = VerticalAlignment.Top;
                moveButton.IsEnabled = isenabled;
                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, pictured[picture])));
                moveButton.Background = imageBrush;
                DropShadowEffect shadowEffect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 320,
                    ShadowDepth = 5,
                    Opacity = 0.5,
                    BlurRadius = 5

                };
                moveButton.Effect = shadowEffect;
                Grid.SetZIndex(moveButton, index);
                this.button = moveButton;
            }
            public static explicit operator Button(YangButton yangButton)
            {
                return yangButton.button;
            }
            public bool IsCoveredCheck(double x, double y)
            {
                //Console.Write($"比较位于{x},{y}与{this.button.Margin.Left},{this.button.Margin.Top}");
                Thickness margin = this.button.Margin;
                double deltax = Math.Abs(x - margin.Left);
                double deltay = Math.Abs(y - margin.Top);
                if (deltax < ButtonWidth && deltay < ButtonHeight)
                {
                    Console.WriteLine("返回为true");
                    return true;
                }
                else
                {
                    Console.WriteLine("返回为false");
                    return false;
                }
            }
            public bool IsCoveredCheck(YangButton button) => IsCoveredCheck(((Button)(button)).Margin.Left, ((Button)(button)).Margin.Top);

            public bool IsCoveredCheck(Button button) => IsCoveredCheck(((Button)(button)).Margin.Left, ((Button)(button)).Margin.Top);
            public void MoveButton(double x, double y)
            {
                QuadraticEase easingFunction = new QuadraticEase();
                easingFunction.EasingMode = EasingMode.EaseOut;
                ThicknessAnimation animation = new ThicknessAnimation
                {
                    To = new Thickness(x, y, 0, 0),
                    FillBehavior = FillBehavior.HoldEnd,
                    Duration = TimeSpan.FromSeconds(0.2),
                    EasingFunction = easingFunction,
                };
                button.BeginAnimation(Button.MarginProperty, animation);
            }
            public void MoveButton(Thickness thickness)
            {
                MoveButton(thickness.Left, thickness.Top);
            }
        }
        public static class MyTool
        {
            public static Thickness DeltaThinckness(Thickness left, Thickness right)
            {
                Thickness thickness = new Thickness(left.Left - right.Left, left.Top - right.Top, left.Right - right.Right, left.Bottom - right.Bottom);
                return thickness;
            }
            public static Thickness TotalThinckness(Thickness left, Thickness right)
            {
                Thickness thickness = new Thickness(left.Left + right.Left, left.Top + right.Top, left.Right + right.Right, left.Bottom + right.Bottom);
                return thickness;
            }
        }

        public class ButtonContainer
        {
            public static (double x, double y) ContainerPostion = (270, 300);
            public List<YangButton> YangButtons = new List<YangButton>();

            public Thickness Getpostion(int index)
            {
                return new Thickness(ContainerPostion.x + YangButton.ButtonWidth * (index - 1), ContainerPostion.y, 0, 0);
            }
            public void add(YangButton yangButton)
            {
                

                bool flag = false;
                for (int i = YangButtons.Count - 1; i >= 0; i--)
                {

                    if (YangButtons[i].picture == yangButton.picture)
                    {
                        flag = true;
                        if (i == YangButtons.Count)
                        {
                            flag = false; break;
                        }
                        else
                        {
                            YangButtons.Insert(i + 1, yangButton);
                            break;
                        }
                    }
                }
                if (flag == false)
                {
                    YangButtons.Add(yangButton);
                }
                check();
            }
            public void check()
            {
                Picture? nowpicture = null;
                int total = 0;
                for (int i = 0; i < YangButtons.Count; i++)
                {
                    if (nowpicture == null)
                    {
                        total = 1;
                        nowpicture = YangButtons[i].picture;
                    }
                    else
                    {
                        if (nowpicture == YangButtons[i].picture)
                        {
                            total++;
                            if (total == 3)
                            {
                                Destory(i - 2);
                                Destory(i - 2);
                                Destory(i - 2);
                                break;
                            }
                        }
                        else
                        {
                            total = 1;
                            nowpicture = YangButtons[i].picture;
                        }
                    }
                }
                resort();
                void Destory(int index)
                {
                    YangButton button = YangButtons[index];
                    Console.WriteLine($"{button.picture}移除");
                    YangButtons.Remove(button);
                    ((Button)(button)).Visibility = Visibility.Collapsed;
                }
                if (YangButtons.Count == 7)
                {
                    MessageBox.Show("你输了");
                }
            }
            void resort()
            {
                for (int i = 0; i < YangButtons.Count; i++)
                {
                    YangButtons[i].MoveButton(Getpostion(i + 1));
                }
            }
            public YangButton? find(Button button)
            {
                for (int i = 1; i <= YangButtons.Count; i++)
                {
                    if ((Button)YangButtons[i] == button)
                    {
                        return YangButtons[i];
                    }
                }
                return null;
            }
        }

        public MainWindow()
        {
            
            InitializeComponent();
            table.Source = new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, @"image\table.PNG")));
            myGrid.Background = new ImageBrush(new BitmapImage(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, @"image\background.png"))));
            firstlevel();
            void firstlevel()
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        YangButton button = new YangButton(200 + j * YangButton.ButtonWidth, 100 + i * YangButton.ButtonHeight, 1, this, true, Picture.lingdang);
                        this.Add(button);
                        tablebuttons.Add(button);
                    }
                }
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        YangButton button = new YangButton(225 + j * YangButton.ButtonWidth, 125 + i * YangButton.ButtonHeight, 2, this, true, Picture.naiping);
                        this.Add(button);
                        tablebuttons.Add(button);
                    }
                }
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        YangButton button = new YangButton(235 + j * YangButton.ButtonWidth, 100 + i * YangButton.ButtonHeight, 3, this, true, Picture.yumi);
                        this.Add(button);
                        tablebuttons.Add(button);
                    }
                }
                Complet = secondlevel;
            }
            void secondlevel()
            {
                List<(double x, double y, int index)> values = new List<(double x, double y, int index)>();
                List<Picture> picture = new List<Picture>();
                MessageBox.Show("下一关");
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        values.Add((300 + j * YangButton.ButtonWidth, 100 + i * YangButton.ButtonHeight, 1));
                        picture.Add(Picture.lingdang);
                        //YangButton button = new YangButton(10 + j * YangButton.ButtonWidth, 100 + i * YangButton.ButtonHeight, 1, this, true, Picture.lingdang);
                        //this.Add(button);
                        //tablebuttons.Add(button);
                    }
                }
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        values.Add((325 + j * YangButton.ButtonWidth, 125 + i * YangButton.ButtonHeight, 2));
                        picture.Add(Picture.naiping);
                        //YangButton button = new YangButton(35 + j * YangButton.ButtonWidth, 125 + i * YangButton.ButtonHeight, 2, this, true, Picture.naiping);
                        //this.Add(button);
                        //tablebuttons.Add(button);
                    }
                }
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        values.Add((335 + j * YangButton.ButtonWidth, 100 + i * YangButton.ButtonHeight, 3));
                        picture.Add(Picture.yumi);
                        //YangButton button = new YangButton(45 + j * YangButton.ButtonWidth, 100 + i * YangButton.ButtonHeight, 3, this, true, Picture.yumi);
                        //this.Add(button);
                        //tablebuttons.Add(button);
                    }
                }
                while (values.Count != 0)
                {
                    var s = RemoveRandomItem<(double x, double y, int index)>(values);
                    YangButton button = new YangButton(s.x, s.y, s.index, this, true, RemoveRandomItem<Picture>(picture));
                    this.Add(button);
                    tablebuttons.Add(button);
                }
                Complet = thirdllevel;
            }
            void thirdllevel()
            {
                List<YangButton> yangButtons = new List<YangButton>();
                MessageBox.Show("下一关");
                int count = 0;
                Picture pic = ((Picture)(new Random().NextInt64(1, 5)));
                for (int i = 1;i<6; )
                {
                    double x = new Random().NextInt64(120, 700);
                    double y = new Random().NextInt64(50, 200);
                    
                    YangButton button = new YangButton(x, y, i, this, true, pic);
                    int flag = 0;
                    foreach(YangButton w in  tablebuttons.tablebuttons)
                    {
                        if (w == button)
                        {
                            break;
                        }
                        else
                        {
                            if(Grid.GetZIndex((Button)(w))== Grid.GetZIndex((Button)(button))&&button.IsCoveredCheck(w))
                            {
                                flag=1;
                                break;
                            }
                            else
                            {
                                flag = 0;
                                
                            }
                        }
                    }
                    if (count >= 9)
                    {
                        count = 0;
                        i++;
                    }
                    if (flag == 0)
                    {
                        AddButtion(button);
                    }
                }
                Complet = thirdllevel;
                void AddButtion(YangButton button)
                {
                    yangButtons.Add(button);
                    if (yangButtons.Count == 3)
                    {
                        foreach(YangButton ww in yangButtons)
                        {
                            this.Add(ww);
                            tablebuttons.Add(ww);
                            count++;
                        }
                        yangButtons.Clear();
                        pic = ((Picture)(new Random().NextInt64(1, 5)));
                    }
                    
                }
            }
            void end()
            {
                MessageBox.Show("");
            }
            T RemoveRandomItem<T>(List<T> list)
            {
                if (list.Count == 0)
                    throw new InvalidOperationException("The list is empty.");

                Random random = new Random();
                int index = random.Next(0, list.Count);
                T item = list[index];
                list.RemoveAt(index);
                return item;
            }
            
        }
        public void moveButton_Click(object sender, RoutedEventArgs e)
        {
            Button myButton = sender as Button;
            //Rectangle rect = new Rectangle { Fill = Brushes.LightGray };
            //SolidColorBrush opacityMask = new SolidColorBrush(Colors.Black);
            //myButton.OpacityMask = opacityMask;
            YangButton? s = tablebuttons.Find(myButton);
            if (s == null)
            {
                throw new Exception("cantfind");
            }
            container.add(s);
            tablebuttons.Remove(s);

            if (container.YangButtons.Count == 0 && tablebuttons.tablebuttons.Count == 0)
            {
                Complet();
            }

        }
    }
}
