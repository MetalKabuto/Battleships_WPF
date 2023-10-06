using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
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
using static Battleships_WPF.Classes;

namespace Battleships_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Player MainPlayer = new Classes.Player(0, 3);
        public Player Enemy = new Classes.Player(1, 3);
        public Match match;

        public static MainWindow Instance { get; private set; }

        public static string MouseHover;

        public static int[] TakenRowCoords = new int[50];
        public static int[] TakenColCoords = new int[50];
        public static List<Image> Images = new List<Image>();
        public static string[] boatLibrary = new string[] { "BigBoat", "MediumBoat", "LittleBoat" };
        public static List<Classes.BoatTemplate> boatTemplates = new List<Classes.BoatTemplate>();

        //Preview Window variables
        public PreviewWindow previewWindow = new Classes.PreviewWindow();
        public Boat currentPreviewBoat;
        public Match currentMatch;

        public static string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        public MainWindow()
        {
            InitializeComponent();

            Instance = this;
            PopulateBoatTemplates();

            currentPreviewBoat = new Classes.Boat("BigBoat", 3);

            LoadImages();   //Fill Images list with images, of the elements in Boat Library

            currentMatch = new Classes.Match(-1, -1, -1, -1);
            currentMatch.CreateMap(watertiles, watertiles2);

            previewWindow.CreateBoatImage(ImageCanvas);
            currentMatch.CreateTitleImage(TitleCanvas);
            AI_Randomize();
        }

        public Boat Randomize_Boat(Boat boat)
        {
            string[] orientations = new string[] { "Horizontal", "Vertical" };

            Classes.Boat newboat = boat;
            Random randomRow = new Random();
            int row = randomRow.Next(9);
            Random randomCol = new Random();
            int col = randomCol.Next(9);
            Random Ori = new Random();
            int index = Ori.Next(orientations.Length);
            newboat.currentOrientation = orientations[index];
            if (col == 8 && newboat.currentOrientation == "Horizontal")
            {
                if (newboat.BoatSize == 3)
                {
                    col -= 2;
                }
                else if (newboat.BoatSize == 2)
                {
                    col -= 1;
                }
            }
            else if (col == 7 && newboat.BoatSize == 3 && newboat.currentOrientation == "Horizontal")
            {
                col -= 1;
            }
            if (row == 8 && newboat.currentOrientation == "Vertical")
            {
                if (newboat.BoatSize == 3)
                {
                    row -= 2;
                }
                else if (newboat.BoatSize == 2)
                {
                    row -= 1;
                }
            }
            else if (row == 7 && newboat.BoatSize == 3 && newboat.currentOrientation == "Vertical")
            {
                row -= 1;
            }
            newboat.row_number = row;
            newboat.column_number = col;
            return newboat;
        }

        private void AI_Randomize()
        {
            for (int i = 0; i < Enemy.boats.Count; i++)
            {
                Enemy.boats[i] = Randomize_Boat(Enemy.boats[i]);
                SetBoatPartPositions(Enemy.boats[i]);
                while (CheckOverlappingBoats(Enemy.boats[i]))
                {
                    Enemy.boats[i] = Randomize_Boat(Enemy.boats[i]);
                    SetBoatPartPositions(Enemy.boats[i]);
                }
            }
        }

        private bool CheckOverlappingBoats(Boat boat)
        {
            foreach(BoatParts part in boat.parts)
            {
                foreach(Boat b in Enemy.boats)
                {
                    if(boat.boatName != b.boatName)
                    {
                        foreach(BoatParts p in b.parts)
                        {
                            if(p.rowPos == part.rowPos && p.colPos  == part.colPos)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        private Boat addBoatParts(Boat boat)
        {
            for (int i = 0;i < boat.BoatSize;i++)
            {
                boat.parts.Add(new Classes.BoatParts(0, 0));
            }
            return boat;
        }

        private Boat SetBoatPartPositions(Boat boat)
        {
            int originalRow = boat.row_number;
            int originalCol = boat.column_number;
            foreach(Classes.BoatParts boatPart in boat.parts)
            {
                if(boat.currentOrientation == "Horizontal")
                {
                    boatPart.rowPos = originalRow;
                    boatPart.colPos = originalCol;
                    originalCol += 1;
                }
                else if(boat.currentOrientation == "Vertical")
                {
                    boatPart.rowPos = originalRow;
                    boatPart.colPos = originalCol;
                    originalRow += 1;
                }
            }
            
            return boat;
        }
        private void TitleButton_Click(object sender, RoutedEventArgs e)
        {
            //Gömmer huvudmenyn när man klickar på 'Begin Game'
            TitleCanvas.Visibility = Visibility.Collapsed;
            TitleButton.Visibility = Visibility.Collapsed;
            watertiles.Visibility = Visibility.Visible;
            watertiles2.Visibility = Visibility.Visible;
            ButtonGrid.Visibility = Visibility.Visible;
            Border1.Visibility = Visibility.Visible;
            Border2.Visibility = Visibility.Visible;
            Border3.Visibility = Visibility.Visible;
            Border4.Visibility = Visibility.Visible;
            Border5.Visibility = Visibility.Visible;
            P1TextBlock.Visibility = Visibility.Visible;
            P2TextBlock.Visibility = Visibility.Visible;
        }
        void LoadImages()
        {
            //FIXME: Move to library class when we have one...

            
            Classes.Boat boat = new Classes.Boat(GetBoatTemplate(currentPreviewBoat.boatName));
            

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
                Images.Add(BodyImage);
            }

            float x = 0;
        }

        void PopulateBoatTemplates()
        {
            for (int i = 0; i < boatLibrary.Length; i++)
            {
                Classes.BoatTemplate boatTemplate = new Classes.BoatTemplate() { boatName = boatLibrary[i] };

                if(boatTemplate.boatName == "BigBoat")
                {
                    boatTemplate.boatSize = 3;
                }
                else if (boatTemplate.boatName == "MediumBoat")
                {
                    boatTemplate.boatSize = 2;
                }
                else if (boatTemplate.boatName == "LittleBoat")
                {
                    boatTemplate.boatSize = 1;
                }

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

                boatTemplate.boatImage = BodyImage;
                MainPlayer.boats.Add(addBoatParts(new Classes.Boat(boatTemplate)));
                Enemy.boats.Add(addBoatParts(new Classes.Boat(boatTemplate)));
                boatTemplates.Add(boatTemplate);
            }
        }

        public Classes.BoatTemplate GetBoatTemplate(string boatName)
        {
            Classes.BoatTemplate boatTemplate = new Classes.BoatTemplate();

            foreach (var template in boatTemplates)
            {
                if(template.boatName == boatName)
                {
                    boatTemplate = template;
                }
            }

            return boatTemplate;
        }

        public void CreateBoatImage(int rotationValue=0, string boatName="BigBoat")
        {
            previewWindow.CreateBoatImage(ImageCanvas, rotationValue, boatName);
        }

        private void GridDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.Serializable);
            if (data is UIElement element)
            {
                element.Uid = Images[Classes.PreviewWindow.currentBoatVisualIndex].Name;
                var Pos = (UIElement)e.Source;
                int c = Grid.GetColumn(Pos);
                int r = Grid.GetRow(Pos);
                Grid.SetColumn(element, c);
                Grid.SetRow(element, r);

                Image image = (Image)data;

                int boatSize = currentPreviewBoat.BoatSize;

                if (currentPreviewBoat.currentOrientation == "Vertical")
                {
                    Image boatImage = data as Image;
                    if (boatImage != null)
                    {
                        boatImage.Height = 153;
                        boatImage.Height /= 3;
                        boatImage.Height *= boatSize;
                    }
                    MainPlayer.boats[Classes.PreviewWindow.currentBoatVisualIndex].currentOrientation = "Vertical";
                    Grid.SetRowSpan(element, boatSize);
                    Grid.SetColumnSpan(element, 1);
                }
                else if (currentPreviewBoat.currentOrientation == "Horizontal")
                {
                    Image boatImage = data as Image;
                    if (boatImage != null)
                    {
                        boatImage.Width = 153;
                        boatImage.Width /= 3;
                        boatImage.Width *= boatSize;
                    }
                    MainPlayer.boats[Classes.PreviewWindow.currentBoatVisualIndex].currentOrientation = "Horizontal";
                    Grid.SetColumnSpan(element, boatSize);
                    Grid.SetRowSpan(element, 1);
                }
                UIElement checker = null;
                foreach (UIElement el in watertiles.Children)
                {
                    if (el.Uid == element.Uid)
                    {
                        checker = el;
                    }
                }
                if (checker != null)
                {
                    watertiles.Children.Remove(checker);
                }
                MainPlayer.boats[Classes.PreviewWindow.currentBoatVisualIndex].column_number = c;
                MainPlayer.boats[Classes.PreviewWindow.currentBoatVisualIndex].row_number = r;
                MainPlayer.boats[Classes.PreviewWindow.currentBoatVisualIndex] = SetBoatPartPositions(MainPlayer.boats[Classes.PreviewWindow.currentBoatVisualIndex]);
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
                    ImageCanvas.Children.Remove(element);
                    watertiles.Children.Remove(element);
                }
            }

        }

        public static void BodyImage_MouseMove(object sender, MouseEventArgs e)
        {
            //Det går inte att klicka på skeppen när matchen har börjat.
            if (matchStart == false)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Image currentImage = Images[Classes.PreviewWindow.currentBoatVisualIndex];
                    DragDrop.DoDragDrop(currentImage, new DataObject(DataFormats.Serializable, currentImage), DragDropEffects.Move);
                }
            }
            else
            {
                //Gör inget
            }
        }
        private void ImageDrop(object sender, DragEventArgs e)
        {
            Point dropposition = e.GetPosition(ImageCanvas);
            Canvas.SetLeft(ImageCanvas.Children[0], dropposition.X - 25);
            Canvas.SetTop(ImageCanvas.Children[0], dropposition.Y - 50);
        }

        public void button_Click(object sender, RoutedEventArgs e)
        {
            Button srcButton = e.Source as Button;
            string buttonpressed = srcButton.Name;
            ButtonX.Text = "text";
            if (matchStart == true && match.winner != 0 && match.winner != 1)
            {
                if(match.turnid == 0)
                {
                    var Pos = (Button)e.Source;
                    int c = Grid.GetColumn(Pos);
                    int r = Grid.GetRow(Pos);
                    ButtonX.Text = $"row: {r} col: {c}";
                    if (Attack(Enemy, r, c))
                    {
                        Image BodyImage = new Image
                        {
                            Width = 51,
                            Height = 51,
                            Name = "fire",
                            Source = new BitmapImage(new Uri(MainWindow.projectDirectory + "\\Images\\Boats\\Fire.png", UriKind.RelativeOrAbsolute)),
                            Stretch = Stretch.Fill
                        };
                        Grid.SetColumn(BodyImage, c);
                        Grid.SetRow(BodyImage, r);
                        watertiles2.Children.Add(BodyImage);
                        ButtonX.Text = $"Boat Hit in position row: {r} col: {c}";
                    } 
                }

            }
            if (MainPlayer.PlayerBoats == 0)
            {
                match.winner = 1;
                ButtonX.Text = "Enemy Won!!";
            }
            else if (Enemy.PlayerBoats == 0)
            {
                match.winner = 0;
                ButtonX.Text = "You won!!!";
            }
        }

        public void button_Enter(object sender, MouseEventArgs e)
        {
            Button srcButton = e.Source as Button;
            string buttonHover = srcButton.Name;
            //MousePositionText.Text = $"MouseOver : {buttonHover}";
            MouseHover = buttonHover;
        }
        //Används för att 'stänga av' de andra knapparna när man tryckt match start.
        //Var tvungen att göra den static för att det skulle funka med 'BodyImage_MouseMove', eftersom den också är static.
        static bool matchStart = false;
        private void StartMatch_Click(object sender, RoutedEventArgs e)
        {
            matchStart = true;
            match = new Match(-1,0,MainPlayer.boats.Count, Enemy.boats.Count);
            MainPlayer.PlayerBoats = MainPlayer.boats.Count;
            Enemy.PlayerBoats = Enemy.boats.Count;
        }
        private void SpinButton_Click(object sender, RoutedEventArgs e)
        {
            if (matchStart == false){
                previewWindow.SpinBoat(ImageCanvas, currentPreviewBoat);
            }
            else
            {
                //Gör inget
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (matchStart == false)
            {
                previewWindow.NextButton_Click(ImageCanvas);
            }
            else
            {
                //Gör inget
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (matchStart == false)
            {
                previewWindow.PreviousButton_Click(ImageCanvas);
            }
            else
            {
                //Gör inget
            }
        }
    }
}
