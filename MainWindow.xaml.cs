using System;
using System.Collections.Generic;
using System.IO;
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
        public static string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        public MainWindow()
        {
            
            InitializeComponent();
            CreateMap();
            CreateBoatImage();           
        }
        void CreateBoatImage()
        {
            Image BodyImage = new Image
            {
                Width = 51,
                Height = 153,
                Name = "BigBoat",
                Source = new BitmapImage(new Uri("C:\\Users\\ddzed\\source\\repos\\Battleships WPF\\Images\\BigBoat\\BigBoat.png", UriKind.Absolute)),
            };
            BodyImage.MouseMove += BodyImage_MouseMove;
            ImageCanvas.Children.Add(BodyImage);
            Canvas.SetLeft(BodyImage, 90);
            Canvas.SetTop(BodyImage, 80);
        }

        private void BodyImage_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(ImageCanvas.Children[0], ImageCanvas.Children[0], DragDropEffects.Move);
            }
        }
        private void ImageDrop(object sender, DragEventArgs e)
        {
            Point dropposition = e.GetPosition(ImageCanvas);
            Canvas.SetLeft(ImageCanvas.Children[0], dropposition.X-25);
            Canvas.SetTop(ImageCanvas.Children[0], dropposition.Y-50);
        }

        void CreateMap()
        {
            int count = 0;
            int count2 = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {

                    Button MyControl1 = new Button();
                    var brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri(projectDirectory + "\\Images\\WaterTileResized.png"));
                    brush.Stretch = Stretch.Fill;
                    MyControl1.Background = brush;
                    MyControl1.Name = "Player" + count.ToString();
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
                for (int j = 0; j < 10; j++)
                {

                    Button MyControl1 = new Button();
                    var brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri(projectDirectory + "\\Images\\WaterTileResized.png"));
                    brush.Stretch = Stretch.Fill;
                    MyControl1.Background = brush;
                    MyControl1.Name = "Enemy" + count2.ToString();
                    MyControl1.Click += new RoutedEventHandler(button_Click);
                    MyControl1.MouseEnter += new MouseEventHandler(button_Enter);
                    if (j != 9)
                    {
                        Grid.SetColumn(MyControl1, j);
                        Grid.SetRow(MyControl1, i);
                        watertiles2.Children.Add(MyControl1);
                        count2++;
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
