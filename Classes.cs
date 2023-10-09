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
                this.players = new List<int>();
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
                TransformedBitmap transformBmp = new TransformedBitmap();
                BitmapImage bmpImage = new BitmapImage();
                bmpImage.BeginInit();
                bmpImage.UriSource = new Uri(MainWindow.projectDirectory + $"\\Images\\titlescreenpicture.jpeg", UriKind.RelativeOrAbsolute);
                bmpImage.EndInit();
                Image BodyImage = new Image
                {
                    Width = 1100,
                    Height = 500,
                    Source = bmpImage
                };
                TitleCanvas.Children.Add(BodyImage);
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

                Boat newBoat = new Boat(MainWindow.Instance.GetBoatTemplate(boatName));

                //FIXME: Depending on the name, get the id from a boat library

                newBoat.currentRotationAngle = rotationValue;

                if (newBoat.currentRotationAngle == 0 || newBoat.currentRotationAngle == 180)
                {
                    newBoat.currentOrientation = "Vertical";
                    //Canvas.SetLeft(BodyImage, 80);
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
        public bool AIattack()
        {
            return false;
        }
    }
}
