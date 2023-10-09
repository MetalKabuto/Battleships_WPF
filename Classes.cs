using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;

namespace Battleships_WPF
{
    public class Classes
    {
        public class Match
        {
            List<int> players;
            public int winner, turnid, playerBoats, enemyBoats;
            public Match(int winner, int turnid, int playerBoats, int enemyBoats)
            {
                players = new List<int>();
                this.winner = winner;
                this.turnid = turnid;
                this.playerBoats = playerBoats;
                this.enemyBoats = enemyBoats;
            }
            public int checkturn()
            {
                return turnid;
            }
            public int checkBoatAmount(Player player)
            {
                return player.AliveBoats(); 
            }
            public void AddPlayer(Player player)
            {
                players.Add(player.playerID);
            }
            public void CreateMap(Grid watertiles, Grid watertiles2)
            {
                int count = 0;
                int count2 = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {

                        Button MyControl1 = new Button();
                        var brush = new ImageBrush();
                        brush.ImageSource = new BitmapImage(new Uri(MainWindow.projectDirectory + "\\Images\\WaterTileResized.png"));
                        brush.Stretch = Stretch.Fill;
                        MyControl1.Background = brush;
                        MyControl1.Name = "Player" + count.ToString();
                        MyControl1.MouseEnter += new MouseEventHandler(MainWindow.Instance.button_Enter);
                        if (j != 9)
                        {
                            Grid.SetColumn(MyControl1, j);
                            Grid.SetRow(MyControl1, i);
                            watertiles.Children.Add(MyControl1);
                            count++;
                        }
                    }
                    for (int k = 0; k < 9; k++)
                    {

                        Button MyControl1 = new Button();
                        var brush = new ImageBrush();
                        brush.ImageSource = new BitmapImage(new Uri(MainWindow.projectDirectory + "\\Images\\WaterTileResized.png"));
                        brush.Stretch = Stretch.Fill;
                        MyControl1.Background = brush;
                        MyControl1.Name = "Enemy" + count2.ToString();
                        MyControl1.Click += new RoutedEventHandler(MainWindow.Instance.button_Click);
                        MyControl1.MouseEnter += new MouseEventHandler(MainWindow.Instance.button_Enter);
                        if (k != 9)
                        {
                            Grid.SetColumn(MyControl1, k);
                            Grid.SetRow(MyControl1, i);
                            watertiles2.Children.Add(MyControl1);
                            count2++;
                        }
                    }


                }
            }
            public void CreateTitleImage(Canvas TitleCanvas)
            {
                BitmapImage bmpImage = new BitmapImage();
                bmpImage.BeginInit();
                bmpImage.UriSource = new Uri(MainWindow.projectDirectory + $"\\Images\\titlescreenpicture.jpeg", UriKind.RelativeOrAbsolute);
                bmpImage.EndInit();
                Image BodyImage = new Image
                {
                    Width = 1100,
                    Height = 550,
                    Source = bmpImage
                };
                BodyImage.Stretch = Stretch.Fill;
                TitleCanvas.Children.Add(BodyImage);
            }
            public void CreateButtonImage(Button testbutton)
            {
                var brush = new ImageBrush();
                brush.Stretch = Stretch.Fill;
                if (testbutton.Name == "TitleButton")
                {
                    //FIX: Sätt riktiga bilder
                    brush.ImageSource = new BitmapImage(new Uri(MainWindow.projectDirectory + "\\Images\\Start.png"));
                    testbutton.Background = brush;
                }
                if (testbutton.Name == "RestartButton")
                {
                    brush.ImageSource = new BitmapImage(new Uri(MainWindow.projectDirectory + "\\Images\\Reset.png"));
                    testbutton.Background = brush;
                }
            }
            public static void AddFire(int row, int col, Grid grid)
            {
                Image BodyImage = new Image
                {
                    Width = 51,
                    Height = 51,
                    Name = "fire",
                    Source = new BitmapImage(new Uri(MainWindow.projectDirectory + "\\Images\\Boats\\Fire.png", UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.Fill
                };
                AddDestroyedBoat(grid);
                Grid.SetColumn(BodyImage, col);
                Grid.SetRow(BodyImage, row);
                grid.Children.Add(BodyImage);
            }
            public static void AddMiss(int c, int r, Grid grid)
            {
                Image BodyImage = new Image
                {
                    Width = 31,
                    Height = 31,
                    Name = "miss",
                    Source = new BitmapImage(new Uri(MainWindow.projectDirectory + "\\Images\\Boats\\Cross.png", UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.Fill
                };
                BodyImage.Uid = "miss" + r + c;
                Grid.SetColumn(BodyImage, c);
                Grid.SetRow(BodyImage, r);
                grid.Children.Add(BodyImage);
            }
            public static void AddDestroyedBoat(Grid grid)
            {
                foreach (Boat b in MainWindow.Enemy.boats)
                {
                    if (b.destroyed == true && b.painted == false)
                    {
                        TransformedBitmap transformBmp = new TransformedBitmap();
                        Image boatImage = new Image();
                        boatImage.Source = new BitmapImage(new Uri(MainWindow.projectDirectory + "\\Images\\Boats\\" + $"{b.boatName}" + ".png"));
                        if (b.currentOrientation == "Vertical")
                        {
                            if (boatImage != null)
                            {
                                boatImage.Height = 153;
                                boatImage.Height /= 3;
                                boatImage.Height *= b.size;
                            }
                            Grid.SetRowSpan(boatImage, b.size);
                            Grid.SetColumnSpan(boatImage, 1);
                        }
                        else if (b.currentOrientation == "Horizontal")
                        {
                            if (boatImage != null)
                            {
                                boatImage.Width = 153;
                                boatImage.Width /= 3;
                                boatImage.Width *= b.size;
                            }
                            Grid.SetColumnSpan(boatImage, b.size);
                            Grid.SetRowSpan(boatImage, 1);
                            transformBmp.BeginInit();
                            transformBmp.Source = new BitmapImage(new Uri(MainWindow.projectDirectory + "\\Images\\Boats\\" + $"{b.boatName}" + ".png"));
                            RotateTransform transform = new RotateTransform(90);
                            transformBmp.Transform = transform;
                            transformBmp.EndInit();
                            boatImage.Source = transformBmp;
                        }
                        Grid.SetColumn(boatImage, b.column_number);
                        Grid.SetRow(boatImage, b.row_number);
                        grid.Children.Add(boatImage);
                        b.painted = true;
                        foreach (BoatParts p in b.parts)
                        {
                            AddFire(p.rowPos, p.colPos, grid);
                        }
                    }
                }
            }
            public static void WinnerImage(Canvas canvas, string path)
            {
                BitmapImage bmpImage = new BitmapImage();
                bmpImage.BeginInit();
                bmpImage.UriSource = new Uri(MainWindow.projectDirectory + $"\\Images\\" + path, UriKind.RelativeOrAbsolute);
                bmpImage.EndInit();
                Image BodyImage = new Image
                {
                    Width = 150,
                    Height = 150,
                    Source = bmpImage
                };
                BodyImage.Stretch = Stretch.Fill;
                canvas.Children.Add(BodyImage);
            }
            public static void AI_Randomize()
            {
                int i = 0;
                foreach (Boat testboat in MainWindow.Enemy.boats.ToList())
                {
                    MainWindow.Enemy.boats[i] = Boat.Randomize_Boat(MainWindow.Enemy.boats[i]);
                    Boat.SetBoatPartPositions(MainWindow.Enemy.boats[i]);
                    while (Boat.CheckOverlappingBoats(MainWindow.Enemy.boats[i],MainWindow.Enemy) == true)
                    {
                        MainWindow.Enemy.boats[i] = Boat.Randomize_Boat(MainWindow.Enemy.boats[i]);
                        Boat.SetBoatPartPositions(MainWindow.Enemy.boats[i]);
                    }
                    i += 1;
                }
            }
            public static void CreateResultImage(Canvas canvas)
            {
                var brush = new ImageBrush();
                brush.Stretch = Stretch.Fill;
                //0 = vinst, 1 = förlust
                if (MainWindow.match.winner == 0)
                {
                    //FIX: Ändra till riktiga bilder.
                    brush.ImageSource = new BitmapImage(new Uri(MainWindow.projectDirectory + "\\Images\\titlescreenpicture.jpeg"));
                    canvas.Background = brush;
                }
                else if (MainWindow.match.winner == 1)
                {
                    brush.ImageSource = new BitmapImage(new Uri(MainWindow.projectDirectory + "\\Images\\titlescreenpicture.jpeg"));
                    canvas.Background = brush;
                }
            }
            public static bool checkCoordinates(Coordinates cord, List<Coordinates> TakenCoordinates)
            {
                foreach (Coordinates c in TakenCoordinates)
                {
                    if (c.row == cord.row && c.col == cord.col)
                    {
                        return true;
                    }
                }
                return false;
            }


        }

        public class Player
        {
            public int playerID, PlayerBoats;
            public List<Boat> boats;
            public Player(int playerID, int playerBoats)
            {
                this.playerID = playerID;
                PlayerBoats = playerBoats;
                this.boats = new List<Boat>();
            }
            public int AliveBoats()
            {
                int count = 0;
                foreach(Boat b in boats)
                {
                    if(b.destroyed == false)
                    {
                        count++;
                    }
                }
                return count;
            }
            public static void AIattacks(Grid grid)
            {
                while (MainWindow.match.turnid == 1)
                {
                    Random AIRandomRow = new Random();
                    int targetRow = AIRandomRow.Next(9);
                    Random AIRandomCol = new Random();
                    int targetCol = AIRandomCol.Next(9);
                    while (Match.checkCoordinates(new Coordinates(targetRow, targetCol), MainWindow.TakenCoordinates))
                    {
                        Random newRow = new Random();
                        targetRow = newRow.Next(9);
                        Random newCol = new Random();
                        targetCol = newCol.Next(9);
                    }
                    MainWindow.TakenCoordinates.Add(new Coordinates(targetRow, targetCol));
                    if (Attack(MainWindow.MainPlayer, targetRow, targetCol) == true)
                    {
                        Match.AddFire(targetRow, targetCol, grid);
                    }
                    else
                    {
                        Match.AddMiss(targetCol, targetRow, grid);
                        MainWindow.match.turnid = 0;
                    }
                }
            }
        }

        public class Boat
        {
            public string boatName; //FIXME: replace property
            public int boatID;
            public string boatPath;

            public int size, damagedParts;
            public bool destroyed,painted;
            public List<BoatParts> parts;

            public int row_number = -1;
            public int column_number = -1;

            public int BoatSize
            {
                get { return size; }
                set { size = value; }
            }

            public int currentRotationAngle = 0;
            public string currentOrientation = "Vertical";

            public Boat(string boatName, int size, int damagedParts=0)
            {
                this.painted = false;
                parts = new List<BoatParts>();
                this.size = size;
                this.damagedParts = damagedParts;
                this.destroyed = false;
                this.boatName = boatName;
            }
            public Boat(BoatTemplate template)
            {
                parts = new List<BoatParts>();

                this.painted = false;
                this.damagedParts = 0;
                this.destroyed = false;

                this.boatName = template.boatName;
                this.size = template.boatSize;
                this.boatPath = template.boatPath;
            }

            public void checkDamage()
            {
                int damage = 0;
                foreach(BoatParts p in parts)
                {
                    if (p.damaged == true)
                    {
                        damage++;
                    }
                }
                damagedParts = damage;
            } 

            public void SetRow(int newRowValue)
            {
                row_number = newRowValue;
            }
            public void SetColumn(int newColumnValue)
            {
                column_number = newColumnValue;
            }

            public static BoatTemplate GetBoatTemplate(string boatName)
            {
                BoatTemplate boatTemplate = new BoatTemplate();

                foreach (var template in MainWindow.boatTemplates)
                {
                    if (template.boatName == boatName)
                    {
                        boatTemplate = template;
                    }
                }

                return boatTemplate;
            }

            public static Boat addBoatParts(Boat boat)
            {
                for (int i = 0; i < boat.BoatSize; i++)
                {
                    boat.parts.Add(new BoatParts(0, 0));
                }
                return boat;
            }
            public static Boat SetBoatPartPositions(Boat boat)
            {
                int originalRow = boat.row_number;
                int originalCol = boat.column_number;
                foreach (BoatParts boatPart in boat.parts)
                {
                    if (boat.currentOrientation == "Horizontal")
                    {
                        boatPart.rowPos = originalRow;
                        boatPart.colPos = originalCol;
                        originalCol += 1;
                    }
                    else if (boat.currentOrientation == "Vertical")
                    {
                        boatPart.rowPos = originalRow;
                        boatPart.colPos = originalCol;
                        originalRow += 1;
                    }
                }

                return boat;
            }
            public static Boat Randomize_Boat(Boat boat)
            {
                //Horizontal genererar från vänster till höger, vertical uppifrån och ner
                string[] orientations = new string[] { "Horizontal", "Vertical" };
                Random randomRow = new Random();
                int row = randomRow.Next(9);
                Random randomCol = new Random();
                int col = randomCol.Next(9);
                Random Ori = new Random();
                int index = Ori.Next(orientations.Length);
                boat.currentOrientation = orientations[index];
                CheckBorders(boat, ref row, ref col);
                boat.row_number = row;
                boat.column_number = col;
                return boat;
            }

            public static void CheckBorders(Boat boat, ref int row, ref int col)
            {
                if (col + boat.size >= 9 && boat.currentOrientation == "Horizontal")
                {
                    col = 9 - boat.size;
                }
                if (row + boat.size >= 9 && boat.currentOrientation == "Vertical")
                {
                    row = 9 - boat.size;
                }
            }

            public static bool CheckOverlappingBoats(Boat boat,Player player)
            {
                foreach (BoatParts part in boat.parts)
                {
                    foreach (Boat b in player.boats)
                    {
                        if (boat.boatName != b.boatName)
                        {
                            foreach (BoatParts p in b.parts)
                            {
                                if (p.rowPos == part.rowPos && p.colPos == part.colPos)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }

        public class BoatParts
        {
            public int colPos, rowPos;
            public bool damaged;
            public BoatParts(int colPos, int rowPos)
            {
                this.colPos = colPos;
                this.rowPos = rowPos;
                this.damaged = false;
            }
        }

        public class BoatTemplate
        {
            public string boatName;
            public string boatPath;
            public int boatSize;
            public Image boatImage;
        }

        public class PreviewWindow
        {
            public static int currentBoatVisualIndex = 0;

            public void SpinBoat(Canvas canvas, Boat currentPreviewBoat)
            {
                if (canvas.Children.Count > 0)
                {
                    canvas.Children.RemoveAt(0);
                    currentPreviewBoat.currentRotationAngle += 90;

                    if (currentPreviewBoat.currentRotationAngle < 0)
                    {
                        currentPreviewBoat.currentRotationAngle = 270;
                    }

                    if (currentPreviewBoat.currentRotationAngle > 360)
                    {
                        currentPreviewBoat.currentRotationAngle = 90;
                    }

                    MainWindow.Instance.CreateBoatImage(currentPreviewBoat.currentRotationAngle, currentPreviewBoat.boatName);
                }
            }

            public void CreateBoatImage(Canvas canvas, int rotationValue = 0, string boatName = "HugeBoat")
            {
                //boatName = MainWindow.Instance.currentPreviewBoat.boatName;
                TransformedBitmap transformBmp = new TransformedBitmap();
                BitmapImage bmpImage = new BitmapImage();

                bmpImage.BeginInit();

                bmpImage.UriSource = new Uri(MainWindow.projectDirectory + $"\\Images\\Boats\\{boatName}.png", UriKind.RelativeOrAbsolute);

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
                BodyImage.MouseMove += MainWindow.BodyImage_MouseMove;
                //mages.Add(BodyImage);

                if (rotationValue == 0 || rotationValue == 180)
                {
                    BodyImage.Width = 51;
                    BodyImage.Height = 153;

                    //FIXME : placeringen av båten, ligger lite off center nu
                    //Canvas.SetLeft(BodyImage, 80);
                    
                }
                else if (rotationValue == 90 || rotationValue == 270)
                {
                    BodyImage.Width = 153;
                    BodyImage.Height = 51;

                    //FIXME : placeringen av båten, ligger lite off center nu
                    //Canvas.SetLeft(BodyImage, 30);
                }

                //Canvas.SetLeft(BodyImage, 80);
                Canvas.SetTop(BodyImage, (MainWindow.Instance.ImageCanvas.Width / 2.0) - (BodyImage.Height / 2.0));
                int index = -1;
                for (int i = 0; i < MainWindow.Images.Count; i++)
                {
                    if (MainWindow.Images[i].Name == boatName)
                    {
                        index = i;
                    }
                }
                if (index != -1)
                {
                    MainWindow.Images[index] = BodyImage;
                }
                else { MainWindow.Images.Add(BodyImage); }

                canvas.Children.Add(BodyImage);

                Boat newBoat = new Boat(Boat.GetBoatTemplate(boatName));

                //FIXME: Depending on the name, get the id from a boat library

                newBoat.currentRotationAngle = rotationValue;

                if (newBoat.currentRotationAngle == 0 || newBoat.currentRotationAngle == 180)
                {
                    newBoat.currentOrientation = "Vertical";
                }
                else if (newBoat.currentRotationAngle == 90 || newBoat.currentRotationAngle == 270)
                {
                    newBoat.currentOrientation = "Horizontal";
                    
                }

                double xPos = (MainWindow.Instance.ImageCanvas.Width / 2.0) - (BodyImage.Width / 2.0);

                Canvas.SetLeft(BodyImage, xPos);

                MainWindow.Instance.currentPreviewBoat = newBoat;
            }
            public void RemoveBoatImage(Canvas canvas)
            {
                if(canvas.Children.Count > 0)
                {
                    canvas.Children.RemoveAt(0);
                }
            }

            public void NextButton_Click(Canvas ImageCanvas)
            {
                Classes.PreviewWindow.currentBoatVisualIndex++;

                //Checks, so index doesnt go out of bounds
                if (Classes.PreviewWindow.currentBoatVisualIndex > MainWindow.boatLibrary.Length - 1)
                {
                    Classes.PreviewWindow.currentBoatVisualIndex = 0;
                }

                if (Classes.PreviewWindow.currentBoatVisualIndex < 0)
                {
                    Classes.PreviewWindow.currentBoatVisualIndex = MainWindow.boatLibrary.Length - 1;
                }

                //Clean the boat visual on the canvas
                if (ImageCanvas.Children.Count > 0)
                {
                    ImageCanvas.Children.RemoveAt(0);
                }


                //Add the new boat visual to the canvas
                if (ImageCanvas.Children.Count == 0)
                {
                    CreateBoatImage(ImageCanvas, boatName: MainWindow.boatLibrary[PreviewWindow.currentBoatVisualIndex]);
                }
            }

            public void PreviousButton_Click(Canvas ImageCanvas)
            {
                Classes.PreviewWindow.currentBoatVisualIndex--;

                //Checks, so index doesnt go out of bounds
                if (Classes.PreviewWindow.currentBoatVisualIndex > MainWindow.boatLibrary.Length - 1)
                {
                    Classes.PreviewWindow.currentBoatVisualIndex = 0;
                }

                if (Classes.PreviewWindow.currentBoatVisualIndex < 0)
                {
                    Classes.PreviewWindow.currentBoatVisualIndex = MainWindow.boatLibrary.Length - 1;
                }

                //Clean the boat visual on the canvas
                if (ImageCanvas.Children.Count > 0)
                {
                    ImageCanvas.Children.RemoveAt(0);
                }


                //Add the new boat visual to the canvas
                if (ImageCanvas.Children.Count == 0)
                {
                    CreateBoatImage(ImageCanvas, boatName: MainWindow.boatLibrary[Classes.PreviewWindow.currentBoatVisualIndex]);
                }
            }

            public static void PopulateTemplates()
            {
                for (int i = 0; i < MainWindow.boatLibrary.Length; i++)
                {
                    BoatTemplate boatTemplate = new BoatTemplate() { boatName = MainWindow.boatLibrary[i] };

                    if (boatTemplate.boatName == "HugeBoat")
                    {
                        boatTemplate.boatSize = 4;
                    }
                    else if (boatTemplate.boatName == "BigBoat")
                    {
                        boatTemplate.boatSize = 3;
                    }
                    else if (boatTemplate.boatName == "MediumBoat1")
                    {
                        boatTemplate.boatSize = 2;
                    }
                    else if (boatTemplate.boatName == "MediumBoat2")
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

                    string boatName = MainWindow.boatLibrary[i];

                    bmpImage.UriSource = new Uri(MainWindow.projectDirectory + $"\\Images\\Boats\\{boatName}.png", UriKind.RelativeOrAbsolute);

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
                    MainWindow.MainPlayer.boats.Add(Boat.addBoatParts(new Classes.Boat(boatTemplate)));
                    MainWindow.Enemy.boats.Add(Boat.addBoatParts(new Classes.Boat(boatTemplate)));
                    MainWindow.boatTemplates.Add(boatTemplate);
                }
            }


        }

        public class Coordinates
        {
            public int row, col;
            public Coordinates(int row, int col)
            {
                this.row = row;
                this.col = col;
            }
        }
        public static bool Attack(Player player,int row,int col)
        {
            foreach(Boat b in player.boats)
            {
                if (b.destroyed == false)
                {
                    foreach (BoatParts parts in b.parts)
                    {
                        if (parts.rowPos == row && parts.colPos == col)
                        {
                            parts.damaged = true;
                            b.checkDamage();
                            if (b.damagedParts == b.size)
                            {
                                b.destroyed = true;
                                player.PlayerBoats -= 1;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
