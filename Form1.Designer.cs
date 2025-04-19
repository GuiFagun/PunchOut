namespace PunchOut
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            enemyLife = new ProgressBar();
            playerLife = new ProgressBar();
            enemyBoxer = new PictureBox();
            boxer = new PictureBox();
            enemyTimer = new System.Windows.Forms.Timer(components);
            enemyMove = new System.Windows.Forms.Timer(components);
            victoryScreen = new PictureBox();
            defeatScreen = new PictureBox();
            victoryLabel = new Label();
            defeatLabel = new Label();
            roundTimerLabel = new Label();
            roundTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)enemyBoxer).BeginInit();
            ((System.ComponentModel.ISupportInitialize)boxer).BeginInit();
            ((System.ComponentModel.ISupportInitialize)victoryScreen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)defeatScreen).BeginInit();
            SuspendLayout();
            // 
            // enemyLife
            // 
            enemyLife.Location = new Point(287, 48);
            enemyLife.Name = "enemyLife";
            enemyLife.Size = new Size(156, 20);
            enemyLife.TabIndex = 0;
            // 
            // playerLife
            // 
            playerLife.Location = new Point(468, 48);
            playerLife.Name = "playerLife";
            playerLife.Size = new Size(156, 20);
            playerLife.TabIndex = 1;
            // 
            // enemyBoxer
            // 
            enemyBoxer.BackColor = Color.Transparent;
            enemyBoxer.Image = Properties.Resources.enemy_stand;
            enemyBoxer.Location = new Point(385, 297);
            enemyBoxer.Name = "enemyBoxer";
            enemyBoxer.Size = new Size(77, 185);
            enemyBoxer.SizeMode = PictureBoxSizeMode.AutoSize;
            enemyBoxer.TabIndex = 2;
            enemyBoxer.TabStop = false;
            // 
            // boxer
            // 
            boxer.BackColor = Color.Transparent;
            boxer.Image = Properties.Resources.boxer_stand;
            boxer.Location = new Point(401, 367);
            boxer.Name = "boxer";
            boxer.Size = new Size(61, 153);
            boxer.SizeMode = PictureBoxSizeMode.AutoSize;
            boxer.TabIndex = 3;
            boxer.TabStop = false;
            // 
            // enemyTimer
            // 
            enemyTimer.Enabled = true;
            enemyTimer.Interval = 1000;
            enemyTimer.Tick += EnemyPunchEvent;
            // 
            // enemyMove
            // 
            enemyMove.Enabled = true;
            enemyMove.Interval = 20;
            enemyMove.Tick += EnemyMoveEvent;
            // 
            // victoryScreen
            // 
            victoryScreen.Image = Properties.Resources.winscreen;
            victoryScreen.Location = new Point(0, 0);
            victoryScreen.Name = "victoryScreen";
            victoryScreen.Size = new Size(841, 604);
            victoryScreen.SizeMode = PictureBoxSizeMode.AutoSize;
            victoryScreen.TabIndex = 7;
            victoryScreen.TabStop = false;
            victoryScreen.Visible = false;
            // 
            // defeatScreen
            // 
            defeatScreen.Image = Properties.Resources.lostscreen;
            defeatScreen.Location = new Point(0, 0);
            defeatScreen.Name = "defeatScreen";
            defeatScreen.Size = new Size(841, 604);
            defeatScreen.SizeMode = PictureBoxSizeMode.AutoSize;
            defeatScreen.TabIndex = 8;
            defeatScreen.TabStop = false;
            defeatScreen.Visible = false;
            // 
            // victoryLabel
            // 
            victoryLabel.AutoSize = true;
            victoryLabel.BackColor = Color.Transparent;
            victoryLabel.Font = new Font("Consolas", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            victoryLabel.ForeColor = Color.White;
            victoryLabel.Location = new Point(101, 45);
            victoryLabel.Name = "victoryLabel";
            victoryLabel.Size = new Size(25, 28);
            victoryLabel.TabIndex = 9;
            victoryLabel.Text = "0";
            victoryLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // defeatLabel
            // 
            defeatLabel.AutoSize = true;
            defeatLabel.BackColor = Color.Transparent;
            defeatLabel.Font = new Font("Consolas", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            defeatLabel.ForeColor = Color.White;
            defeatLabel.Location = new Point(196, 45);
            defeatLabel.Name = "defeatLabel";
            defeatLabel.Size = new Size(25, 28);
            defeatLabel.TabIndex = 10;
            defeatLabel.Text = "0";
            defeatLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // roundTimerLabel
            // 
            roundTimerLabel.AutoSize = true;
            roundTimerLabel.BackColor = Color.Transparent;
            roundTimerLabel.Font = new Font("Consolas", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            roundTimerLabel.ForeColor = Color.White;
            roundTimerLabel.Location = new Point(668, 45);
            roundTimerLabel.Name = "roundTimerLabel";
            roundTimerLabel.Size = new Size(77, 28);
            roundTimerLabel.TabIndex = 11;
            roundTimerLabel.Text = "01:00";
            roundTimerLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // roundTimer
            // 
            roundTimer.Interval = 1000;
            roundTimer.Tick += roundTimer_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.ring3;
            ClientSize = new Size(825, 565);
            Controls.Add(roundTimerLabel);
            Controls.Add(defeatLabel);
            Controls.Add(victoryLabel);
            Controls.Add(defeatScreen);
            Controls.Add(victoryScreen);
            Controls.Add(boxer);
            Controls.Add(enemyBoxer);
            Controls.Add(playerLife);
            Controls.Add(enemyLife);
            Name = "Form1";
            Text = "\\";
            KeyDown += keyisdown;
            KeyUp += keyisup;
            ((System.ComponentModel.ISupportInitialize)enemyBoxer).EndInit();
            ((System.ComponentModel.ISupportInitialize)boxer).EndInit();
            ((System.ComponentModel.ISupportInitialize)victoryScreen).EndInit();
            ((System.ComponentModel.ISupportInitialize)defeatScreen).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar enemyLife;
        private ProgressBar playerLife;
        private PictureBox enemy;
        private PictureBox enemyBoxer;
        private PictureBox boxer;
        private System.Windows.Forms.Timer enemyTimer;
        private System.Windows.Forms.Timer enemyMove;
        private PictureBox victoryScreen;
        private PictureBox defeatScreen;
        private Label victoryLabel;
        private Label defeatLabel;
        private Label roundTimerLabel;
    }
}
