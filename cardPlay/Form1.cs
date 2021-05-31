using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace cardPlay
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region ConstantVariables
        //in order to be able to catch clicked Pictureboxes and to compare them eachothers, these variables has been defined as global variables. 
        PictureBox clicked;
        PictureBox clicked2;
        int clicked2Tag;
        int clickedTag;
        bool showImage1 = false;
        bool showImage2 = false;
        int time = 100;
        int score = 0;
        int level = 1;
        
        
        


        #endregion

        #region OperationalMethods
        // for getting dynamic directory path 
        public string getDirectoryPath()
        {
            string fullCurrentDirectory = Directory.GetCurrentDirectory();
            string trimmedDirectoryPath = fullCurrentDirectory.Replace(@"\", "/");
            return trimmedDirectoryPath;

        }

        /// <summary>
        /// gamecard initializer  
        /// </summary>
        /// <param name="isTop">boolean parameter for defining flowlayout panel where the pictures will be placed </param>
        /// <param name="numberOfCard">integer that define the card number (defined as default 10) </param>
        public void generateCard(bool isTop, int numberOfCard = 10)
        {

            string currentDirectory = getDirectoryPath();
            Random rdm = new Random();
            //for preventing reiterated numbers, a list has been created.
            List<int> list = new List<int>();
            for (int i = 1; i <= numberOfCard; i++)
            {
                int rand = rdm.Next(1, numberOfCard + 1);

                PictureBox pict = new PictureBox();
                pict.Width = 110;
                pict.Height = 150;
                pict.BorderStyle = BorderStyle.Fixed3D;
                while (list.Contains(rand))
                    rand = rdm.Next(1, 11);
                pict.BackgroundImage = Image.FromFile($"{currentDirectory}/images/{rand}.jpg");
                pict.BackgroundImageLayout = ImageLayout.Stretch;
                pict.Image = Image.FromFile($"{currentDirectory}/images/background.jpg");
                pict.SizeMode = PictureBoxSizeMode.StretchImage;
                pict.Tag = new { tag = rand };

                //
                if (isTop)
                {
                    flwPictures.Controls.Add(pict);
                    list.Add(rand);
                    pict.Click += Pict_Click;
                }
                else
                {
                    flwPictures2.Controls.Add(pict);
                    list.Add(rand);
                    pict.Click += Pict2_Click;
                }
            }
            
        }
        /// <summary>
        /// in case of game over, this dialog box asks the player reload the game
        /// </summary>
        /// <param name="message">the parameter that convey the message about game ending (time over, win or lose)</param>
        public void gameOver(string message)
        {
            timer1.Stop();
            flwPictures.Enabled = false;
            flwPictures2.Enabled = false;
            MessageBox.Show(message);
            bool playAgain = GetMessageResult("Would you like to Play Again?", "Game");

            if (playAgain)
            {
                startTheGame();
            }
        }
        public void startTheGame()
        {
            flwPictures.Controls.Clear();
            flwPictures2.Controls.Clear();
            generateCard(true);
            generateCard(false);
            time = 100;
            lblTime.Text = time.ToString();
            score = 0;
            lblscore.Text = score.ToString();
            level = 1;
            lblLevel.Text = level.ToString();
            flwPictures.Enabled = true;
            flwPictures2.Enabled = true;
            clicked = null;
            clicked2 = null;
            showAllImages();
            timer3.Start();
            timer1.Start();

        }

        public void showAllImages()
        {
            foreach (PictureBox item in flwPictures.Controls)
                item.Image = null;
            foreach (PictureBox item2 in flwPictures2.Controls)
                item2.Image = null;

        }
        public void hideAllImages()
        {
            string currentDirectory = getDirectoryPath();
            foreach (PictureBox item in flwPictures.Controls)
                item.Image = Image.FromFile($"{currentDirectory}/images/background.jpg"); ;
            foreach (PictureBox item2 in flwPictures2.Controls)
                item2.Image = Image.FromFile($"{currentDirectory}/images/background.jpg"); ;

        }
        public void levelUp()
        {
            MessageBox.Show($"{level + 1}. Level");
            flwPictures.Controls.Clear();
            flwPictures2.Controls.Clear();
            generateCard(true);
            generateCard(false);
            level++;
            lblLevel.Text = level.ToString();
            time = 100 - (level * 5);
            lblTime.Text = time.ToString();
            flwPictures.Enabled = true;
            flwPictures2.Enabled = true;
            clicked = null;
            clicked2 = null;
            showAllImages();
            timer3.Start();
            timer1.Start();
            if (level == 5)
                gameOver("congratulation, we have winner!!! ");
        }

        /// <summary>
        /// to compare the clicked pictures, to increase the score in case of matching pictures, to understand which flowlayout panel's picturebox clicked first  
        /// </summary>
        /// <param name="currentImage">home image for the click event that call this method  (exmp: click1, showImage1) </param>
        /// <param name="otherImage">second image for the click event (example: click1, showImage2) </param>
        public void compareImages(bool currentImage, bool otherImage)
        {
            if (otherImage)
            {
                if (clickedTag == clicked2Tag)
                {
                    clicked.Image = null;
                    clicked2.Image = null;
                    flwPictures.Enabled = true;
                    flwPictures2.Enabled = true;
                    score += 50;
                    lblscore.Text = score.ToString();
                    timer2.Stop();
                    if (score != 0 && score % 500 == 0)
                        levelUp();
                }
                else
                    timer2.Start();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">the question</param>
        /// <param name="caption"></param>
        /// <returns></returns>
        bool GetMessageResult(string message, string caption)
        {

            DialogResult dialogResult = MessageBox.Show(message, caption, MessageBoxButtons.YesNo);

            return dialogResult == DialogResult.Yes ? true : false;
        }
        #endregion

        #region Events

        private void Form1_Load(object sender, EventArgs e)
        {
            
            generateCard(true);
            generateCard(false);
            timer1.Stop();
            
        }

        #region ClickEvents
        private void btnStart_Click_1(object sender, EventArgs e)
        {
            startTheGame();
        }

        private void Pict2_Click(object sender, EventArgs e)
        {
            clicked2 = (PictureBox)sender;
            clicked2.Image = null;
            showImage2 = true;
            flwPictures2.Enabled = false;
            clicked2Tag = (int)clicked2.Tag.GetType().GetProperty("tag").GetValue(clicked2.Tag);
            compareImages(showImage2, showImage1);
            
        }

        private void Pict_Click(object sender, EventArgs e)
        {
            clicked = (PictureBox)sender;
            clicked.Image = null;
            showImage1 = true;
            flwPictures.Enabled = false;
            clickedTag = (int)clicked.Tag.GetType().GetProperty("tag").GetValue(clicked.Tag);
            compareImages(showImage1, showImage2);

        }

        #endregion

        #region TimerEvents
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            time--;
            lblTime.Text = time.ToString();
            if (time == 0)
                gameOver("Uzgunum, kaybettiniz");
            
        }
        private void timer2_Tick_1(object sender, EventArgs e)
        {
            string currentDirectory = getDirectoryPath();
            if(clicked != null && clicked2 != null)
            {
                clicked.Image = Image.FromFile($"{currentDirectory}/images/background.jpg");
                clicked2.Image = Image.FromFile($"{currentDirectory}/images/background.jpg");
            }
            
            showImage1 = false;
            showImage2 = false;
            flwPictures.Enabled = true;
            flwPictures2.Enabled = true;
            timer2.Stop();

        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            hideAllImages();
            timer3.Stop();
        }


        #endregion

        #endregion


    }
}
