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

namespace Battleships_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string MouseHover;

        public MainWindow()
        {
            InitializeComponent();
            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    Button MyControl1 = new Button();
                    var brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri("C:\\Users\\Josef\\source\\repos\\Battleships-WPF\\Battleships-WPF\\Images\\WaterTileResized.png"));
                    brush.Stretch = Stretch.Fill;
                    MyControl1.Background = brush;
                    MyControl1.Name = "Button" + count.ToString();
                    MyControl1.Click += new RoutedEventHandler(button_Click);
                    MyControl1.MouseEnter += new MouseEventHandler(button_Enter);
                    if (j != 9)
                    {
                        Grid.SetColumn(MyControl1, j);
                        Grid.SetRow(MyControl1, i);
                        watertiles.Children.Add(MyControl1);
                        count++;
                    }               
                }
            }
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            Button srcButton = e.Source as Button;
            string buttonpressed = srcButton.Name;
            ButtonX.Text = buttonpressed;
        }

        void button_Enter(object sender, MouseEventArgs e)
        {
            Button srcButton = e.Source as Button;
            string buttonHover = srcButton.Name;
            MousePositionText.Text = $"MouseOver : {buttonHover}";
            MouseHover = buttonHover;

        }
    }
}
