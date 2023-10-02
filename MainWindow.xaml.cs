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
        public static List<Image> Images = new List<Image>();

        private int currentBoatRotation = 0; //FIXME: Ta bort denna och ha rotationsvärdet i båt-klassen istället
        private string currentRotation = "Vertical";

        public static string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        public MainWindow()
        {
            
            InitializeComponent();
            CreateMap();
            CreateBoatImage();           
        }
        void CreateBoatImage(int rotationValue=0)
        {
            TransformedBitmap transformBmp = new TransformedBitmap();
            BitmapImage bmpImage = new BitmapImage();

            bmpImage.BeginInit();

            bmpImage.UriSource = new Uri(projectDirectory + "\\Images\\BigBoat\\BigBoat.png", UriKind.RelativeOrAbsolute);

            bmpImage.EndInit();

            transformBmp.BeginInit();

            transformBmp.Source = bmpImage;

            RotateTransform transform = new RotateTransform(rotationValue);

            transformBmp.Transform = transform;

            transformBmp.EndInit();

            Image BodyImage = new Image
            {
                Width = 51,
                Height = 153,
                Name = "BigBoat",
                Source = transformBmp
                //new BitmapImage(new Uri(projectDirectory+"\\Images\\BigBoat\\BigBoat.png", UriKind.Absolute)), 
                //transformBmp
            };
            BodyImage.MouseMove += BodyImage_MouseMove;
            Images.Add(BodyImage);
            ImageCanvas.Children.Add(BodyImage);

            if (rotationValue == 0 || rotationValue == 180)
            {
                BodyImage.Width = 51;
                BodyImage.Height = 153;

                currentRotation = "Vertical";

                //FIXME : placeringen av båten, ligger lite off center nu
                Canvas.SetLeft(BodyImage, 80);
            }
            else if (rotationValue == 90 || rotationValue == 270)
            {
                BodyImage.Width = 153;
                BodyImage.Height = 51;

                currentRotation = "Horizontal";

                //FIXME : placeringen av båten, ligger lite off center nu
                Canvas.SetLeft(BodyImage, 30);
            }

            //Canvas.SetLeft(BodyImage, 80);
            Canvas.SetTop(BodyImage, 80);
        }

        private void GridDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.Serializable);
            if(data is UIElement element)
            {
                var Pos = (UIElement)e.Source;
                int c = Grid.GetColumn(Pos);
                int r = Grid.GetRow(Pos);
                Grid.SetColumn(element, c);
                Grid.SetRow(element, r);
                

                if(currentRotation == "Vertical")
                {
                    Grid.SetRowSpan(element, 3);
                }
                else if(currentRotation == "Horizontal")
                {
                    Grid.SetColumnSpan(element, 3);
                }               
                watertiles.Children.Add(element);
            }
        }
        private void Drag_Leave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource == ImageCanvas)
            {
                object data = e.Data.GetData(DataFormats.Serializable);
                if (data is UIElement element)
                {
                    ImageCanvas.Children.Remove(element);
                }
            }
            else
            {
                object data = e.Data.GetData(DataFormats.Serializable);
                if (data is UIElement element)
                {
                    watertiles.Children.Remove(element);
                }
            }

        }

        private void BodyImage_MouseMove(object sender, MouseEventArgs e)
        {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                DragDrop.DoDragDrop(Images[0], new DataObject(DataFormats.Serializable, Images[0]), DragDropEffects.Move);
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

        private void SpinButton_MouseDown(object sender, RoutedEventArgs e)
        {
            
            if (ImageCanvas.Children.Count > 0)
            {
                ImageCanvas.Children.RemoveAt(0);
                currentBoatRotation -= 90;

                if (currentBoatRotation < 0)
                {
                    currentBoatRotation = 270;
                }

                if (currentBoatRotation > 360)
                {
                    currentBoatRotation = 90;
                }

                CreateBoatImage(currentBoatRotation);
            }
            ButtonX.Text = $"Spin : {currentBoatRotation}";
        }
    }
}
