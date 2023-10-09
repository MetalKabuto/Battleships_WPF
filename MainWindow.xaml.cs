using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static Battleships_WPF.Classes;
using System.Diagnostics;
using System.Reflection;

namespace Battleships_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        //Players
        public static Player MainPlayer = new Player(0, 3);
        public static Player Enemy = new Player(1, 3);
        public static Match match;
        static bool matchStart = false;
        
        //Sound 
        public static MediaPlayer mediaPlayer;
        public static double masterVolume;

        //Controls
        public static MainWindow Instance { get; private set; }
        public static string MouseHover;

        //Player used tiles
        public static List<Coordinates> PlayerTakenCoordinates = new List<Coordinates>();
        public static List<Coordinates> TakenCoordinates= new List<Coordinates>();

        //Interface Variables
        public static List<Image> Images = new List<Image>();
        public static string[] boatLibrary = new string[] { "HugeBoat", "BigBoat", "MediumBoat1", "MediumBoat2", "LittleBoat" };
        public static List<BoatTemplate> boatTemplates = new List<Classes.BoatTemplate>();

        //Preview Window variables
        public PreviewWindow previewWindow = new PreviewWindow();
        public Boat currentPreviewBoat;
        public Match currentMatch;
        public static string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        public MainWindow()
        {
            InitializeComponent();

            Instance = this;
            PreviewWindow.PopulateTemplates();

            currentPreviewBoat = new Boat(boatTemplates[0].boatName, boatTemplates[0].boatSize);

            LoadImages();   //Fill Images list with images, of the elements in Boat Library

            currentMatch = new Match(-1, -1, -1, -1);
            currentMatch.CreateMap(watertiles, watertiles2);

            previewWindow.CreateBoatImage(ImageCanvas);
            currentMatch.CreateTitleImage(TitleCanvas);
            currentMatch.CreateButtonImage(TitleButton);
            //AI placerar skepp när man startar programmet
            Match.AI_Randomize();
        }
        public static void PlaySound(string filename, int volume)
        {
            mediaPlayer = new MediaPlayer();
            mediaPlayer.Volume = (volume / 100.0f) * masterVolume;
            mediaPlayer.Open(new Uri(filename));
            mediaPlayer.Play();
        }
        private void RestartProgram()
        {
            //taget från http://csharp.tips/tip/article/962-how-to-restart-program-in-csharp
            //OBS: Pathen kanske inte funkar om man kör programmet utanför debugging?
            string ProgramPath = $"{projectDirectory}\\bin\\Debug\\net6.0-windows\\Battleships WPF.EXE"; 
            Process.Start(ProgramPath);
            Environment.Exit(0);
        }
        private void TitleButton_Click(object sender, RoutedEventArgs e)
        {
            //Gömmer huvudmenyn när man klickar på 'Begin Game'
            TitleCanvas.Visibility = Visibility.Collapsed;
            TitleButton.Visibility = Visibility.Collapsed;
            TitleText.Visibility = Visibility.Collapsed;
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
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            RestartProgram();
        }
        void LoadImages()
        {
            //FIXME: Move to library class when we have one...

            
            Boat boat = new Boat(Boat.GetBoatTemplate(currentPreviewBoat.boatName));
            

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
                };
                BodyImage.MouseMove += BodyImage_MouseMove;
                Images.Add(BodyImage);
            }
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
                element.Uid = Images[PreviewWindow.currentBoatVisualIndex].Name;
                var Pos = (UIElement)e.Source;
                int c = Grid.GetColumn(Pos);
                int r = Grid.GetRow(Pos);
                int boatSize = currentPreviewBoat.BoatSize;
                Boat.CheckBorders(currentPreviewBoat,ref r,ref c);
                if (currentPreviewBoat.currentOrientation == "Vertical")
                {
                    Image boatImage = data as Image;
                    if (boatImage != null)
                    {
                        boatImage.Height = 153;
                        boatImage.Height /= 3;
                        boatImage.Height *= boatSize;
                    }
                    MainPlayer.boats[PreviewWindow.currentBoatVisualIndex].currentOrientation = "Vertical";
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
                Grid.SetColumn(element, c);
                Grid.SetRow(element, r);
                MainPlayer.boats[PreviewWindow.currentBoatVisualIndex].column_number = c;
                MainPlayer.boats[PreviewWindow.currentBoatVisualIndex].row_number = r;
                MainPlayer.boats[PreviewWindow.currentBoatVisualIndex] = Boat.SetBoatPartPositions(MainPlayer.boats[PreviewWindow.currentBoatVisualIndex]);
                watertiles.Children.Add(element);
                if (Boat.CheckOverlappingBoats(MainPlayer.boats[PreviewWindow.currentBoatVisualIndex], MainPlayer))
                {
                    watertiles.Children.Remove(element);
                    ButtonX.Text = "The boat is overlapping another boat place it properly";
                }
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
            try
            {
                Point dropposition = e.GetPosition(ImageCanvas);
                Canvas.SetLeft(ImageCanvas.Children[0], dropposition.X - 25);
                Canvas.SetTop(ImageCanvas.Children[0], dropposition.Y - 50);
            }
            catch (ArgumentOutOfRangeException)
            {
                ButtonX.Text = "Cant place boats outside player bounds";
            }
        }
        public void button_Click(object sender, RoutedEventArgs e)
        {
            Button srcButton = e.Source as Button;
            string buttonpressed = srcButton.Name;
            if (matchStart == true && match.winner != 0 && match.winner != 1)
            {
                //Tror man kan ta bort turnid
                if (match.turnid == 0)
                {
                    DelayAttack(e,watertiles,watertiles2,ButtonX, MousePositionText);
                }
            }
            //Lade till så att restartknappen visas när matchen är över
            //Createbuttonimage körs i båda satserna, eftersom bilden som visas inte är samma
            if (MainPlayer.PlayerBoats == 0)
            {
                match.winner = 1;
                EndScreen();
            }
            else if (Enemy.PlayerBoats == 0)
            {
                match.winner = 0;
                EndScreen();
            }
        }

        async static void DelayAttack(RoutedEventArgs e,Grid grid,Grid grid2, TextBox box,TextBox box2)
        {
            var Pos = (Button)e.Source;
            int c = Grid.GetColumn(Pos);
            int r = Grid.GetRow(Pos);
            box.Text = $"row: {r} col: {c}";
            if (Match.checkCoordinates(new Coordinates(r, c), PlayerTakenCoordinates))
            {
                box.Text = "Already attacked this location!";
            }
            else
            {
                PlayerTakenCoordinates.Add(new Coordinates(r, c));
                if (Attack(Enemy, r, c) == true)
                {
                    Match.AddFire(r, c, grid2);
                    box.Text = $"Boat Hit in position row: {r} col: {c}";
                    PlaySound(projectDirectory + "\\Sounds\\hitsound1.wav", 3);
                }
                //Lade till så att det visas en ikon på rutor man gissat på, men som inte har skepp.
                else
                {
                    Match.AddMiss(c, r, grid2);
                    box.Text = $"No ship at the position: row {r} col: {c}";
                    PlaySound(projectDirectory + "\\Sounds\\watersplash.wav", 15);
                    match.turnid = 1;
                }
                box2.Text = "Enemy Turn";
                await Task.Delay(2000);
                if (match.turnid == 1)
                {
                    Player.AIattacks(grid);
                    box2.Text = "Your Turn";
                }
            }
        }

        public void EndScreen()
        {
                currentMatch.CreateButtonImage(RestartButton);
                Match.CreateResultImage(ResultCanvas);
                RestartButton.Visibility = Visibility.Visible;
                ResultCanvas.Visibility = Visibility.Visible;
                if(match.winner == 0)
            {
                Match.WinnerImage(Winner,"Win.png");
            }
            else
            {
                Match.WinnerImage(Winner, "Lose.png");
            }
                Winner.Visibility = Visibility.Visible;
        }
        public void button_Enter(object sender, MouseEventArgs e)
        {
            Button srcButton = e.Source as Button;
            string buttonHover = srcButton.Name;
            MouseHover = buttonHover;
        }
        private void StartMatch_Click(object sender, RoutedEventArgs e)
        {
            matchStart = true;
            match = new Match(-1,0,MainPlayer.boats.Count, Enemy.boats.Count);
            MainPlayer.PlayerBoats = MainPlayer.boats.Count;
            Enemy.PlayerBoats = Enemy.boats.Count;
            int BoatChecker = 0;
            foreach(UIElement element in watertiles.Children)
            {
                if(element.Uid == "HugeBoat" | element.Uid == "BigBoat" | element.Uid == "MediumBoat1"| element.Uid == "MediumBoat2" | element.Uid == "LittleBoat")
                {
                    BoatChecker += 1;
                }
            }
            if(BoatChecker != 5)
            {
                matchStart =false;
                MousePositionText.Text = $"Place all your boats!\n You only have placed {BoatChecker} boats";
            }
            else { MousePositionText.Text = "Your Turn"; }
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
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double volumeValue = (e.NewValue / 10.0);
            int volumeValuePercentage = (int)(volumeValue * 100.0);

            VolumeLabel.Content = $"Volume ({volumeValuePercentage}%)";
            masterVolume = volumeValue;
        }
    }
}
