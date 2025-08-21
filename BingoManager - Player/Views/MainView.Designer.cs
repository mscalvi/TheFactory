namespace BingoManager
{
    partial class MainView
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
            toolTip1 = new ToolTip(components);
            pnlBingoManager = new Panel();
            pnlButtons = new Panel();
            btnBingo = new Button();
            btnRandom = new Button();
            btnReset = new Button();
            btnStart = new Button();
            pnlPlay = new Panel();
            lbltResults = new Label();
            lblLastResult = new Label();
            lblResults = new Label();
            picPlayLogo = new PictureBox();
            pnlPlayNumbersB = new Panel();
            flwPlayB = new FlowLayoutPanel();
            pnlPlayNumbersI = new Panel();
            flwPlayI = new FlowLayoutPanel();
            pnlPlayNumbersN = new Panel();
            flwPlayN = new FlowLayoutPanel();
            pnlPlayNumbersG = new Panel();
            flwPlayG = new FlowLayoutPanel();
            pnlPlayNumbersO = new Panel();
            flwPlayO = new FlowLayoutPanel();
            pnlGameInfo = new Panel();
            groupBox1 = new GroupBox();
            rdManual = new RadioButton();
            rdDigital = new RadioButton();
            lblCardsQnt = new Label();
            lblGameName = new Label();
            grpPlayPhase = new GroupBox();
            rdLine = new RadioButton();
            rdFull = new RadioButton();
            lblMyBingo = new Label();
            pnlBingoManager.SuspendLayout();
            pnlButtons.SuspendLayout();
            pnlPlay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPlayLogo).BeginInit();
            pnlPlayNumbersB.SuspendLayout();
            pnlPlayNumbersI.SuspendLayout();
            pnlPlayNumbersN.SuspendLayout();
            pnlPlayNumbersG.SuspendLayout();
            pnlPlayNumbersO.SuspendLayout();
            pnlGameInfo.SuspendLayout();
            groupBox1.SuspendLayout();
            grpPlayPhase.SuspendLayout();
            SuspendLayout();
            // 
            // pnlBingoManager
            // 
            pnlBingoManager.AutoSize = true;
            pnlBingoManager.Controls.Add(pnlButtons);
            pnlBingoManager.Controls.Add(pnlPlay);
            pnlBingoManager.Controls.Add(pnlGameInfo);
            pnlBingoManager.Controls.Add(lblMyBingo);
            pnlBingoManager.Dock = DockStyle.Fill;
            pnlBingoManager.Location = new Point(0, 0);
            pnlBingoManager.Name = "pnlBingoManager";
            pnlBingoManager.Size = new Size(1217, 741);
            pnlBingoManager.TabIndex = 0;
            // 
            // pnlButtons
            // 
            pnlButtons.Anchor = AnchorStyles.Top;
            pnlButtons.Controls.Add(btnBingo);
            pnlButtons.Controls.Add(btnRandom);
            pnlButtons.Controls.Add(btnReset);
            pnlButtons.Controls.Add(btnStart);
            pnlButtons.Location = new Point(583, 81);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(631, 134);
            pnlButtons.TabIndex = 39;
            // 
            // btnBingo
            // 
            btnBingo.Location = new Point(319, 3);
            btnBingo.Name = "btnBingo";
            btnBingo.Size = new Size(152, 125);
            btnBingo.TabIndex = 3;
            btnBingo.Text = "Bingo";
            btnBingo.UseVisualStyleBackColor = true;
            btnBingo.Click += btnBingo_Click;
            // 
            // btnRandom
            // 
            btnRandom.Location = new Point(161, 3);
            btnRandom.Name = "btnRandom";
            btnRandom.Size = new Size(152, 125);
            btnRandom.TabIndex = 2;
            btnRandom.Text = "Sortear";
            btnRandom.UseVisualStyleBackColor = true;
            btnRandom.Click += btnRandom_Click;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(476, 3);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(152, 125);
            btnReset.TabIndex = 1;
            btnReset.Text = "Reiniciar";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnRestart_Click;
            // 
            // btnStart
            // 
            btnStart.Location = new Point(3, 3);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(152, 125);
            btnStart.TabIndex = 0;
            btnStart.Text = "Começar";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // pnlPlay
            // 
            pnlPlay.Anchor = AnchorStyles.Top;
            pnlPlay.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlPlay.BackColor = Color.Transparent;
            pnlPlay.Controls.Add(lbltResults);
            pnlPlay.Controls.Add(lblLastResult);
            pnlPlay.Controls.Add(lblResults);
            pnlPlay.Controls.Add(picPlayLogo);
            pnlPlay.Controls.Add(pnlPlayNumbersB);
            pnlPlay.Controls.Add(pnlPlayNumbersI);
            pnlPlay.Controls.Add(pnlPlayNumbersN);
            pnlPlay.Controls.Add(pnlPlayNumbersG);
            pnlPlay.Controls.Add(pnlPlayNumbersO);
            pnlPlay.Location = new Point(3, 221);
            pnlPlay.Name = "pnlPlay";
            pnlPlay.Size = new Size(1211, 513);
            pnlPlay.TabIndex = 38;
            // 
            // lbltResults
            // 
            lbltResults.Anchor = AnchorStyles.Top;
            lbltResults.Font = new Font("Segoe UI", 12F);
            lbltResults.Location = new Point(800, 6);
            lbltResults.Name = "lbltResults";
            lbltResults.Size = new Size(411, 23);
            lbltResults.TabIndex = 37;
            lbltResults.Text = "Últimos Resultados";
            lbltResults.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblLastResult
            // 
            lblLastResult.Anchor = AnchorStyles.Top;
            lblLastResult.Font = new Font("Segoe UI", 12F);
            lblLastResult.Location = new Point(818, 32);
            lblLastResult.Name = "lblLastResult";
            lblLastResult.Size = new Size(205, 127);
            lblLastResult.TabIndex = 39;
            lblLastResult.Text = "Último Elemento Sorteado";
            lblLastResult.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblResults
            // 
            lblResults.Anchor = AnchorStyles.Top;
            lblResults.Font = new Font("Segoe UI", 15F);
            lblResults.Location = new Point(800, 162);
            lblResults.Name = "lblResults";
            lblResults.Size = new Size(408, 343);
            lblResults.TabIndex = 33;
            lblResults.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // picPlayLogo
            // 
            picPlayLogo.Anchor = AnchorStyles.Top;
            picPlayLogo.Location = new Point(1029, 32);
            picPlayLogo.Name = "picPlayLogo";
            picPlayLogo.Size = new Size(173, 127);
            picPlayLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picPlayLogo.TabIndex = 40;
            picPlayLogo.TabStop = false;
            // 
            // pnlPlayNumbersB
            // 
            pnlPlayNumbersB.Anchor = AnchorStyles.Top;
            pnlPlayNumbersB.BackColor = Color.Transparent;
            pnlPlayNumbersB.Controls.Add(flwPlayB);
            pnlPlayNumbersB.Location = new Point(3, 6);
            pnlPlayNumbersB.Name = "pnlPlayNumbersB";
            pnlPlayNumbersB.Size = new Size(794, 95);
            pnlPlayNumbersB.TabIndex = 0;
            // 
            // flwPlayB
            // 
            flwPlayB.BackColor = Color.Transparent;
            flwPlayB.Dock = DockStyle.Fill;
            flwPlayB.Location = new Point(0, 0);
            flwPlayB.Name = "flwPlayB";
            flwPlayB.Size = new Size(794, 95);
            flwPlayB.TabIndex = 0;
            // 
            // pnlPlayNumbersI
            // 
            pnlPlayNumbersI.Anchor = AnchorStyles.Top;
            pnlPlayNumbersI.Controls.Add(flwPlayI);
            pnlPlayNumbersI.Location = new Point(3, 107);
            pnlPlayNumbersI.Name = "pnlPlayNumbersI";
            pnlPlayNumbersI.Size = new Size(794, 95);
            pnlPlayNumbersI.TabIndex = 1;
            // 
            // flwPlayI
            // 
            flwPlayI.Dock = DockStyle.Fill;
            flwPlayI.Location = new Point(0, 0);
            flwPlayI.Name = "flwPlayI";
            flwPlayI.Size = new Size(794, 95);
            flwPlayI.TabIndex = 0;
            // 
            // pnlPlayNumbersN
            // 
            pnlPlayNumbersN.Anchor = AnchorStyles.Top;
            pnlPlayNumbersN.Controls.Add(flwPlayN);
            pnlPlayNumbersN.Location = new Point(3, 208);
            pnlPlayNumbersN.Name = "pnlPlayNumbersN";
            pnlPlayNumbersN.Size = new Size(794, 95);
            pnlPlayNumbersN.TabIndex = 2;
            // 
            // flwPlayN
            // 
            flwPlayN.Dock = DockStyle.Fill;
            flwPlayN.Location = new Point(0, 0);
            flwPlayN.Name = "flwPlayN";
            flwPlayN.Size = new Size(794, 95);
            flwPlayN.TabIndex = 0;
            // 
            // pnlPlayNumbersG
            // 
            pnlPlayNumbersG.Anchor = AnchorStyles.Top;
            pnlPlayNumbersG.Controls.Add(flwPlayG);
            pnlPlayNumbersG.Location = new Point(3, 309);
            pnlPlayNumbersG.Name = "pnlPlayNumbersG";
            pnlPlayNumbersG.Size = new Size(794, 95);
            pnlPlayNumbersG.TabIndex = 3;
            // 
            // flwPlayG
            // 
            flwPlayG.Dock = DockStyle.Fill;
            flwPlayG.Location = new Point(0, 0);
            flwPlayG.Name = "flwPlayG";
            flwPlayG.Size = new Size(794, 95);
            flwPlayG.TabIndex = 0;
            // 
            // pnlPlayNumbersO
            // 
            pnlPlayNumbersO.Anchor = AnchorStyles.Top;
            pnlPlayNumbersO.Controls.Add(flwPlayO);
            pnlPlayNumbersO.Location = new Point(3, 410);
            pnlPlayNumbersO.Name = "pnlPlayNumbersO";
            pnlPlayNumbersO.Size = new Size(794, 95);
            pnlPlayNumbersO.TabIndex = 4;
            // 
            // flwPlayO
            // 
            flwPlayO.Dock = DockStyle.Fill;
            flwPlayO.Location = new Point(0, 0);
            flwPlayO.Name = "flwPlayO";
            flwPlayO.Size = new Size(794, 95);
            flwPlayO.TabIndex = 0;
            // 
            // pnlGameInfo
            // 
            pnlGameInfo.Anchor = AnchorStyles.Top;
            pnlGameInfo.Controls.Add(groupBox1);
            pnlGameInfo.Controls.Add(lblCardsQnt);
            pnlGameInfo.Controls.Add(lblGameName);
            pnlGameInfo.Controls.Add(grpPlayPhase);
            pnlGameInfo.Location = new Point(3, 81);
            pnlGameInfo.Name = "pnlGameInfo";
            pnlGameInfo.Size = new Size(574, 134);
            pnlGameInfo.TabIndex = 36;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top;
            groupBox1.Controls.Add(rdManual);
            groupBox1.Controls.Add(rdDigital);
            groupBox1.Font = new Font("Segoe UI", 12F);
            groupBox1.Location = new Point(9, 66);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(284, 62);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Tipo de Jogo";
            // 
            // rdManual
            // 
            rdManual.Anchor = AnchorStyles.Top;
            rdManual.AutoSize = true;
            rdManual.Checked = true;
            rdManual.Font = new Font("Segoe UI", 12F);
            rdManual.Location = new Point(6, 24);
            rdManual.Name = "rdManual";
            rdManual.Size = new Size(134, 25);
            rdManual.TabIndex = 1;
            rdManual.TabStop = true;
            rdManual.Text = "Sorteio Manual";
            rdManual.UseVisualStyleBackColor = true;
            // 
            // rdDigital
            // 
            rdDigital.Anchor = AnchorStyles.Top;
            rdDigital.AutoSize = true;
            rdDigital.Font = new Font("Segoe UI", 12F);
            rdDigital.Location = new Point(146, 24);
            rdDigital.Name = "rdDigital";
            rdDigital.Size = new Size(127, 25);
            rdDigital.TabIndex = 0;
            rdDigital.Text = "Sorteio Digital";
            rdDigital.UseVisualStyleBackColor = true;
            // 
            // lblCardsQnt
            // 
            lblCardsQnt.Anchor = AnchorStyles.Top;
            lblCardsQnt.Font = new Font("Segoe UI", 14F);
            lblCardsQnt.Location = new Point(298, 3);
            lblCardsQnt.Name = "lblCardsQnt";
            lblCardsQnt.Size = new Size(270, 37);
            lblCardsQnt.TabIndex = 28;
            lblCardsQnt.Text = "Quantidade de Cartelas:";
            lblCardsQnt.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblGameName
            // 
            lblGameName.Anchor = AnchorStyles.Top;
            lblGameName.Font = new Font("Segoe UI", 14F);
            lblGameName.Location = new Point(9, 3);
            lblGameName.Name = "lblGameName";
            lblGameName.Size = new Size(284, 37);
            lblGameName.TabIndex = 25;
            lblGameName.Text = "Nome do Jogo:";
            lblGameName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // grpPlayPhase
            // 
            grpPlayPhase.Anchor = AnchorStyles.Top;
            grpPlayPhase.Controls.Add(rdLine);
            grpPlayPhase.Controls.Add(rdFull);
            grpPlayPhase.Font = new Font("Segoe UI", 12F);
            grpPlayPhase.Location = new Point(298, 66);
            grpPlayPhase.Name = "grpPlayPhase";
            grpPlayPhase.Size = new Size(270, 62);
            grpPlayPhase.TabIndex = 3;
            grpPlayPhase.TabStop = false;
            grpPlayPhase.Text = "Fase de Jogo";
            // 
            // rdLine
            // 
            rdLine.Anchor = AnchorStyles.Top;
            rdLine.AutoSize = true;
            rdLine.Checked = true;
            rdLine.Font = new Font("Segoe UI", 12F);
            rdLine.Location = new Point(6, 24);
            rdLine.Name = "rdLine";
            rdLine.Size = new Size(70, 25);
            rdLine.TabIndex = 1;
            rdLine.TabStop = true;
            rdLine.Text = "Quina";
            rdLine.UseVisualStyleBackColor = true;
            // 
            // rdFull
            // 
            rdFull.Anchor = AnchorStyles.Top;
            rdFull.AutoSize = true;
            rdFull.Font = new Font("Segoe UI", 12F);
            rdFull.Location = new Point(82, 24);
            rdFull.Name = "rdFull";
            rdFull.Size = new Size(120, 25);
            rdFull.TabIndex = 0;
            rdFull.Text = "Cartela Cheia";
            rdFull.UseVisualStyleBackColor = true;
            // 
            // lblMyBingo
            // 
            lblMyBingo.Dock = DockStyle.Top;
            lblMyBingo.Font = new Font("Segoe UI", 26.25F, FontStyle.Bold);
            lblMyBingo.Location = new Point(0, 0);
            lblMyBingo.Name = "lblMyBingo";
            lblMyBingo.Size = new Size(1217, 78);
            lblMyBingo.TabIndex = 35;
            lblMyBingo.Text = "Custom Bingo - Sala Delta";
            lblMyBingo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1217, 741);
            Controls.Add(pnlBingoManager);
            Name = "MainView";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bingo Manager";
            WindowState = FormWindowState.Maximized;
            Load += MainView_Load;
            pnlBingoManager.ResumeLayout(false);
            pnlButtons.ResumeLayout(false);
            pnlPlay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picPlayLogo).EndInit();
            pnlPlayNumbersB.ResumeLayout(false);
            pnlPlayNumbersI.ResumeLayout(false);
            pnlPlayNumbersN.ResumeLayout(false);
            pnlPlayNumbersG.ResumeLayout(false);
            pnlPlayNumbersO.ResumeLayout(false);
            pnlGameInfo.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            grpPlayPhase.ResumeLayout(false);
            grpPlayPhase.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ToolTip toolTip1;
        private Panel pnlBingoManager;
        private Label lblResults;
        private Label lblGameName;
        private Label lbltResults;
        private PictureBox picPlayLogo;
        private Label lblLastResult;
        private Panel pnlPlay;
        private Panel pnlPlayNumbersB;
        private FlowLayoutPanel flwPlayB;
        private Panel pnlPlayNumbersI;
        private FlowLayoutPanel flwPlayI;
        private Panel pnlPlayNumbersN;
        private FlowLayoutPanel flwPlayN;
        private Panel pnlPlayNumbersG;
        private FlowLayoutPanel flwPlayG;
        private Panel pnlPlayNumbersO;
        private FlowLayoutPanel flwPlayO;
        private Panel pnlGameInfo;
        private Button BtnRestartAn;
        private ComboBox CboPlayAnSelection;
        private GroupBox grpPlayPhase;
        private RadioButton rdLine;
        private RadioButton rdFull;
        private Label lblMyBingo;
        private Label lblCardsQnt;
        private Panel pnlButtons;
        private Button btnRandom;
        private Button btnReset;
        private Button btnStart;
        private GroupBox groupBox1;
        private RadioButton rdManual;
        private RadioButton rdDigital;
        private Button btnBingo;
    }
}
