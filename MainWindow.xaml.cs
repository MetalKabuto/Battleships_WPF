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
                    brush.ImageSource = new BitmapImage(new Uri("C:\\Users\\ddzed\\source\\repos\\Battleships WPF\\Images\\WaterTileResized.png"));
                    brush.Stretch = Stretch.Fill;
                    MyControl1.Background = brush;
                    MyControl1.Name = "Button" + count.ToString();
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
    }
}
