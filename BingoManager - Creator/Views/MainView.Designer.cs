namespace BingoCreator
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
            tabControlMain = new TabControl();
            tabCreatePage = new TabPage();
            tabControlCreate = new TabControl();
            tabCreateElement = new TabPage();
            btnElementCreate = new Button();
            lblElementMessage = new Label();
            lblElementTitle = new Label();
            cboElementList = new ComboBox();
            lblElementList = new Label();
            boxElementNote2 = new TextBox();
            boxElementNote1 = new TextBox();
            boxElementCardName = new TextBox();
            boxElementName = new TextBox();
            lblElementNote2 = new Label();
            lblElementNote1 = new Label();
            lblElementCardName = new Label();
            lblElementName = new Label();
            tabCreateList = new TabPage();
            btnListTxt = new Button();
            btnListImport = new Button();
            btnListCreate = new Button();
            lblListMessage = new Label();
            boxListDescription = new TextBox();
            boxListName = new TextBox();
            lblListDescription = new Label();
            lblListName = new Label();
            lblListTitle = new Label();
            tabCreateCards = new TabPage();
            cboCardsTheme = new ComboBox();
            lblCardsTheme = new Label();
            grpCardsSize = new GroupBox();
            radCardsSize5 = new RadioButton();
            radCardsSize4 = new RadioButton();
            btnCardsExport = new Button();
            lblCardsList = new Label();
            cboCardsList = new ComboBox();
            lblCardsQuant = new Label();
            boxCardsQuant = new TextBox();
            lblCardsEnd = new Label();
            boxCardsEnd = new TextBox();
            lblCardsHeader = new Label();
            lblCardsMessage = new Label();
            boxCardsTitle = new TextBox();
            boxCardsName = new TextBox();
            lblCardsTitle = new Label();
            lblCardsName = new Label();
            tabEditPage = new TabPage();
            boxEditText5 = new TextBox();
            lblEditText5 = new Label();
            btnEditExclude = new Button();
            flwEditItens = new FlowLayoutPanel();
            btnEditEdit = new Button();
            lblEditMessage = new Label();
            cboEdit = new ComboBox();
            picEdit = new PictureBox();
            boxEditText4 = new TextBox();
            boxEditText3 = new TextBox();
            boxEditText2 = new TextBox();
            boxEditText1 = new TextBox();
            lblEditImage = new Label();
            lblEditText4 = new Label();
            lblEditText3 = new Label();
            lblEditText2 = new Label();
            lblEditText1 = new Label();
            lblEditHeader = new Label();
            cboCardModel = new ComboBox();
            lblCardsModel = new Label();
            pnlMainView.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabCreatePage.SuspendLayout();
            tabControlCreate.SuspendLayout();
            tabCreateElement.SuspendLayout();
            tabCreateList.SuspendLayout();
            tabCreateCards.SuspendLayout();
            grpCardsSize.SuspendLayout();
            tabEditPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picEdit).BeginInit();
            SuspendLayout();
            // 
            // pnlMainView
            // 
            pnlMainView.AutoSize = true;
            pnlMainView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlMainView.Controls.Add(tabControlMain);
            pnlMainView.Dock = DockStyle.Fill;
            pnlMainView.Location = new Point(0, 0);
            pnlMainView.Name = "pnlMainView";
            pnlMainView.Size = new Size(1190, 744);
            pnlMainView.TabIndex = 0;
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabCreatePage);
            tabControlMain.Controls.Add(tabEditPage);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1190, 744);
            tabControlMain.TabIndex = 2;
            // 
            // tabCreatePage
            // 
            tabCreatePage.Controls.Add(tabControlCreate);
            tabCreatePage.Location = new Point(4, 24);
            tabCreatePage.Name = "tabCreatePage";
            tabCreatePage.Padding = new Padding(3);
            tabCreatePage.Size = new Size(1182, 716);
            tabCreatePage.TabIndex = 1;
            tabCreatePage.Text = "CreatePage";
            tabCreatePage.UseVisualStyleBackColor = true;
            // 
            // tabControlCreate
            // 
            tabControlCreate.Controls.Add(tabCreateElement);
            tabControlCreate.Controls.Add(tabCreateList);
            tabControlCreate.Controls.Add(tabCreateCards);
            tabControlCreate.Dock = DockStyle.Fill;
            tabControlCreate.Location = new Point(3, 3);
            tabControlCreate.Name = "tabControlCreate";
            tabControlCreate.SelectedIndex = 0;
            tabControlCreate.Size = new Size(1176, 710);
            tabControlCreate.TabIndex = 0;
            // 
            // tabCreateElement
            // 
            tabCreateElement.Controls.Add(btnElementCreate);
            tabCreateElement.Controls.Add(lblElementMessage);
            tabCreateElement.Controls.Add(lblElementTitle);
            tabCreateElement.Controls.Add(cboElementList);
            tabCreateElement.Controls.Add(lblElementList);
            tabCreateElement.Controls.Add(boxElementNote2);
            tabCreateElement.Controls.Add(boxElementNote1);
            tabCreateElement.Controls.Add(boxElementCardName);
            tabCreateElement.Controls.Add(boxElementName);
            tabCreateElement.Controls.Add(lblElementNote2);
            tabCreateElement.Controls.Add(lblElementNote1);
            tabCreateElement.Controls.Add(lblElementCardName);
            tabCreateElement.Controls.Add(lblElementName);
            tabCreateElement.Location = new Point(4, 24);
            tabCreateElement.Name = "tabCreateElement";
            tabCreateElement.Padding = new Padding(3);
            tabCreateElement.Size = new Size(1168, 682);
            tabCreateElement.TabIndex = 0;
            tabCreateElement.Text = "Element";
            tabCreateElement.UseVisualStyleBackColor = true;
            // 
            // btnElementCreate
            // 
            btnElementCreate.Anchor = AnchorStyles.Top;
            btnElementCreate.Location = new Point(787, 485);
            btnElementCreate.Name = "btnElementCreate";
            btnElementCreate.Size = new Size(295, 76);
            btnElementCreate.TabIndex = 14;
            btnElementCreate.Text = "Criar";
            btnElementCreate.UseVisualStyleBackColor = true;
            btnElementCreate.Click += btnElementCreat_Clicked;
            // 
            // lblElementMessage
            // 
            lblElementMessage.Anchor = AnchorStyles.Top;
            lblElementMessage.Font = new Font("Segoe UI", 12F);
            lblElementMessage.Location = new Point(787, 196);
            lblElementMessage.Name = "lblElementMessage";
            lblElementMessage.Size = new Size(295, 216);
            lblElementMessage.TabIndex = 13;
            lblElementMessage.Text = "Mensagem";
            lblElementMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblElementTitle
            // 
            lblElementTitle.Dock = DockStyle.Top;
            lblElementTitle.Font = new Font("Segoe UI", 16F);
            lblElementTitle.Location = new Point(3, 3);
            lblElementTitle.Name = "lblElementTitle";
            lblElementTitle.Size = new Size(1162, 87);
            lblElementTitle.TabIndex = 12;
            lblElementTitle.Text = "Criar Elemento";
            lblElementTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cboElementList
            // 
            cboElementList.FormattingEnabled = true;
            cboElementList.Location = new Point(275, 418);
            cboElementList.Name = "cboElementList";
            cboElementList.Size = new Size(395, 23);
            cboElementList.TabIndex = 5;
            // 
            // lblElementList
            // 
            lblElementList.Anchor = AnchorStyles.Top;
            lblElementList.Font = new Font("Segoe UI", 12F);
            lblElementList.Location = new Point(32, 407);
            lblElementList.Name = "lblElementList";
            lblElementList.Size = new Size(237, 38);
            lblElementList.TabIndex = 8;
            lblElementList.Text = "Lista:";
            lblElementList.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // boxElementNote2
            // 
            boxElementNote2.Anchor = AnchorStyles.Top;
            boxElementNote2.Font = new Font("Segoe UI", 12F);
            boxElementNote2.Location = new Point(275, 348);
            boxElementNote2.Name = "boxElementNote2";
            boxElementNote2.Size = new Size(395, 29);
            boxElementNote2.TabIndex = 4;
            // 
            // boxElementNote1
            // 
            boxElementNote1.Anchor = AnchorStyles.Top;
            boxElementNote1.Font = new Font("Segoe UI", 12F);
            boxElementNote1.Location = new Point(275, 273);
            boxElementNote1.Name = "boxElementNote1";
            boxElementNote1.Size = new Size(395, 29);
            boxElementNote1.TabIndex = 3;
            // 
            // boxElementCardName
            // 
            boxElementCardName.Anchor = AnchorStyles.Top;
            boxElementCardName.Font = new Font("Segoe UI", 12F);
            boxElementCardName.Location = new Point(275, 216);
            boxElementCardName.Name = "boxElementCardName";
            boxElementCardName.Size = new Size(395, 29);
            boxElementCardName.TabIndex = 2;
            // 
            // boxElementName
            // 
            boxElementName.Anchor = AnchorStyles.Top;
            boxElementName.Font = new Font("Segoe UI", 12F);
            boxElementName.Location = new Point(275, 160);
            boxElementName.Name = "boxElementName";
            boxElementName.Size = new Size(395, 29);
            boxElementName.TabIndex = 1;
            // 
            // lblElementNote2
            // 
            lblElementNote2.Anchor = AnchorStyles.Top;
            lblElementNote2.Font = new Font("Segoe UI", 12F);
            lblElementNote2.ImageAlign = ContentAlignment.MiddleLeft;
            lblElementNote2.Location = new Point(32, 342);
            lblElementNote2.Name = "lblElementNote2";
            lblElementNote2.Size = new Size(237, 38);
            lblElementNote2.TabIndex = 3;
            lblElementNote2.Text = "Anotação 2:";
            lblElementNote2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblElementNote1
            // 
            lblElementNote1.Anchor = AnchorStyles.Top;
            lblElementNote1.Font = new Font("Segoe UI", 12F);
            lblElementNote1.ImageAlign = ContentAlignment.MiddleLeft;
            lblElementNote1.Location = new Point(32, 273);
            lblElementNote1.Name = "lblElementNote1";
            lblElementNote1.Size = new Size(237, 38);
            lblElementNote1.TabIndex = 2;
            lblElementNote1.Text = "Anotação 1:";
            lblElementNote1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblElementCardName
            // 
            lblElementCardName.Anchor = AnchorStyles.Top;
            lblElementCardName.Font = new Font("Segoe UI", 12F);
            lblElementCardName.ImageAlign = ContentAlignment.MiddleLeft;
            lblElementCardName.Location = new Point(32, 210);
            lblElementCardName.Name = "lblElementCardName";
            lblElementCardName.Size = new Size(237, 38);
            lblElementCardName.TabIndex = 1;
            lblElementCardName.Text = "Nome para Cartela:";
            lblElementCardName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblElementName
            // 
            lblElementName.Anchor = AnchorStyles.Top;
            lblElementName.Font = new Font("Segoe UI", 12F);
            lblElementName.ImageAlign = ContentAlignment.MiddleLeft;
            lblElementName.Location = new Point(32, 151);
            lblElementName.Name = "lblElementName";
            lblElementName.Size = new Size(237, 38);
            lblElementName.TabIndex = 0;
            lblElementName.Text = "Nome:";
            lblElementName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tabCreateList
            // 
            tabCreateList.Controls.Add(btnListTxt);
            tabCreateList.Controls.Add(btnListImport);
            tabCreateList.Controls.Add(btnListCreate);
            tabCreateList.Controls.Add(lblListMessage);
            tabCreateList.Controls.Add(boxListDescription);
            tabCreateList.Controls.Add(boxListName);
            tabCreateList.Controls.Add(lblListDescription);
            tabCreateList.Controls.Add(lblListName);
            tabCreateList.Controls.Add(lblListTitle);
            tabCreateList.Location = new Point(4, 24);
            tabCreateList.Name = "tabCreateList";
            tabCreateList.Padding = new Padding(3);
            tabCreateList.Size = new Size(1168, 682);
            tabCreateList.TabIndex = 1;
            tabCreateList.Text = "List";
            tabCreateList.UseVisualStyleBackColor = true;
            // 
            // btnListTxt
            // 
            btnListTxt.Anchor = AnchorStyles.Top;
            btnListTxt.Location = new Point(390, 505);
            btnListTxt.Name = "btnListTxt";
            btnListTxt.Size = new Size(295, 76);
            btnListTxt.TabIndex = 24;
            btnListTxt.Text = "Importar Txt";
            btnListTxt.UseVisualStyleBackColor = true;
            btnListTxt.Click += btnListTxt_Clicked;
            // 
            // btnListImport
            // 
            btnListImport.Anchor = AnchorStyles.Top;
            btnListImport.Location = new Point(47, 505);
            btnListImport.Name = "btnListImport";
            btnListImport.Size = new Size(295, 76);
            btnListImport.TabIndex = 23;
            btnListImport.Text = "Importar Pasta";
            btnListImport.UseVisualStyleBackColor = true;
            btnListImport.Click += btnListImport_Clicked;
            // 
            // btnListCreate
            // 
            btnListCreate.Anchor = AnchorStyles.Top;
            btnListCreate.Location = new Point(217, 385);
            btnListCreate.Name = "btnListCreate";
            btnListCreate.Size = new Size(295, 76);
            btnListCreate.TabIndex = 22;
            btnListCreate.Text = "Criar";
            btnListCreate.UseVisualStyleBackColor = true;
            btnListCreate.Click += btnListCreate_Clicked;
            // 
            // lblListMessage
            // 
            lblListMessage.Anchor = AnchorStyles.Top;
            lblListMessage.Font = new Font("Segoe UI", 12F);
            lblListMessage.Location = new Point(817, 172);
            lblListMessage.Name = "lblListMessage";
            lblListMessage.Size = new Size(295, 289);
            lblListMessage.TabIndex = 21;
            lblListMessage.Text = "Mensagem";
            lblListMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // boxListDescription
            // 
            boxListDescription.Anchor = AnchorStyles.Top;
            boxListDescription.Font = new Font("Segoe UI", 12F);
            boxListDescription.Location = new Point(290, 237);
            boxListDescription.Name = "boxListDescription";
            boxListDescription.Size = new Size(395, 29);
            boxListDescription.TabIndex = 17;
            // 
            // boxListName
            // 
            boxListName.Anchor = AnchorStyles.Top;
            boxListName.Font = new Font("Segoe UI", 12F);
            boxListName.Location = new Point(290, 181);
            boxListName.Name = "boxListName";
            boxListName.Size = new Size(395, 29);
            boxListName.TabIndex = 15;
            // 
            // lblListDescription
            // 
            lblListDescription.Anchor = AnchorStyles.Top;
            lblListDescription.Font = new Font("Segoe UI", 12F);
            lblListDescription.ImageAlign = ContentAlignment.MiddleLeft;
            lblListDescription.Location = new Point(47, 231);
            lblListDescription.Name = "lblListDescription";
            lblListDescription.Size = new Size(237, 38);
            lblListDescription.TabIndex = 16;
            lblListDescription.Text = "Descrição:";
            lblListDescription.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblListName
            // 
            lblListName.Anchor = AnchorStyles.Top;
            lblListName.Font = new Font("Segoe UI", 12F);
            lblListName.ImageAlign = ContentAlignment.MiddleLeft;
            lblListName.Location = new Point(47, 172);
            lblListName.Name = "lblListName";
            lblListName.Size = new Size(237, 38);
            lblListName.TabIndex = 14;
            lblListName.Text = "Nome:";
            lblListName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblListTitle
            // 
            lblListTitle.Dock = DockStyle.Top;
            lblListTitle.Font = new Font("Segoe UI", 16F);
            lblListTitle.Location = new Point(3, 3);
            lblListTitle.Name = "lblListTitle";
            lblListTitle.Size = new Size(1162, 87);
            lblListTitle.TabIndex = 13;
            lblListTitle.Text = "Criar Lista";
            lblListTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tabCreateCards
            // 
            tabCreateCards.Controls.Add(cboCardModel);
            tabCreateCards.Controls.Add(lblCardsModel);
            tabCreateCards.Controls.Add(cboCardsTheme);
            tabCreateCards.Controls.Add(lblCardsTheme);
            tabCreateCards.Controls.Add(grpCardsSize);
            tabCreateCards.Controls.Add(btnCardsExport);
            tabCreateCards.Controls.Add(lblCardsList);
            tabCreateCards.Controls.Add(cboCardsList);
            tabCreateCards.Controls.Add(lblCardsQuant);
            tabCreateCards.Controls.Add(boxCardsQuant);
            tabCreateCards.Controls.Add(lblCardsEnd);
            tabCreateCards.Controls.Add(boxCardsEnd);
            tabCreateCards.Controls.Add(lblCardsHeader);
            tabCreateCards.Controls.Add(lblCardsMessage);
            tabCreateCards.Controls.Add(boxCardsTitle);
            tabCreateCards.Controls.Add(boxCardsName);
            tabCreateCards.Controls.Add(lblCardsTitle);
            tabCreateCards.Controls.Add(lblCardsName);
            tabCreateCards.Location = new Point(4, 24);
            tabCreateCards.Name = "tabCreateCards";
            tabCreateCards.Size = new Size(1168, 682);
            tabCreateCards.TabIndex = 2;
            tabCreateCards.Text = "Cards";
            tabCreateCards.UseVisualStyleBackColor = true;
            // 
            // cboCardsTheme
            // 
            cboCardsTheme.Anchor = AnchorStyles.Top;
            cboCardsTheme.FormattingEnabled = true;
            cboCardsTheme.Location = new Point(177, 414);
            cboCardsTheme.Name = "cboCardsTheme";
            cboCardsTheme.Size = new Size(202, 23);
            cboCardsTheme.TabIndex = 38;
            // 
            // lblCardsTheme
            // 
            lblCardsTheme.Anchor = AnchorStyles.Top;
            lblCardsTheme.Font = new Font("Segoe UI", 12F);
            lblCardsTheme.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsTheme.Location = new Point(67, 403);
            lblCardsTheme.Name = "lblCardsTheme";
            lblCardsTheme.Size = new Size(104, 38);
            lblCardsTheme.TabIndex = 37;
            lblCardsTheme.Text = "Cor:";
            lblCardsTheme.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // grpCardsSize
            // 
            grpCardsSize.Anchor = AnchorStyles.Top;
            grpCardsSize.Controls.Add(radCardsSize5);
            grpCardsSize.Controls.Add(radCardsSize4);
            grpCardsSize.Location = new Point(67, 533);
            grpCardsSize.Name = "grpCardsSize";
            grpCardsSize.Size = new Size(638, 100);
            grpCardsSize.TabIndex = 36;
            grpCardsSize.TabStop = false;
            grpCardsSize.Text = "Tamanho das Cartelas";
            // 
            // radCardsSize5
            // 
            radCardsSize5.Anchor = AnchorStyles.Top;
            radCardsSize5.Checked = true;
            radCardsSize5.Location = new Point(321, 44);
            radCardsSize5.Name = "radCardsSize5";
            radCardsSize5.Size = new Size(200, 32);
            radCardsSize5.TabIndex = 1;
            radCardsSize5.TabStop = true;
            radCardsSize5.Text = "Cartelas 5x5 (45+ Elementos)";
            radCardsSize5.UseVisualStyleBackColor = true;
            // 
            // radCardsSize4
            // 
            radCardsSize4.Anchor = AnchorStyles.Top;
            radCardsSize4.Location = new Point(64, 44);
            radCardsSize4.Name = "radCardsSize4";
            radCardsSize4.Size = new Size(200, 32);
            radCardsSize4.TabIndex = 0;
            radCardsSize4.TabStop = true;
            radCardsSize4.Text = "Cartelas 4x4 (35+ Elementos)";
            radCardsSize4.UseVisualStyleBackColor = true;
            // 
            // btnCardsExport
            // 
            btnCardsExport.Anchor = AnchorStyles.Top;
            btnCardsExport.Font = new Font("Segoe UI", 12F);
            btnCardsExport.Location = new Point(819, 557);
            btnCardsExport.Name = "btnCardsExport";
            btnCardsExport.Size = new Size(295, 76);
            btnCardsExport.TabIndex = 35;
            btnCardsExport.Text = "Exportar Jogo";
            btnCardsExport.UseVisualStyleBackColor = true;
            btnCardsExport.Click += btnExportCards_Click;
            // 
            // lblCardsList
            // 
            lblCardsList.Anchor = AnchorStyles.Top;
            lblCardsList.Font = new Font("Segoe UI", 12F);
            lblCardsList.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsList.Location = new Point(67, 466);
            lblCardsList.Name = "lblCardsList";
            lblCardsList.Size = new Size(237, 38);
            lblCardsList.TabIndex = 33;
            lblCardsList.Text = "Lista:";
            lblCardsList.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cboCardsList
            // 
            cboCardsList.Anchor = AnchorStyles.Top;
            cboCardsList.FormattingEnabled = true;
            cboCardsList.Location = new Point(310, 477);
            cboCardsList.Name = "cboCardsList";
            cboCardsList.Size = new Size(395, 23);
            cboCardsList.TabIndex = 32;
            // 
            // lblCardsQuant
            // 
            lblCardsQuant.Anchor = AnchorStyles.Top;
            lblCardsQuant.Font = new Font("Segoe UI", 12F);
            lblCardsQuant.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsQuant.Location = new Point(67, 339);
            lblCardsQuant.Name = "lblCardsQuant";
            lblCardsQuant.Size = new Size(237, 38);
            lblCardsQuant.TabIndex = 31;
            lblCardsQuant.Text = "Quantidade:";
            lblCardsQuant.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // boxCardsQuant
            // 
            boxCardsQuant.Anchor = AnchorStyles.Top;
            boxCardsQuant.Font = new Font("Segoe UI", 12F);
            boxCardsQuant.Location = new Point(310, 345);
            boxCardsQuant.Name = "boxCardsQuant";
            boxCardsQuant.Size = new Size(395, 29);
            boxCardsQuant.TabIndex = 30;
            // 
            // lblCardsEnd
            // 
            lblCardsEnd.Anchor = AnchorStyles.Top;
            lblCardsEnd.Font = new Font("Segoe UI", 12F);
            lblCardsEnd.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsEnd.Location = new Point(67, 270);
            lblCardsEnd.Name = "lblCardsEnd";
            lblCardsEnd.Size = new Size(237, 38);
            lblCardsEnd.TabIndex = 29;
            lblCardsEnd.Text = "Mensagem Final:";
            lblCardsEnd.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // boxCardsEnd
            // 
            boxCardsEnd.Anchor = AnchorStyles.Top;
            boxCardsEnd.Font = new Font("Segoe UI", 12F);
            boxCardsEnd.Location = new Point(310, 276);
            boxCardsEnd.Name = "boxCardsEnd";
            boxCardsEnd.Size = new Size(395, 29);
            boxCardsEnd.TabIndex = 28;
            // 
            // lblCardsHeader
            // 
            lblCardsHeader.Dock = DockStyle.Top;
            lblCardsHeader.Font = new Font("Segoe UI", 16F);
            lblCardsHeader.Location = new Point(0, 0);
            lblCardsHeader.Name = "lblCardsHeader";
            lblCardsHeader.Size = new Size(1168, 87);
            lblCardsHeader.TabIndex = 27;
            lblCardsHeader.Text = "Criar Cartelas";
            lblCardsHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblCardsMessage
            // 
            lblCardsMessage.Anchor = AnchorStyles.Top;
            lblCardsMessage.Font = new Font("Segoe UI", 12F);
            lblCardsMessage.Location = new Point(819, 185);
            lblCardsMessage.Name = "lblCardsMessage";
            lblCardsMessage.Size = new Size(295, 305);
            lblCardsMessage.TabIndex = 26;
            lblCardsMessage.Text = "Mensagem";
            lblCardsMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // boxCardsTitle
            // 
            boxCardsTitle.Anchor = AnchorStyles.Top;
            boxCardsTitle.Font = new Font("Segoe UI", 12F);
            boxCardsTitle.Location = new Point(310, 207);
            boxCardsTitle.Name = "boxCardsTitle";
            boxCardsTitle.Size = new Size(395, 29);
            boxCardsTitle.TabIndex = 25;
            // 
            // boxCardsName
            // 
            boxCardsName.Anchor = AnchorStyles.Top;
            boxCardsName.Font = new Font("Segoe UI", 12F);
            boxCardsName.Location = new Point(310, 138);
            boxCardsName.Name = "boxCardsName";
            boxCardsName.Size = new Size(395, 29);
            boxCardsName.TabIndex = 23;
            // 
            // lblCardsTitle
            // 
            lblCardsTitle.Anchor = AnchorStyles.Top;
            lblCardsTitle.Font = new Font("Segoe UI", 12F);
            lblCardsTitle.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsTitle.Location = new Point(67, 201);
            lblCardsTitle.Name = "lblCardsTitle";
            lblCardsTitle.Size = new Size(237, 38);
            lblCardsTitle.TabIndex = 24;
            lblCardsTitle.Text = "Título para Cartela:";
            lblCardsTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblCardsName
            // 
            lblCardsName.Anchor = AnchorStyles.Top;
            lblCardsName.Font = new Font("Segoe UI", 12F);
            lblCardsName.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsName.Location = new Point(67, 132);
            lblCardsName.Name = "lblCardsName";
            lblCardsName.Size = new Size(237, 38);
            lblCardsName.TabIndex = 22;
            lblCardsName.Text = "Nome do Conjunto:";
            lblCardsName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tabEditPage
            // 
            tabEditPage.Controls.Add(boxEditText5);
            tabEditPage.Controls.Add(lblEditText5);
            tabEditPage.Controls.Add(btnEditExclude);
            tabEditPage.Controls.Add(flwEditItens);
            tabEditPage.Controls.Add(btnEditEdit);
            tabEditPage.Controls.Add(lblEditMessage);
            tabEditPage.Controls.Add(cboEdit);
            tabEditPage.Controls.Add(picEdit);
            tabEditPage.Controls.Add(boxEditText4);
            tabEditPage.Controls.Add(boxEditText3);
            tabEditPage.Controls.Add(boxEditText2);
            tabEditPage.Controls.Add(boxEditText1);
            tabEditPage.Controls.Add(lblEditImage);
            tabEditPage.Controls.Add(lblEditText4);
            tabEditPage.Controls.Add(lblEditText3);
            tabEditPage.Controls.Add(lblEditText2);
            tabEditPage.Controls.Add(lblEditText1);
            tabEditPage.Controls.Add(lblEditHeader);
            tabEditPage.Location = new Point(4, 24);
            tabEditPage.Name = "tabEditPage";
            tabEditPage.Size = new Size(1182, 716);
            tabEditPage.TabIndex = 2;
            tabEditPage.Text = "EditPage";
            tabEditPage.UseVisualStyleBackColor = true;
            // 
            // boxEditText5
            // 
            boxEditText5.Anchor = AnchorStyles.Top;
            boxEditText5.Font = new Font("Segoe UI", 12F);
            boxEditText5.Location = new Point(742, 344);
            boxEditText5.Name = "boxEditText5";
            boxEditText5.Size = new Size(395, 29);
            boxEditText5.TabIndex = 32;
            // 
            // lblEditText5
            // 
            lblEditText5.Anchor = AnchorStyles.Top;
            lblEditText5.Font = new Font("Segoe UI", 12F);
            lblEditText5.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditText5.Location = new Point(499, 338);
            lblEditText5.Name = "lblEditText5";
            lblEditText5.Size = new Size(237, 38);
            lblEditText5.TabIndex = 31;
            lblEditText5.Text = "Informação 5:";
            lblEditText5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnEditExclude
            // 
            btnEditExclude.Anchor = AnchorStyles.Top;
            btnEditExclude.Location = new Point(820, 619);
            btnEditExclude.Name = "btnEditExclude";
            btnEditExclude.Size = new Size(267, 76);
            btnEditExclude.TabIndex = 30;
            btnEditExclude.Text = "Excluir";
            btnEditExclude.UseVisualStyleBackColor = true;
            // 
            // flwEditItens
            // 
            flwEditItens.Location = new Point(27, 148);
            flwEditItens.Name = "flwEditItens";
            flwEditItens.Size = new Size(391, 401);
            flwEditItens.TabIndex = 29;
            // 
            // btnEditEdit
            // 
            btnEditEdit.Anchor = AnchorStyles.Top;
            btnEditEdit.Location = new Point(499, 619);
            btnEditEdit.Name = "btnEditEdit";
            btnEditEdit.Size = new Size(267, 76);
            btnEditEdit.TabIndex = 28;
            btnEditEdit.Text = "Editar";
            btnEditEdit.UseVisualStyleBackColor = true;
            // 
            // lblEditMessage
            // 
            lblEditMessage.Anchor = AnchorStyles.Top;
            lblEditMessage.Font = new Font("Segoe UI", 12F);
            lblEditMessage.Location = new Point(27, 585);
            lblEditMessage.Name = "lblEditMessage";
            lblEditMessage.Size = new Size(391, 110);
            lblEditMessage.TabIndex = 27;
            lblEditMessage.Text = "Mensagem";
            lblEditMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cboEdit
            // 
            cboEdit.FormattingEnabled = true;
            cboEdit.Location = new Point(27, 119);
            cboEdit.Name = "cboEdit";
            cboEdit.Size = new Size(391, 23);
            cboEdit.TabIndex = 24;
            // 
            // picEdit
            // 
            picEdit.Location = new Point(742, 405);
            picEdit.Name = "picEdit";
            picEdit.Size = new Size(395, 191);
            picEdit.TabIndex = 26;
            picEdit.TabStop = false;
            // 
            // boxEditText4
            // 
            boxEditText4.Anchor = AnchorStyles.Top;
            boxEditText4.Font = new Font("Segoe UI", 12F);
            boxEditText4.Location = new Point(742, 287);
            boxEditText4.Name = "boxEditText4";
            boxEditText4.Size = new Size(395, 29);
            boxEditText4.TabIndex = 22;
            // 
            // boxEditText3
            // 
            boxEditText3.Anchor = AnchorStyles.Top;
            boxEditText3.Font = new Font("Segoe UI", 12F);
            boxEditText3.Location = new Point(742, 230);
            boxEditText3.Name = "boxEditText3";
            boxEditText3.Size = new Size(395, 29);
            boxEditText3.TabIndex = 20;
            // 
            // boxEditText2
            // 
            boxEditText2.Anchor = AnchorStyles.Top;
            boxEditText2.Font = new Font("Segoe UI", 12F);
            boxEditText2.Location = new Point(742, 173);
            boxEditText2.Name = "boxEditText2";
            boxEditText2.Size = new Size(395, 29);
            boxEditText2.TabIndex = 18;
            // 
            // boxEditText1
            // 
            boxEditText1.Anchor = AnchorStyles.Top;
            boxEditText1.Font = new Font("Segoe UI", 12F);
            boxEditText1.Location = new Point(742, 114);
            boxEditText1.Name = "boxEditText1";
            boxEditText1.Size = new Size(395, 29);
            boxEditText1.TabIndex = 16;
            // 
            // lblEditImage
            // 
            lblEditImage.Anchor = AnchorStyles.Top;
            lblEditImage.Font = new Font("Segoe UI", 12F);
            lblEditImage.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditImage.Location = new Point(499, 429);
            lblEditImage.Name = "lblEditImage";
            lblEditImage.Size = new Size(237, 38);
            lblEditImage.TabIndex = 23;
            lblEditImage.Text = "Imagem:";
            lblEditImage.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEditText4
            // 
            lblEditText4.Anchor = AnchorStyles.Top;
            lblEditText4.Font = new Font("Segoe UI", 12F);
            lblEditText4.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditText4.Location = new Point(499, 281);
            lblEditText4.Name = "lblEditText4";
            lblEditText4.Size = new Size(237, 38);
            lblEditText4.TabIndex = 21;
            lblEditText4.Text = "Informação 4:";
            lblEditText4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEditText3
            // 
            lblEditText3.Anchor = AnchorStyles.Top;
            lblEditText3.Font = new Font("Segoe UI", 12F);
            lblEditText3.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditText3.Location = new Point(499, 224);
            lblEditText3.Name = "lblEditText3";
            lblEditText3.Size = new Size(237, 38);
            lblEditText3.TabIndex = 19;
            lblEditText3.Text = "Informação 3:";
            lblEditText3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEditText2
            // 
            lblEditText2.Anchor = AnchorStyles.Top;
            lblEditText2.Font = new Font("Segoe UI", 12F);
            lblEditText2.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditText2.Location = new Point(499, 167);
            lblEditText2.Name = "lblEditText2";
            lblEditText2.Size = new Size(237, 38);
            lblEditText2.TabIndex = 17;
            lblEditText2.Text = "Informação 2:";
            lblEditText2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEditText1
            // 
            lblEditText1.Anchor = AnchorStyles.Top;
            lblEditText1.Font = new Font("Segoe UI", 12F);
            lblEditText1.ImageAlign = ContentAlignment.MiddleLeft;
            lblEditText1.Location = new Point(499, 110);
            lblEditText1.Name = "lblEditText1";
            lblEditText1.Size = new Size(237, 38);
            lblEditText1.TabIndex = 15;
            lblEditText1.Text = "Informação 1:";
            lblEditText1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblEditHeader
            // 
            lblEditHeader.Dock = DockStyle.Top;
            lblEditHeader.Font = new Font("Segoe UI", 16F);
            lblEditHeader.Location = new Point(0, 0);
            lblEditHeader.Name = "lblEditHeader";
            lblEditHeader.Size = new Size(1182, 87);
            lblEditHeader.TabIndex = 13;
            lblEditHeader.Text = "Editar";
            lblEditHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cboCardModel
            // 
            cboCardModel.Anchor = AnchorStyles.Top;
            cboCardModel.FormattingEnabled = true;
            cboCardModel.Location = new Point(503, 414);
            cboCardModel.Name = "cboCardModel";
            cboCardModel.Size = new Size(202, 23);
            cboCardModel.TabIndex = 40;
            // 
            // lblCardsModel
            // 
            lblCardsModel.Anchor = AnchorStyles.Top;
            lblCardsModel.Font = new Font("Segoe UI", 12F);
            lblCardsModel.ImageAlign = ContentAlignment.MiddleLeft;
            lblCardsModel.Location = new Point(393, 403);
            lblCardsModel.Name = "lblCardsModel";
            lblCardsModel.Size = new Size(104, 38);
            lblCardsModel.TabIndex = 39;
            lblCardsModel.Text = "Modelo:";
            lblCardsModel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1190, 744);
            Controls.Add(pnlMainView);
            Name = "MainView";
            Text = "BingoManager - Creator";
            pnlMainView.ResumeLayout(false);
            tabControlMain.ResumeLayout(false);
            tabCreatePage.ResumeLayout(false);
            tabControlCreate.ResumeLayout(false);
            tabCreateElement.ResumeLayout(false);
            tabCreateElement.PerformLayout();
            tabCreateList.ResumeLayout(false);
            tabCreateList.PerformLayout();
            tabCreateCards.ResumeLayout(false);
            tabCreateCards.PerformLayout();
            grpCardsSize.ResumeLayout(false);
            tabEditPage.ResumeLayout(false);
            tabEditPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picEdit).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlMainView;
        private TabControl tabControlMain;
        private TabPage tabCreatePage;
        private TabPage tabEditPage;
        private TabControl tabControlCreate;
        private TabPage tabCreateElement;
        private TabPage tabCreateList;
        private TabPage tabCreateCards;
        private TextBox boxElementName;
        private Label lblElementNote2;
        private Label lblElementNote1;
        private Label lblElementCardName;
        private Label lblElementName;
        private Label lblElementList;
        private TextBox boxElementNote2;
        private TextBox boxElementNote1;
        private TextBox boxElementCardName;
        private Label lblElementTitle;
        private ComboBox cboElementList;
        private Label lblElementMessage;
        private Label lblListMessage;
        private TextBox boxListDescription;
        private TextBox boxListName;
        private Label lblListDescription;
        private Label lblListName;
        private Label lblListTitle;
        private Label lblCardsHeader;
        private Label lblCardsMessage;
        private TextBox boxCardsTitle;
        private TextBox boxCardsName;
        private Label lblCardsTitle;
        private Label lblCardsName;
        private TextBox boxCardsEnd;
        private Label lblCardsList;
        private ComboBox cboCardsList;
        private Label lblCardsQuant;
        private TextBox boxCardsQuant;
        private Label lblCardsEnd;
        private Button btnElementCreate;
        private Button btnListCreate;
        private Button btnCardsExport;
        private FlowLayoutPanel flwEditItens;
        private Button btnEditEdit;
        private Label lblEditMessage;
        private ComboBox cboEdit;
        private PictureBox picEdit;
        private TextBox boxEditText4;
        private TextBox boxEditText3;
        private TextBox boxEditText2;
        private TextBox boxEditText1;
        private Label lblEditImage;
        private Label lblEditText4;
        private Label lblEditText3;
        private Label lblEditText2;
        private Label lblEditText1;
        private Label lblEditHeader;
        private Button btnEditExclude;
        private TextBox boxEditText5;
        private Label lblEditText5;
        private GroupBox grpCardsSize;
        private RadioButton radCardsSize5;
        private RadioButton radCardsSize4;
        private Button btnListImport;
        private Button btnListTxt;
        private ComboBox cboCardsTheme;
        private Label lblCardsTheme;
        private ComboBox cboCardModel;
        private Label lblCardsModel;
    }
}
