using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TheGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        
        //Global init
        double xloc = 0;
        double yloc = 0;
        
        Rectangle[] aisquares;
        int[] directions;
        Random globalSeed;
        Timer aiClock;
        Timer collisionTimer;
        int numberOfOpponents = 50;
        Timer timerUp;
        Timer timerDown;
        Timer timerRight;
        Timer timerLeft;
        
        double speed;
        string path = "./Scores2.txt";
        bool lose;

        int highScoreEasy;
        int highScoreMedium;
        int highScoreHard;

        int playerScore;
        int playerSize;

        
        //Application launch
        public MainWindow()
        {
            InitializeComponent();
            MainMenu.Visibility = Visibility.Visible;
            
            
            //Highscore file management

           
            if (!File.Exists(path))
            {
                File.Create(path);
                TextWriter tw = new StreamWriter(path);
                tw.WriteLine("0");
                tw.WriteLine("0");
                tw.WriteLine("0");
                tw.Close();
            }
            else if (File.Exists(path))
            {
                string[] scores = System.IO.File.ReadAllLines(path);

                try
                {
                    highScoreEasy = Convert.ToInt32(scores[0]);
                    highScoreMedium = Convert.ToInt32(scores[1]);
                    highScoreHard = Convert.ToInt32(scores[2]);

                }
                    catch(Exception)
                {

                }
                

            }
             //Add loaded highscores to menu labels

            lblEasy.Content = highScoreEasy + "";
            lblMedium.Content = highScoreMedium + "";
            lblHard.Content = highScoreHard + "";

            //Gloabl random seed to prevent clock generation issues
            globalSeed = new Random();

            //Create timers for ai movement and for player movement
            //Timers added to input to allow multiple keypresses at once
            aiClock = new Timer(1);
            aiClock.Elapsed += aiClock_Elapsed;
            collisionTimer = new Timer(100);
            collisionTimer.Elapsed += collisionDetection;
            timerDown = new Timer(1);
            timerDown.Elapsed += timerDown_Elapsed;
            timerLeft = new Timer(1);
            timerLeft.Elapsed += timerLeft_Elapsed;
            timerRight = new Timer(1);
            timerRight.Elapsed += timerRight_Elapsed;
            timerUp = new Timer(1);
            timerUp.Elapsed += timerUp_Elapsed;
            

           

            

            
        }

        //AI Movement
        protected void aiClock_Elapsed(Object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                lblScore.Text = playerScore + ""; //change player score

                //losing state
                if(lose == true)
                {
                    
                    //Hide the player, show the menu
                    MainMenu.Visibility = Visibility.Visible;
                    rectPlayer.Visibility = Visibility.Collapsed;
                    lblScore.Visibility = Visibility.Collapsed;

                    
                    //Hide the ai squares
                    for (int i = 0; i < aisquares.Length; i++)
                    {
                        aisquares[i].Visibility = Visibility.Collapsed;
                    }

                    aiClock.Enabled = false;//stop AI movement
                    collisionTimer.Enabled = false; //stop collision detection
                    
                }
                else
                {
                    rectPlayer.Width = playerSize;
                    rectPlayer.Height = playerSize;
                }

                //Ai movement logic
                //Direction passed to TargetSquare class when generation occurs
                //Allows each AI square to be assigned a random direction
                int xChange = 0;
                int yChange = 0;
                for (int i = 0; i < aisquares.Length; i++)
                {
                    

                    if(directions[i] == 0)
                    {
                        xChange = -1;
                        yChange = 0;
                    }
                    if (directions[i] == 1)
                    {
                        xChange = -1;
                        yChange = -1;
                    }
                    if (directions[i] == 2)
                    {
                        xChange = 0;
                        yChange = -1;
                    }
                    if (directions[i] == 3)
                    {
                        xChange = 1;
                        yChange = -1;
                    }
                    if (directions[i] == 4)
                    {
                        xChange = 1;
                        yChange = 0;
                    }
                    if (directions[i] == 5)
                    {
                        xChange = 1;
                        yChange = 1;
                    }
                    if (directions[i] == 6)
                    {
                        xChange = 0;
                        yChange = 1;
                    }
                    if (directions[i] == 7)
                    {
                        xChange = -1;
                        yChange = 1;
                    }
                    
                    double oldX = aisquares[i].Margin.Left;
                    double newX = aisquares[i].Margin.Left + xChange;

                    double oldY = aisquares[i].Margin.Top;
                    double newY = aisquares[i].Margin.Top + yChange;

                    //Wall collision handling - If AI sqaare is going off any edge, flip its direction
                    //Creates a bounce effect
                    if(newX < -475)
                    {
                        if(directions[i] == 0)
                        {
                            directions[i] = 4;
                        }

                        if (directions[i] == 1)
                        {
                            directions[i] = 3;
                        }

                        if (directions[i] == 7)
                        {
                            directions[i] = 5;
                        }


                    }

                    if (newX > 475)
                    {
                        if (directions[i] == 3)
                        {
                            directions[i] = 1;
                        }

                        if (directions[i] == 4)
                        {
                            directions[i] = 0;
                        }

                        if (directions[i] == 5)
                        {
                            directions[i] = 7;
                        }


                    }

                    if (newY < -450)
                    {
                        if (directions[i] == 1)
                        {
                            directions[i] = 7;
                        }

                        if (directions[i] == 2)
                        {
                            directions[i] = 6;
                        }

                        if (directions[i] == 3)
                        {
                            directions[i] = 5;
                        }


                    }

                    if (newY > 450)
                    {
                        if (directions[i] == 5)
                        {
                            directions[i] = 3;
                        }

                        if (directions[i] == 6)
                        {
                            directions[i] = 2;
                        }

                        if (directions[i] == 7)
                        {
                            directions[i] = 1;
                        }
                    }
                    //Apply new direction to the sqaure
                    aisquares[i].Margin = new Thickness(aisquares[i].Margin.Left + xChange, aisquares[i].Margin.Top + yChange, 0, 0);
                }
            }));
        }

        //Collision Detection
        protected void collisionDetection(Object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                
                

                for(int i = 0; i < aisquares.Length; i++)
                {
                    //get player current dimensions and position
                    double playerRight = rectPlayer.Margin.Left + (playerSize);
                    double playerBottom = rectPlayer.Margin.Top + (playerSize);
                    double playerLeft = rectPlayer.Margin.Left - (playerSize);
                    double playerTop = rectPlayer.Margin.Top - (playerSize);

                    //get target square dimensions and position
                    double badTop = aisquares[i].Margin.Top - aisquares[i].ActualHeight;
                    double badBottom = aisquares[i].Margin.Top + aisquares[i].ActualHeight;
                    double badLeft = aisquares[i].Margin.Left- aisquares[i].ActualWidth;
                    double badRight = aisquares[i].Margin.Left + aisquares[i].ActualWidth;

                    
                    
                    //Test for collsion using above dimensions
                    if (badRight >= playerLeft &&
                        badLeft <= playerRight &&
                        badBottom >= playerTop &&
                        badTop <= playerBottom)
                    {//successful collision
                        if(i < 24)//Green square locations in array
                        {
                            //play good sound
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"c:\Windows\Media\chimes.wav");
                            player.Play();
                            //update score, size and speed
                            playerScore += 1;
                            playerSize += 1;
                            speed += 0.5;
                            
                        }
                        else//A red sqaure has been hit
                        {   
                            //Show game over, play bad sound and record new high score if achieved
                            gridGameOver.Visibility = Visibility.Visible;
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"c:\Windows\Media\Windows Critical Stop.wav");
                            player.Play();

                            //save new high score to file depending on what difficultty was used
                            //Easy
                            if(numberOfOpponents == 30 && playerScore > highScoreEasy )
                            {
                                gridGameOver.Visibility = Visibility.Collapsed;
                                gridHighScore.Visibility = Visibility.Visible;
                                highScoreEasy = playerScore;
                               
                                TextWriter tw = new StreamWriter(path);
                                tw.WriteLine(highScoreEasy+"");
                                tw.WriteLine(highScoreMedium+"");
                                tw.WriteLine(highScoreHard+"");
                                tw.Close();

                            }
                            //Medium
                            if (numberOfOpponents == 75 && playerScore > highScoreMedium)
                            {
                                gridGameOver.Visibility = Visibility.Collapsed;
                                gridHighScore.Visibility = Visibility.Visible;
                                highScoreMedium = playerScore;

                                TextWriter tw = new StreamWriter(path);
                                tw.WriteLine(highScoreEasy + "");
                                tw.WriteLine(highScoreMedium + "");
                                tw.WriteLine(highScoreHard + "");
                                tw.Close();
                            }
                            //Hard
                            if (numberOfOpponents == 100 && playerScore > highScoreHard)
                            {
                                gridGameOver.Visibility = Visibility.Collapsed;
                                gridHighScore.Visibility = Visibility.Visible;
                                highScoreHard = playerScore;

                                TextWriter tw = new StreamWriter(path);
                                tw.WriteLine(highScoreEasy + "");
                                tw.WriteLine(highScoreMedium + "");
                                tw.WriteLine(highScoreHard + "");
                                tw.Close();
                            }


                            lblEasy.Content = highScoreEasy + "";
                            lblMedium.Content = highScoreMedium + "";
                            lblHard.Content = highScoreHard + "";
                            
                            lose = true;
                        }
                        

                        //respawn the square that was hit
                        int rndX = 0;
                        int rndY = 0;
                        //While used to make sure respawned sqaures are away from current player position to avoid instant collision
                        while (rndX < rectPlayer.Margin.Left + 50 + playerSize && rndX > rectPlayer.Margin.Left -50 - playerSize)
                        { 
                            rndX = globalSeed.Next(-475, 475);
                        }
                        while (rndY < rectPlayer.Margin.Top + 50 + playerSize && rndY > rectPlayer.Margin.Top - 50 - playerSize)
                        {
                            rndY = globalSeed.Next(-450, 450);
                        }
                        aisquares[i].Margin = new Thickness(rndX, rndY, 0, 0);
                    }
                }
            }));
        }
        
        /// <summary>
        /// Game start function
        /// </summary>
        private void startGame()
        {
            if (aisquares == null)
            {
                numberOfOpponents = 50;
                directions = new int[numberOfOpponents];//Make and fill array of random direction modifiers to apply to targets
                aisquares = new Rectangle[numberOfOpponents];//Make and fill array of target squares
            }
            
            xloc = 0;
            yloc = 0;
            
            //Fill array of target squares
            for (int i = 0; i < aisquares.Length; i++)
            {
                bool good = false;
                if (i < 24)
                {
                    good = true;
                }
                TargetSquare aitoadd = new TargetSquare(globalSeed, good);//Create new target object
                Rectangle fromtarget = aitoadd.makeTheShape();//Get reutrned rectangle from target
                aisquares[i] = fromtarget;//Add the target to the traget array
                mainGrid.Children.Add(aisquares[i]);//Add the target to the grid
                aisquares[i].Visibility = Visibility.Collapsed;
                directions[i] = globalSeed.Next(0, 8);//Add random direction modifier to square using global seed
            }

           //hide menu and display the game
            rectPlayer.Margin = new Thickness(xloc, yloc, 0, 0);
            lblScore.Margin = new Thickness(xloc, yloc, 0, 0);

            MainMenu.Visibility = Visibility.Collapsed;
            rectPlayer.Visibility = Visibility.Visible;
            lblScore.Visibility = Visibility.Visible;
            playerScore = 0;
            playerSize = 20;
            speed = 2;
            lose = false;
            //Show the targets
            for (int i = 0; i < aisquares.Length; i++)
            {
                aisquares[i].Visibility = Visibility.Visible;
            }

            
            aiClock.Enabled = true;//start AI movement
            collisionTimer.Enabled = true; //start collision detection
        }

       
      
        //Exit button clicked
        private void btnExitGame_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //Instructions button clicked
        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            gridInstructions.Visibility = Visibility.Visible;
        }

        //Difficulty buttons
        private void diffEasy_Click(object sender, RoutedEventArgs e)
        {
            numberOfOpponents = 30;
            directions = new int[numberOfOpponents];
            aisquares = new Rectangle[numberOfOpponents];
            startGame();
        }

        private void diffMedium_Click(object sender, RoutedEventArgs e)
        {
            numberOfOpponents = 75;
            directions = new int[numberOfOpponents];
            aisquares = new Rectangle[numberOfOpponents];
            startGame();
        }

        private void diffHard_Click(object sender, RoutedEventArgs e)
        {
            numberOfOpponents = 100;
            directions = new int[numberOfOpponents];
            aisquares = new Rectangle[numberOfOpponents];
            startGame();
        }

        //Close button for instruction panel
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            gridInstructions.Visibility = Visibility.Collapsed;
        }
        //Keyboard interface
        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                timerDown.Enabled = true;
            }
            if (e.Key == Key.Up)
            {
                timerUp.Enabled = true;
            }
            if (e.Key == Key.Right)
            {
                timerRight.Enabled = true;
            }
            if (e.Key == Key.Left)
            {
                timerLeft.Enabled = true;
            }
            if (e.Key == Key.Enter)
            {
                gridHighScore.Visibility = Visibility.Collapsed;
                gridGameOver.Visibility = Visibility.Collapsed;
                
            }
            //if game is NOT running and space is pressed start the game
            //Used for quick restart
            if(e.Key == Key.Space && aiClock.Enabled == false)
            {
                gridHighScore.Visibility = Visibility.Collapsed;
                gridGameOver.Visibility = Visibility.Collapsed;
                startGame();
            }

        }

        //Stop key timers when key is released
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                timerDown.Enabled = false;
            }
            if (e.Key == Key.Up)
            {
                timerUp.Enabled = false;
            }
            if (e.Key == Key.Right)
            {
                timerRight.Enabled = false;
            }
            if (e.Key == Key.Left)
            {
                timerLeft.Enabled = false;
            }
        }

        //Move down
        protected void timerDown_Elapsed(Object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (yloc < mainGrid.ActualHeight - rectPlayer.ActualHeight)
                {
                    yloc += speed;
                }
                rectPlayer.Margin = new Thickness(xloc, yloc, 0, 0);
                lblScore.Margin = new Thickness(xloc, yloc, 0, 0);
            }));
        }

        //Move Left
        protected void timerLeft_Elapsed(Object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (xloc > - mainGrid.ActualWidth + rectPlayer.ActualWidth)
                {
                    xloc -= speed;
                }
                rectPlayer.Margin = new Thickness(xloc, yloc, 0, 0);
                lblScore.Margin = new Thickness(xloc, yloc, 0, 0);
            }));
        }
        //Move UP
        protected void timerUp_Elapsed(Object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (yloc > - mainGrid.ActualHeight + rectPlayer.ActualHeight)
                {
                    yloc -= speed;
                }
                rectPlayer.Margin = new Thickness(xloc, yloc, 0, 0);
                lblScore.Margin = new Thickness(xloc, yloc, 0, 0);
            }));
        }
        //Move right
        protected void timerRight_Elapsed(Object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                if (xloc < mainGrid.ActualWidth - rectPlayer.ActualWidth)
                {
                    xloc += speed;
                }
                rectPlayer.Margin = new Thickness(xloc, yloc, 0, 0);
                lblScore.Margin = new Thickness(xloc, yloc, 0, 0);
            }));
        }


        

       
        //Class to generate target squares
        //IF passed bool is true a GREEN square is returned, if false a RED sqaure is returned
        public class TargetSquare
        {
           
            Rectangle targetSquare;
            public TargetSquare(Random seed, bool good)
            {
                
                targetSquare = new Rectangle();
                int rndX = 0;
                int rndY = 0;
                while (rndX < 100 && rndX > -100)
                { 
                    rndX = seed.Next(-475, 475);
                }
                while (rndY < 100 && rndY > -100)
                {
                    rndY = seed.Next(-450, 450);
                }
                
                targetSquare = new Rectangle();
                targetSquare.Height = 10;
                targetSquare.Width = 10;
                targetSquare.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                targetSquare.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                targetSquare.Margin = new Thickness(rndX, rndY, 0, 0);
                if (good)//set colour green
                {
                    Color myColor = new Color();
                    myColor = Color.FromRgb(0, 255, 0);
                    SolidColorBrush brush = new SolidColorBrush(myColor);
                    targetSquare.Fill = brush;
                }
                else{//set colour red
                    Color myColor = new Color();
                    myColor = Color.FromRgb(255, 0, 0);
                    SolidColorBrush brush = new SolidColorBrush(myColor);
                    targetSquare.Fill = brush;
                }
            }
            
            //function to make target square then return it
            public Rectangle makeTheShape()
            {
                return targetSquare;
            }
        }
    }
}
