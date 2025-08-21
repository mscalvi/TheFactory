namespace BingoManager.Views
{
    partial class LogoView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ShowCompName = new Label();
            ShowCompLogo = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)ShowCompLogo).BeginInit();
            SuspendLayout();
            // 
            // ShowCompName
            // 
            ShowCompName.Dock = DockStyle.Top;
            ShowCompName.Font = new Font("Segoe UI", 27.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            ShowCompName.Location = new Point(0, 0);
            ShowCompName.Name = "ShowCompName";
            ShowCompName.Size = new Size(1350, 93);
            ShowCompName.TabIndex = 0;
            ShowCompName.Text = "Bingo Manager";
            ShowCompName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ShowCompLogo
            // 
            ShowCompLogo.Dock = DockStyle.Fill;
            ShowCompLogo.Location = new Point(0, 93);
            ShowCompLogo.Name = "ShowCompLogo";
            ShowCompLogo.Size = new Size(1350, 636);
            ShowCompLogo.SizeMode = PictureBoxSizeMode.Zoom;
            ShowCompLogo.TabIndex = 1;
            ShowCompLogo.TabStop = false;
            // 
            // LogoView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(1350, 729);
            Controls.Add(ShowCompLogo);
            Controls.Add(ShowCompName);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LogoView";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bingo Manager";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)ShowCompLogo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label ShowCompName;
        private PictureBox ShowCompLogo;
    }
}