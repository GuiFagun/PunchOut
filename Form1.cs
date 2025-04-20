using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;
using NAudio.Wave; // For sound effects that won't interrupt music
using Timer = System.Windows.Forms.Timer; // For timer.

namespace PunchOut
{
    public partial class Form1 : Form
    {
        bool blockAttack = false; // this is the player block boolean
        // below is the enemy attack lists
        List<string> enemyAttacks = new List<string> { "left", "right", "block" };
        // rnd is the new random class used to generate random numbers
        Random rnd = new Random();
        // below is the enemy speed variable
        int enemySpeed = 5;
        // below int i will be used to check the enemy moves
        int i = 0;
        bool enemyBlocked; // this is the enemy attack block boolean
        int playerHealth = 100; // players total health
        int enemyHealth = 100; // enemys total health

        // Those are related to the OnPaint method to fiz transparency, they basically work as invisible canvases where the sprites are prepared before showing.
        private Bitmap boxerBuffer;
        private Bitmap enemyBuffer;

        // Soundplayer for the soundtrack
        private SoundPlayer backgroundMusic;
        private SoundPlayer winSound;
        private SoundPlayer loseSound;

        // Checking when was the last punch and sets cooldown.
        private DateTime lastPunchTime = DateTime.MinValue;
        private int punchCooldownMs = 500; // Half-second delay between punches

        // Check if game is over (only here to prevent bugs, specially with the win/loss screens)
        private bool gameOver = false;

        // Timer and win/loss counters
        int victories = 0;
        int defeats = 0;
        int roundTimeLeft = 60;
        Timer roundTimer = new Timer();


        public Form1()
        {
            InitializeComponent();

            victoryScreen.Image = Properties.Resources.winscreen;
            defeatScreen.Image = Properties.Resources.lostscreen;

            roundTimeLeft = 60; // 1 minute duration for the round.
            roundTimer.Start();

            // Play background music
            Stream musicStream = new MemoryStream(Properties.Resources.soundtrack1);
            backgroundMusic = new SoundPlayer(musicStream);
            backgroundMusic.PlayLooping(); // Loop the music

            // Win and loss music
            winSound = new SoundPlayer(new MemoryStream(Properties.Resources.winsound));
            loseSound = new SoundPlayer(new MemoryStream(Properties.Resources.lostsound));

            // Create buffers for sprites only (Related to OnPain method)
            boxerBuffer = new Bitmap(boxer.Width, boxer.Height);
            enemyBuffer = new Bitmap(enemyBoxer.Width, enemyBoxer.Height);

            // Draws everything on hidden buffer, then instantly swap screen when ready (imagine like flipping to a new animation cel).
            // Without this we would experience flickering.
            this.DoubleBuffered = true;
        }

        // Timer code
        private void roundTimer_Tick(object sender, EventArgs e)
        {
            roundTimeLeft--; // Decrease the time left by 1 second each second.

            if (roundTimeLeft >= 0) // Whenever timer is greater than 0, it runs.
            {
                if (roundTimeLeft >= 10)
                {
                    roundTimerLabel.Text = $"00:{roundTimeLeft}";
                }
                else if (roundTimeLeft < 10) // Just to prevent going from 00:10 to 00:9, and instead shows 00:09
                {
                    roundTimerLabel.Text = $"00:0{roundTimeLeft}";
                }
            }

            //Checks if time ran out and who won based on remaining health. On a tie, enemy wins.
            if (roundTimeLeft == 0 && (enemyHealth > playerHealth || playerHealth == enemyHealth))
            {
                gameOver = true;
                enemyTimer.Stop();
                enemyMove.Stop();
                defeatScreen.Visible = true;
                backgroundMusic.Stop(); // Stop the background music
                loseSound.Play();
                defeats++;
                defeatLabel.Text = $"{defeats}";

                Task.Run(() =>
                {
                    MessageBox.Show("Damn that was tough! Don't retire! Try again!");
                    this.Invoke(new Action(resetGame));
                });
                return;
            }
            else if (roundTimeLeft == 0 && playerHealth > enemyHealth)
            {
                gameOver = true;
                enemyTimer.Stop();
                enemyMove.Stop();
                victoryScreen.Visible = true;
                backgroundMusic.Stop(); // Stop the background music
                winSound.Play();
                victories++;
                victoryLabel.Text = $"{victories}";

                Task.Run(() =>
                {
                    MessageBox.Show("Congrats Champ! Up for one more round?");
                    this.Invoke(new Action(resetGame));
                });
                return;
            }
        }

        /* Windows Forms still overlaps picture boxes, but instead of their boxes becoming transparent, they become the background image.  
        I won't lie in saying I definitely had to use AI to help me fix this as it was very hard and confusing trying to find a solution online, so take a leap of faith in this case, it worked.  
        Whatit does is that it basically overrides the default drawing behavior, first it draws the enemy boxer and then the player with proper transparency.  
        By drawing the sprites directly, it bypasses the limitations of PictureBox.  
        The way it runs is:
        1- Draws enemy ? buffer
        2- Draws player ? buffer with matrix filter applied
        3- Shows final composite image
        Then for every pixel in boxer that overlaps with ther enemy, we have "FinalColor = (BoxerColor × 0.99) + (EnemyColor × 0.01)", where 0.99 is the alpha value.
        */
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Let Windows handle all controls normally

            // Draw enemy first
            e.Graphics.DrawImage(enemyBoxer.Image, enemyBoxer.Left, enemyBoxer.Top,
                               enemyBoxer.Width, enemyBoxer.Height);

            // Draw player with proper transparency
            using (var attributes = new ImageAttributes())
            {
                var transparency = 1f; // Adjust if needed.
                var colorMatrix = new ColorMatrix(new float[][] {
                    new float[] {1, 0, 0, 0, 0}, // Red. Keeps red at 100%.
                    new float[] {0, 1, 0, 0, 0}, // Blue. Keeps green at 100%.
                    new float[] {0, 0, 1, 0, 0}, // Green. Keeps blue at 100%.
                    new float[] {0, 0, 0, transparency, 0}, // Alpha. Controls alpha transparency opacity.
                    new float[] {0, 0, 0, 0, 1} // Bright. Preserves brightness.
            });

                attributes.SetColorMatrix(colorMatrix);

                e.Graphics.DrawImage(
                    boxer.Image,
                    new Rectangle(boxer.Left, boxer.Top, boxer.Width, boxer.Height),
                    0, 0, boxer.Width, boxer.Height,
                    GraphicsUnit.Pixel,
                    attributes);
            }
        }

        private void EnemyMoveEvent(object sender, EventArgs e)
        {
            // Add some randomness to movement
            if (rnd.Next(0, 100) > 90) // 10% chance to change direction
            {
                enemySpeed = -enemySpeed; // Reverse direction
            }

            // Variable speed (between 3-7)
            if (rnd.Next(0, 100) > 80) // 20% chance to change speed
            {
                enemySpeed = rnd.Next(3, 8) * (enemySpeed > 0 ? 1 : -1);
            }

            enemyBoxer.Left += enemySpeed;

            // Keep enemy within bounds (315-480)
            if (enemyBoxer.Left > 480)
            {
                enemySpeed = -Math.Abs(enemySpeed); // Force left movement
            }
            if (enemyBoxer.Left < 315)
            {
                enemySpeed = Math.Abs(enemySpeed); // Force right movement
            }
            if (playerHealth >= 0)
            {
                // then we link it to the player life progress bar
                playerLife.Value = Convert.ToInt32(playerHealth);
            }
            // if the enemy health is greater than 1
            if (enemyHealth >= 0)
            {
                // then we link it to the enemy life progress bar
                enemyLife.Value = Convert.ToInt32(enemyHealth);
            }
            // if the enemy goes between 480 and 315
            // we will revserse the speed
            if (enemyBoxer.Left > 480)
            {
                // we move the enemy towards the left
                enemySpeed = -5;
            }
            // if the enemy left is less than 315 pixels
            if (enemyBoxer.Left < 315)
            {
                // we move the enemy towards the right
                enemySpeed = 5;
            }
            // if the enemy health is less than 1
            if (!gameOver)
            {
                if (enemyHealth <= 0)
                {
                    gameOver = true;
                    enemyTimer.Stop();
                    enemyMove.Stop();
                    victoryScreen.Visible = true;
                    backgroundMusic.Stop(); // Stop the background music
                    winSound.Play();
                    victories++;
                    victoryLabel.Text = $"{victories}";

                    Task.Run(() =>
                    {
                        MessageBox.Show("Congrats Champ! Up for one more round?");
                        this.Invoke(new Action(resetGame));
                    });
                    return;
                }

                if (playerHealth <= 0)
                {
                    gameOver = true;
                    enemyTimer.Stop();
                    enemyMove.Stop();
                    defeatScreen.Visible = true;
                    backgroundMusic.Stop(); // Stop the background music
                    loseSound.Play();
                    defeats++;
                    defeatLabel.Text = $"{defeats}";

                    Task.Run(() =>
                    {
                        MessageBox.Show("Damn that was tough! Don't retire! Try again!");
                        this.Invoke(new Action(resetGame));
                    });
                    return;
                }
            }

            Invalidate(); // Forces redraw

        }

        private void EnemyPunchEvent(object sender, EventArgs e)
        {
            // Generate a random number and store in i
            // The random number will need to be between 0 and the number of attacks in the attacks list
            i = rnd.Next(0, enemyAttacks.Count);

            // Below is the switch statement that will check which action the enemy is taking
            switch (enemyAttacks[i].ToString())
            {
                // If the attack is left
                case "left":
                    // Change the enemy to the punch 1 image
                    enemyBoxer.Image = Properties.Resources.enemy_punch1;
                    // Check if the player and enemy are colliding and the blocking is set to false
                    if (enemyBoxer.Bounds.IntersectsWith(boxer.Bounds))
                    {
                        // Reduce 20 from the player's health
                        if (!blockAttack)
                        {
                            playerHealth -= 20;
                            PlayGetHitSound();
                        }
                        else
                        {
                            PlayBlockSound();
                        }
                    }
                    enemyBlocked = false; // Set the blocking to false
                    break;

                // If the attack is right
                case "right":
                    // Change the enemy to the punch 2 image
                    enemyBoxer.Image = Properties.Resources.enemy_punch2;
                    // Check if the player and enemy are colliding and the blocking is set to false
                    if (enemyBoxer.Bounds.IntersectsWith(boxer.Bounds))
                    {
                        // Reduce 20 from the player's health
                        if (!blockAttack)
                        {
                            playerHealth -= 20;
                            PlayGetHitSound();
                        }
                        else
                        {
                            PlayBlockSound();
                        }
                    }
                    enemyBlocked = false; // Set the blocking to false
                    break;

                // If the attack is block
                case "block":
                    // Change the enemy picture to block
                    enemyBoxer.Image = Properties.Resources.enemy_block;
                    // Change the boolean to true
                    enemyBlocked = true;
                    break;
            }

            // After any action, return to idle after a short delay
            Task.Delay(250).ContinueWith(_ =>
            {
                this.Invoke((Action)(() =>
                {
                    enemyBoxer.Image = Properties.Resources.enemy_stand;
                }));

                Invalidate(); // Forces redraw
            });
        }

        // This is the key down event, whenever X key is down, some event will trigger, simple enough.
        private void keyisdown(object sender, KeyEventArgs e)
        {
            // if the player hits the down key
            if (e.KeyCode == Keys.Down)
            {
                // we change the player image to the block image
                boxer.Image = Properties.Resources.boxer_block;
                blockAttack = true; // change the block attack boolean to true
            }

            // Here are the punches, see that the conditional checks for the cooldown time and subtracts from the time at the moment, to alter the cooldown check the variable on the top of the class.
            if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) && DateTime.Now.Subtract(lastPunchTime).TotalMilliseconds >= punchCooldownMs)
            {
                lastPunchTime = DateTime.Now;

                if (e.KeyCode == Keys.Left)
                {
                    boxer.Image = Properties.Resources.boxer_left_punch;

                    if (enemyBoxer.Bounds.IntersectsWith(boxer.Bounds) && !enemyBlocked)
                    {
                        enemyHealth -= 5;
                        PlayPunchSound();
                    }
                    else
                    {
                        PlayMissPunchSound();
                    }
                }
                else if (e.KeyCode == Keys.Right)
                {
                    boxer.Image = Properties.Resources.boxer_right_punch;

                    if (enemyBoxer.Bounds.IntersectsWith(boxer.Bounds) && !enemyBlocked)
                    {
                        enemyHealth -= 5;
                        PlayPunchSound();
                    }
                    else
                    {
                        PlayMissPunchSound();
                    }
                }
            }
        }

        // Method to return boxer sprite to idle whenever you're not pressing a key. Yes this does mean you can hold down the key and it will keep punching, but try doing that and see how well it goes for you.
        private void keyisup(object sender, KeyEventArgs e)
        {
            // when the player will release the keys they press this event will trigger
            boxer.Image = Properties.Resources.boxer_stand; // we will reset player image back to stand
            blockAttack = false; // change the block attack boolean back to false
        }

        // Game reset method
        private void resetGame()
        {

            enemyTimer.Stop();   // Always stop first
            enemyMove.Stop();
            roundTimeLeft = 60;
            enemyBoxer.Left = 346;
            enemyBoxer.Top = 193;
            enemyBoxer.Image = Properties.Resources.enemy_stand;
            boxer.Image = Properties.Resources.boxer_stand;

            playerHealth = 100;
            enemyHealth = 100;

            // Hide the win/loss screens
            victoryScreen.Visible = false;
            defeatScreen.Visible = false;

            gameOver = false;

            // Starts sooundtrack again
            backgroundMusic.PlayLooping();

            enemyTimer.Start();  // Then restart cleanly
            enemyMove.Start();
        }

        // Down here we have some sound effects, simples stuff, used NAudio for this.
        private void PlayPunchSound()
        {
            var punchSound = new MemoryStream(Properties.Resources.punchsound);
            var waveOut = new WaveOutEvent();
            var reader = new WaveFileReader(punchSound);
            waveOut.Init(reader);
            waveOut.Play();

            // Dispose when done
            waveOut.PlaybackStopped += (s, a) =>
            {
                waveOut.Dispose();
                reader.Dispose();
            };
        }
        private void PlayMissPunchSound()
        {
            var missSound = new MemoryStream(Properties.Resources.misspunchsound);
            var waveOut = new WaveOutEvent();
            var reader = new WaveFileReader(missSound);
            waveOut.Init(reader);
            waveOut.Play();

            waveOut.PlaybackStopped += (s, a) =>
            {
                waveOut.Dispose();
                reader.Dispose();
            };
        }
        private void PlayBlockSound()
        {
            var blockSound = new MemoryStream(Properties.Resources.blocksound);
            var waveOut = new WaveOutEvent();
            var reader = new WaveFileReader(blockSound);
            waveOut.Init(reader);
            waveOut.Play();

            waveOut.PlaybackStopped += (s, a) =>
            {
                waveOut.Dispose();
                reader.Dispose();
            };
        }
        private void PlayGetHitSound()
        {
            var gethitSound = new MemoryStream(Properties.Resources.gethitsound);
            var waveOut = new WaveOutEvent();
            var reader = new WaveFileReader(gethitSound);
            waveOut.Init(reader);
            waveOut.Play();

            waveOut.PlaybackStopped += (s, a) =>
            {
                waveOut.Dispose();
                reader.Dispose();
            };
        }
    }
}

/*
Comment on game over logic:

gameOver = true; // Set game over flag true, as said before, prevent bugs guaranteeing the game is over.
enemyTimer.Stop(); 
enemyMove.Stop(); // Both those stops end the actions/timers/events of the enemy and the player, including health bars.
victoryScreen.Visible = true; // Shows the screen of either win or loss.
backgroundMusic.Stop(); // Stop the background music.
winSound.Play(); // Play music for either win or loss.
victories++; // Increment counter for either win or loss.
victoryLabel.Text = $"{victories}";

// This is the way I found to solve a bug where when the game restarted, player and enemy became immortal and when the bar reached 0, game didn't end.
// This pulls the message box and the reset game through a callback instead of IN the enemyMoveEvent or roundTimer method, ensuring everything is properly reset.

Task.Run(() =>
{
    MessageBox.Show("Congrats Champ! Up for one more round?");
    this.Invoke(new Action(resetGame));
});
return;
*/