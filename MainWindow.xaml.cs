using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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

        private int currentBoatVisualIndex = 0;
        private string[] boatLibrary = new string[] { "BigBoat", "MediumBoat", "LittleBoat" };

        private Classes.Boat currentOptionBoat;

        public static string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        public MainWindow()
        {
            InitializeComponent();
            CreateMap();
            LoadImages();
            CreateBoatImage();
            CreateTitleImage();
        }
        private void TitleButton_Click(object sender, RoutedEventArgs e)
        {
            //Gömmer huvudmenyn när man klickar på 'Begin Game'
            TitleCanvas.Visibility = Visibility.Collapsed;
            TitleButton.Visibility = Visibility.Collapsed;
            watertiles.Visibility = Visibility.Visible;
            watertiles2.Visibility = Visibility.Visible;
            ButtonGrid.Visibility = Visibility.Visible;
        }
        void CreateTitleImage()
        {
            TransformedBitmap transformBmp = new TransformedBitmap();
            BitmapImage bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.UriSource = new Uri(projectDirectory + $"\\Images\\titlescreenpicture.jpeg", UriKind.RelativeOrAbsolute);
            bmpImage.EndInit();
            Image BodyImage = new Image
            {
                Width = 1100,
                Height = 500,
                Source = bmpImage
            };
            TitleCanvas.Children.Add(BodyImage);
        }
        void LoadImages()
        {
            for(int i = 0; i < boatLibrary.Length; i++)
            {
                TransformedBitmap transformBmp = new TransformedBitmap();
                BitmapImage bmpImage = new BitmapImage();

                bmpImage.BeginInit();

                string boatName = boatLibrary[i];

                bmpImage.UriSource = new Uri(projectDirectory + $"\\Images\\Boats\\{boatName}.png", UriKind.RelativeOrAbsolute);

                bmpImage.EndInit();

                transformBmp.BeginInit();

                transformBmp.Source = bmpImage;

                RotateTransform transform = new RotateTransform(0);

                transformBmp.Transform = transform;

                transformBmp.EndInit();

                Image BodyImage = new Image
                {
                    Width = 51,
                    Height = 153,
                    Name = boatName,
                    Source = transformBmp
                    //new BitmapImage(new Uri(projectDirectory+"\\Images\\BigBoat\\BigBoat.png", UriKind.Absolute)), 
                    //transformBmp
                };
                BodyImage.MouseMove += BodyImage_MouseMove;
            }
        }

        void CreateBoatImage(int rotationValue=0, string boatName="BigBoat")
        {
            TransformedBitmap transformBmp = new TransformedBitmap();
            BitmapImage bmpImage = new BitmapImage();

            bmpImage.BeginInit();

            bmpImage.UriSource = new Uri(projectDirectory + $"\\Images\\Boats\\{boatName}.png", UriKind.RelativeOrAbsolute);

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
                Name = boatName,
                Source = transformBmp
                //new BitmapImage(new Uri(projectDirectory+"\\Images\\BigBoat\\BigBoat.png", UriKind.Absolute)), 
                //transformBmp
            };
            BodyImage.MouseMove += BodyImage_MouseMove;
            //mages.Add(BodyImage);

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
            int index = -1;
            for(int i = 0; i < Images.Count; i++)
            {
                if (Images[i].Name == boatName)
                {
                    index = i;
                }
            }
            if (index != -1)
            {
                Images[index] = BodyImage;
            }
            else { Images.Add(BodyImage);}
            ImageCanvas.Children.Add(BodyImage);
        }

        private void GridDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.Serializable);
            if(data is UIElement element)
            {
                element.Uid = Images[currentBoatVisualIndex].Name;
                var Pos = (UIElement)e.Source;
                int c = Grid.GetColumn(Pos);
                int r = Grid.GetRow(Pos);
                Grid.SetColumn(element, c);
                Grid.SetRow(element, r);
                if(currentRotation == "Vertical")
                {
                    Grid.SetRowSpan(element, 3);
                    Grid.SetColumnSpan(element, 1);
                }
                else if(currentRotation == "Horizontal")
                {
                    Grid.SetColumnSpan(element, 3);
                    Grid.SetRowSpan(element, 1);
                }
                UIElement checker = null;
                foreach(UIElement el in watertiles.Children)
                {
                    if(el.Uid == element.Uid) 
                    {
                        checker = el;
                    }
                }if(checker != null)
                {
                    watertiles.Children.Remove(checker);
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
                Image currentImage = Images[currentBoatVisualIndex];
                DragDrop.DoDragDrop(currentImage, new DataObject(DataFormats.Serializable, currentImage), DragDropEffects.Move);
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

        private void SpinButton_Click(object sender, RoutedEventArgs e)
        {
            if (ImageCanvas.Children.Count > 0)
            {
                ImageCanvas.Children.RemoveAt(0);
                currentBoatRotation += 90;

                if (currentBoatRotation < 0)
                {
                    currentBoatRotation = 270;
                }

                if (currentBoatRotation > 360)
                {
                    currentBoatRotation = 90;
                }

                CreateBoatImage(currentBoatRotation, boatLibrary[currentBoatVisualIndex]);
            }
            ButtonX.Text = $"Spin : {currentBoatRotation}";
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            currentBoatVisualIndex++;

            //Checks, so index doesnt go out of bounds
            if(currentBoatVisualIndex > boatLibrary.Length-1)
            {
                currentBoatVisualIndex = 0;
            }

            if (currentBoatVisualIndex < 0)
            {
                currentBoatVisualIndex = boatLibrary.Length-1;
            }

            //Clean the boat visual on the canvas
            if (ImageCanvas.Children.Count > 0)
            {
                ImageCanvas.Children.RemoveAt(0);
            }


            //Add the new boat visual to the canvas
            if (ImageCanvas.Children.Count == 0)
            {
                CreateBoatImage(boatName: boatLibrary[currentBoatVisualIndex]);
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            currentBoatVisualIndex--;

            //Checks, so index doesnt go out of bounds
            if (currentBoatVisualIndex > boatLibrary.Length - 1)
            {
                currentBoatVisualIndex = 0;
            }

            if (currentBoatVisualIndex < 0)
            {
                currentBoatVisualIndex = boatLibrary.Length - 1;
            }

            //Clean the boat visual on the canvas
            if (ImageCanvas.Children.Count > 0)
            {
                ImageCanvas.Children.RemoveAt(0);
            }


            //Add the new boat visual to the canvas
            if (ImageCanvas.Children.Count == 0)
            {
                CreateBoatImage(boatName: boatLibrary[currentBoatVisualIndex]);
            }
        }
    }
}
