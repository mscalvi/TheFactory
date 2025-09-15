namespace GeradorCartas___Guildas
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
            pnlMainView = new Panel();
            tabctrlMainView = new TabControl();
            tabCreation = new TabPage();
            pnlCreateMaps = new Panel();
            btnImportListRelics = new Button();
            btnImportListPersonalities = new Button();
            btnImportListActions = new Button();
            btnImportListCharacters = new Button();
            btnImportListMaps = new Button();
            tabEdition = new TabPage();
            pnlMainView.SuspendLayout();
            tabctrlMainView.SuspendLayout();
            tabCreation.SuspendLayout();
            pnlCreateMaps.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMainView
            // 
            pnlMainView.Controls.Add(tabctrlMainView);
            pnlMainView.Dock = DockStyle.Fill;
            pnlMainView.Location = new Point(0, 0);
            pnlMainView.Name = "pnlMainView";
            pnlMainView.Size = new Size(1052, 559);
            pnlMainView.TabIndex = 0;
            // 
            // tabctrlMainView
            // 
            tabctrlMainView.Controls.Add(tabCreation);
            tabctrlMainView.Controls.Add(tabEdition);
            tabctrlMainView.Location = new Point(0, 3);
            tabctrlMainView.Name = "tabctrlMainView";
            tabctrlMainView.SelectedIndex = 0;
            tabctrlMainView.Size = new Size(1052, 553);
            tabctrlMainView.TabIndex = 0;
            // 
            // tabCreation
            // 
            tabCreation.Controls.Add(pnlCreateMaps);
            tabCreation.Location = new Point(4, 24);
            tabCreation.Name = "tabCreation";
            tabCreation.Padding = new Padding(3);
            tabCreation.Size = new Size(1044, 525);
            tabCreation.TabIndex = 0;
            tabCreation.Text = "Criar Cartas";
            tabCreation.UseVisualStyleBackColor = true;
            // 
            // pnlCreateMaps
            // 
            pnlCreateMaps.Controls.Add(btnImportListRelics);
            pnlCreateMaps.Controls.Add(btnImportListPersonalities);
            pnlCreateMaps.Controls.Add(btnImportListActions);
            pnlCreateMaps.Controls.Add(btnImportListCharacters);
            pnlCreateMaps.Controls.Add(btnImportListMaps);
            pnlCreateMaps.Dock = DockStyle.Fill;
            pnlCreateMaps.Location = new Point(3, 3);
            pnlCreateMaps.Name = "pnlCreateMaps";
            pnlCreateMaps.Size = new Size(1038, 519);
            pnlCreateMaps.TabIndex = 0;
            // 
            // btnImportListRelics
            // 
            btnImportListRelics.Location = new Point(705, 49);
            btnImportListRelics.Name = "btnImportListRelics";
            btnImportListRelics.Size = new Size(299, 86);
            btnImportListRelics.TabIndex = 5;
            btnImportListRelics.Text = "Importar Relíquias";
            btnImportListRelics.UseVisualStyleBackColor = true;
            btnImportListRelics.Click += btnImportListRelics_Click;
            // 
            // btnImportListPersonalities
            // 
            btnImportListPersonalities.Location = new Point(378, 160);
            btnImportListPersonalities.Name = "btnImportListPersonalities";
            btnImportListPersonalities.Size = new Size(299, 86);
            btnImportListPersonalities.TabIndex = 4;
            btnImportListPersonalities.Text = "Importar Personalidades";
            btnImportListPersonalities.UseVisualStyleBackColor = true;
            btnImportListPersonalities.Click += btnImportListPersonalities_Click;
            // 
            // btnImportListActions
            // 
            btnImportListActions.Location = new Point(378, 49);
            btnImportListActions.Name = "btnImportListActions";
            btnImportListActions.Size = new Size(299, 86);
            btnImportListActions.TabIndex = 3;
            btnImportListActions.Text = "Importar Ações";
            btnImportListActions.UseVisualStyleBackColor = true;
            btnImportListActions.Click += btnImportListActions_Click;
            // 
            // btnImportListCharacters
            // 
            btnImportListCharacters.Location = new Point(50, 49);
            btnImportListCharacters.Name = "btnImportListCharacters";
            btnImportListCharacters.Size = new Size(299, 86);
            btnImportListCharacters.TabIndex = 2;
            btnImportListCharacters.Text = "Importar Personagens";
            btnImportListCharacters.UseVisualStyleBackColor = true;
            btnImportListCharacters.Click += btnImportListCharacters_Click;
            // 
            // btnImportListMaps
            // 
            btnImportListMaps.Location = new Point(50, 160);
            btnImportListMaps.Name = "btnImportListMaps";
            btnImportListMaps.Size = new Size(299, 86);
            btnImportListMaps.TabIndex = 1;
            btnImportListMaps.Text = "Importar Mapas";
            btnImportListMaps.UseVisualStyleBackColor = true;
            btnImportListMaps.Click += btnImportListMaps_Click;
            // 
            // tabEdition
            // 
            tabEdition.Location = new Point(4, 24);
            tabEdition.Name = "tabEdition";
            tabEdition.Padding = new Padding(3);
            tabEdition.Size = new Size(1044, 525);
            tabEdition.TabIndex = 1;
            tabEdition.Text = "Editar Arquivos";
            tabEdition.UseVisualStyleBackColor = true;
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1052, 559);
            Controls.Add(pnlMainView);
            Name = "MainView";
            Text = "Guildas de Mudjin - Software de Controle";
            pnlMainView.ResumeLayout(false);
            tabctrlMainView.ResumeLayout(false);
            tabCreation.ResumeLayout(false);
            pnlCreateMaps.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMainView;
        private TabControl tabctrlMainView;
        private TabPage tabCreation;
        private Panel pnlCreateMaps;
        private Button btnImportListMaps;
        private TabPage tabEdition;
        private Button btnImportListCharacters;
        private Button btnImportListRelics;
        private Button btnImportListPersonalities;
        private Button btnImportListActions;
    }
}
