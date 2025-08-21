using BingoManager.Models;
using BingoManager.Services;
using BingoManager.Views;
using System.ComponentModel.Design;
using System.Data;
using System.Xml.Linq;
using Microsoft.Win32;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;

namespace BingoManager
{
    public partial class MainView : Form
    {
        private string selectedImagePath;
        private ToolTip toolTip;
        private List<DataRow> allCompaniesList = new List<DataRow>();
        private LogoView logoDisplayForm;
        private readonly string appDataPath;
        private readonly string imageFolderPath;
        private System.Windows.Forms.ToolTip _toolTip; 
        private readonly Random _rng = new Random();
        Image defaultImage;

        public class ElementTag
        {
            public ElementModel Element { get; set; }
            public string Column { get; set; }
        }
        public MainView()
        {
            InitializeComponent();

            _toolTip = new System.Windows.Forms.ToolTip
            {
                AutoPopDelay = 5000,  // quanto tempo o bal�o fica vis�vel
                InitialDelay = 500,   // antes do primeiro aparecimento
                ReshowDelay = 100,   // antes de reaparecer
                ShowAlways = true   // mesmo sem foco
            };

            toolTip = new ToolTip
            {
                AutoPopDelay = 0,
                InitialDelay = 0,
                ReshowDelay = 500,
                ShowAlways = true
            };

            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            imageFolderPath = Path.Combine(appDataPath, "BingoManager", "Images");

            // Subscri��o ao evento para detectar mudan�as nos monitores
            SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

            LoadGame();
        }

        //M�todo para mostrar Segunda Tela
        private void ShowLogoOnSecondScreen()
        {
            // Verificar se existem v�rias telas conectadas
            if (Screen.AllScreens.Length > 1)
            {
                // Exibir todas as telas conectadas (para fins de depura��o)
                foreach (var screen in Screen.AllScreens)
                {
                    Console.WriteLine($"Screen: {screen.DeviceName}, Resolution: {screen.Bounds.Width}x{screen.Bounds.Height}");
                }

                // Usa a segunda tela (�ndice 1, j� que a primeira � 0)
                Screen secondScreen = Screen.AllScreens[1];

                // Inicializa o formul�rio de exibi��o do logotipo se ainda n�o estiver criado
                if (logoDisplayForm == null || logoDisplayForm.IsDisposed)
                {
                    logoDisplayForm = new LogoView();
                }

                // Move o formul�rio para a segunda tela
                logoDisplayForm.StartPosition = FormStartPosition.Manual;
                logoDisplayForm.Location = secondScreen.WorkingArea.Location; // Define a localiza��o na segunda tela
                logoDisplayForm.WindowState = FormWindowState.Maximized; // Maximiza na segunda tela
                logoDisplayForm.Show();

                // Se j� houver uma imagem no PicPlayLogo, exibi-la na segunda tela
                if (picPlayLogo.Image != null)
                {
                    // Atualiza o logo e o nome da Elemento na segunda tela
                    logoDisplayForm.UpdateLogoAndName(picPlayLogo.Image, lblLastResult.Text);
                }
            }
            else
            {
                MessageBox.Show("A segunda tela n�o est� dispon�vel.");
            }
        }
        private void MainView_Load(object sender, EventArgs e)
        {
            // Inicializa a segunda tela ao carregar o formul�rio principal
            ShowLogoOnSecondScreen();
        }
        private void MainView_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cancelar a subscri��o do evento ao fechar o formul�rio
            SystemEvents.DisplaySettingsChanged -= new EventHandler(SystemEvents_DisplaySettingsChanged);
        }
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            // Verificar novamente as telas dispon�veis e mover o logo para a segunda tela
            ShowLogoOnSecondScreen();
        }


        //Jogar
        //M�todo para carregar todos os jogos para Jogar
        private void LoadGame()
        {
            btnRandom.Enabled = false;
            btnReset.Enabled = false;
            btnBingo.Enabled = false;

            int setId = 0; // �nico CardSet no banco tem ID = 0
            GameModel game = DataService.GetGameInfo(setId);

            if (game == null)
            {
                MessageBox.Show(
                    "Nenhum jogo encontrado para o CardSet ID = 0.\n" +
                    "Verifique se o arquivo de banco de dados cont�m exatamente um CardSet com SetId=0.",
                    "Erro ao carregar jogo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            string gameName = game.GameName;
            lblGameName.Text = gameName;

            string cardsTotal = game.GameTotal.ToString();
            lblCardsQnt.Text = cardsTotal + " cartelas";

            DisplayGamePanels(game);
        }

        // M�todo para mostrar as Elementos durante o jogo
        private void DisplayGamePanels(GameModel game)
        {
            int buttonSize = 35;
            int panelWidth = flwPlayB.Width;
            int buttonsPerRow = panelWidth / buttonSize;

            int Number = 1;

            flwPlayB.Controls.Clear();
            flwPlayI.Controls.Clear();
            flwPlayN.Controls.Clear();
            flwPlayG.Controls.Clear();
            flwPlayO.Controls.Clear();

            SetupFlowPanels();

            // Adicionar bot�es ao grupo B
            foreach (var element in game.BElements)
            {
                Button elementButton = new Button
                {
                    Text = Number.ToString(),
                    Tag = new ElementTag
                    {
                        Element = element,
                        Column = "B"
                    },
                    Width = buttonSize,
                    Height = buttonSize,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Enabled = false
                };

                // Define o tooltip para o nome da empresa:
                var tag = (ElementTag)elementButton.Tag;
                var elementModel = tag.Element;
                toolTip1.SetToolTip(elementButton, elementModel.CardName);

                Number++;
                elementButton.Click += elementButton_Click;
                flwPlayB.Controls.Add(elementButton);
            }

            // Adicionar bot�es ao grupo I
            foreach (var element in game.IElements)
            {
                Button elementButton = new Button
                {
                    Text = Number.ToString(),
                    Tag = new ElementTag
                    {
                        Element = element,
                        Column = "B"
                    },
                    Width = buttonSize,
                    Height = buttonSize,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Enabled = false
                };

                // Define o tooltip para o nome da empresa:
                var tag = (ElementTag)elementButton.Tag;
                var elementModel = tag.Element;
                toolTip1.SetToolTip(elementButton, elementModel.CardName);
                Number++;
                elementButton.Click += elementButton_Click;
                flwPlayI.Controls.Add(elementButton);
            }

            // Adicionar bot�es ao grupo N
            foreach (var element in game.NElements)
            {
                Button elementButton = new Button
                {
                    Text = Number.ToString(),
                    Tag = new ElementTag
                    {
                        Element = element,
                        Column = "B"
                    },
                    Width = buttonSize,
                    Height = buttonSize,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Enabled = false
                };

                // Define o tooltip para o nome da empresa:
                var tag = (ElementTag)elementButton.Tag;
                var elementModel = tag.Element;
                toolTip1.SetToolTip(elementButton, elementModel.CardName);
                Number++;
                elementButton.Click += elementButton_Click;
                flwPlayN.Controls.Add(elementButton);
            }

            // Adicionar bot�es ao grupo G
            foreach (var element in game.GElements)
            {
                Button elementButton = new Button
                {
                    Text = Number.ToString(),
                    Tag = new ElementTag
                    {
                        Element = element,
                        Column = "B"
                    },
                    Width = buttonSize,
                    Height = buttonSize,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Enabled = false
                };

                // Define o tooltip para o nome da empresa:
                var tag = (ElementTag)elementButton.Tag;
                var elementModel = tag.Element;
                toolTip1.SetToolTip(elementButton, elementModel.CardName);
                Number++;
                elementButton.Click += elementButton_Click;
                flwPlayG.Controls.Add(elementButton);
            }

            // Adicionar bot�es ao grupo O
            foreach (var element in game.OElements)
            {
                Button elementButton = new Button
                {
                    Text = Number.ToString(),
                    Tag = new ElementTag
                    {
                        Element = element,
                        Column = "B"
                    },
                    Width = buttonSize,
                    Height = buttonSize,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Enabled = false
                };

                // Define o tooltip para o nome da empresa:
                var tag = (ElementTag)elementButton.Tag;
                var elementModel = tag.Element;
                toolTip1.SetToolTip(elementButton, elementModel.CardName);
                Number++;
                elementButton.Click += elementButton_Click;
                flwPlayO.Controls.Add(elementButton);
            }

            lblResults.Text = "Jogo pronto! Aperte o bot�o COME�AR para iniciar a partida! Lembre-se de mudar o modo de jogo conforme desejar.";
        }

        // Configurando os FlowLayoutPanels
        private void SetupFlowPanels()
        {
            foreach (var flow in new[] { flwPlayB, flwPlayI, flwPlayN, flwPlayG, flwPlayO})
            {
                flow.FlowDirection = FlowDirection.LeftToRight;
                flow.WrapContents = true;
                flow.AutoSize = false;
                flow.AutoScroll = true;
                flow.Dock = DockStyle.Fill;
                flow.Padding = new Padding(5);
                flow.Margin = new Padding(0);
            }
        }

        // M�todo para come�ar o jogo
        private void btnStart_Click(object sender, EventArgs e)
        {

            if (File.Exists("Images/.Capa.png"))
            {
                defaultImage = Image.FromFile("Images/.Capa.png");
                picPlayLogo.Image = defaultImage;
                lblLastResult.Text = "Jogo Pausado";

                if (logoDisplayForm != null && logoDisplayForm.Visible)
                {
                    logoDisplayForm.UpdateLogoAndName(defaultImage, "Bingo!");
                }
            }
            else
            {
                defaultImage = Image.FromFile("Images/default_logo.jpg");
                lblLastResult.Text = "Capa n�o encontrada";
                picPlayLogo.Image = defaultImage;

                if (logoDisplayForm != null && logoDisplayForm.Visible)
                {
                    logoDisplayForm.UpdateLogoAndName(defaultImage, "Bingo!");
                }
            }

            btnReset.Enabled = true;
            btnStart.Enabled = false;
            btnBingo.Enabled = true;

            rdManual.Enabled = false;
            rdDigital.Enabled = false;

            if (rdDigital.Checked)
            {
                btnRandom.Enabled = true;

                foreach (Button btn in flwPlayB.Controls.OfType<Button>()) btn.BackColor = Color.White;
                foreach (Button btn in flwPlayI.Controls.OfType<Button>()) btn.BackColor = Color.White;
                foreach (Button btn in flwPlayN.Controls.OfType<Button>()) btn.BackColor = Color.White;
                foreach (Button btn in flwPlayG.Controls.OfType<Button>()) btn.BackColor = Color.White;
                foreach (Button btn in flwPlayO.Controls.OfType<Button>()) btn.BackColor = Color.White;
            }
            else if (rdManual.Checked)
            {
                // Habilita todos os bot�es em cada FlowLayoutPanel
                foreach (Control ctl in flwPlayB.Controls)
                    if (ctl is Button btnB) btnB.Enabled = true;

                foreach (Control ctl in flwPlayI.Controls)
                    if (ctl is Button btnI) btnI.Enabled = true;

                foreach (Control ctl in flwPlayN.Controls)
                    if (ctl is Button btnN) btnN.Enabled = true;

                foreach (Control ctl in flwPlayG.Controls)
                    if (ctl is Button btnG) btnG.Enabled = true;

                foreach (Control ctl in flwPlayO.Controls)
                    if (ctl is Button btnO) btnO.Enabled = true;
            }
        }

        private void elementButton_Click(object sender, EventArgs e)
        {
            int bingoPhase = rdLine.Checked ? 1 : (rdFull.Checked ? 2 : 0);
            string colun = null;

            Button clickedButton = sender as Button;

            // Reconhecer Coluna
            var parentPanel = clickedButton.Parent as FlowLayoutPanel;
            if (parentPanel != null)
            {
                if (parentPanel == flwPlayB)
                    colun = "B";
                else if (parentPanel == flwPlayI)
                    colun = "I";
                else if (parentPanel == flwPlayN)
                    colun = "N";
                else if (parentPanel == flwPlayG)
                    colun = "G";
                else if (parentPanel == flwPlayO)
                    colun = "O";
            }

            if (clickedButton != null && clickedButton.Tag is ElementTag tag)
            {
                ElementModel selectedElement = tag.Element;

                string numero = clickedButton.Text;
                Image elementImage = null;

                // Se houver nome de arquivo no selectedElement.Logo, tenta carregar; sen�o, n�o carrega nada
                if (!string.IsNullOrEmpty(selectedElement.ImageName))
                {
                    elementImage = DataService.LoadImageFromFile(selectedElement.Id);
                }

                // Marca o bot�o como sorteado, alterando a cor para vermelho
                clickedButton.BackColor = Color.Red;

                // Adiciona a empresa sorteada ao servi�o de jogo
                PlayService.AddElement(selectedElement.Id);

                // Atualiza o logo na tela principal
                picPlayLogo.Image = elementImage;
                lblLastResult.Text = numero + "\r\nColuna " + colun + "\r\n" + selectedElement.CardName;

                // Atualiza o logo e nome na tela secund�ria
                if (logoDisplayForm != null && logoDisplayForm.Visible)
                {
                    logoDisplayForm.UpdateLogoAndName(elementImage, "Coluna " + colun + " - " + selectedElement.CardName);
                }

                // Buscar cartelas
                List<int> chosenCards = PlayService.CheckCards(selectedElement.Id);

                // Verificar bingo
                List<int> winningCards = PlayService.CheckBingo(chosenCards, bingoPhase, selectedElement.Id);
                if (winningCards.Count > 0)
                {
                    string winningCardsText = string.Join(", ", winningCards);
                    lblResults.Text = $"BINGO! Cartelas vencedoras: {winningCardsText}";
                }
                else
                {
                    lblResults.Text = "Sem bingo!";
                }

            }
        }

        //M�todo para reinicar o jogo
        private void btnRestart_Click(object sender, EventArgs e)
        {
            // Confirma��o do usu�rio
            var result = MessageBox.Show("Voc� tem certeza que deseja reiniciar o jogo?", "Confirma��o", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                picPlayLogo.Image = null;
                lblResults.Text = string.Empty;
                lblLastResult.Text = string.Empty;

                flwPlayB.Controls.Clear();
                flwPlayI.Controls.Clear();
                flwPlayN.Controls.Clear();
                flwPlayG.Controls.Clear();
                flwPlayO.Controls.Clear();

                rdManual.Enabled = true;
                rdDigital.Enabled = true;

                btnStart.Enabled = true;

                PlayService.ResetGame();

                if (File.Exists("Images/.Capa.png"))
                {
                    defaultImage = Image.FromFile("Images/.Capa.png");
                    picPlayLogo.Image = defaultImage;
                    lblLastResult.Text = "Jogo Pausado";

                    if (logoDisplayForm != null && logoDisplayForm.Visible)
                    {
                        logoDisplayForm.UpdateLogoAndName(defaultImage, "Bingo!");
                    }
                }
                else
                {
                    defaultImage = Image.FromFile("Images/default_logo.jpg");
                    lblLastResult.Text = "Capa n�o encontrada";
                    picPlayLogo.Image = defaultImage;

                    if (logoDisplayForm != null && logoDisplayForm.Visible)
                    {
                        logoDisplayForm.UpdateLogoAndName(defaultImage, "Bingo!");
                    }
                }

                LoadGame();
            }
        }

        // M�todo para sortear uma Elemento no modo Digital e verificar as cartelas sorteadas e vencedoras
        private void btnRandom_Click(object sender, EventArgs e)
        {
            int bingoPhase = rdLine.Checked ? 1 : (rdFull.Checked ? 2 : 0);

            string colun = null;

            // Coleta todas as empresas dispon�veis para sorteio
            var avaliableElements = new List<Button>();

            // Adiciona os labels de todas as colunas ao availableCompanies, que ainda n�o foram sorteados (brancos)
            avaliableElements.AddRange(flwPlayB.Controls.OfType<Button>().Where(lbl => lbl.BackColor == Color.White));
            avaliableElements.AddRange(flwPlayI.Controls.OfType<Button>().Where(lbl => lbl.BackColor == Color.White));
            avaliableElements.AddRange(flwPlayN.Controls.OfType<Button>().Where(lbl => lbl.BackColor == Color.White));
            avaliableElements.AddRange(flwPlayG.Controls.OfType<Button>().Where(lbl => lbl.BackColor == Color.White));
            avaliableElements.AddRange(flwPlayO.Controls.OfType<Button>().Where(lbl => lbl.BackColor == Color.White));

            // Verifica se ainda h� empresas dispon�veis para sortear
            if (avaliableElements.Count == 0)
            {
                MessageBox.Show("Todos os Elementos j� foram sorteados.");
                return;
            }

            // Seleciona aleatoriamente uma empresa dispon�vel
            int randomIndex = _rng.Next(avaliableElements.Count);
            var selectedButton = avaliableElements[randomIndex];

            var parentPanel = selectedButton.Parent as FlowLayoutPanel;
            if (parentPanel != null)
            {
                if (parentPanel == flwPlayB)
                    colun = "B";
                else if (parentPanel == flwPlayI)
                    colun = "I";
                else if (parentPanel == flwPlayN)
                    colun = "N";
                else if (parentPanel == flwPlayG)
                    colun = "G";
                else if (parentPanel == flwPlayO)
                    colun = "O";
            }

            // Muda a cor do label sorteado para vermelho (marca como sorteado)
            selectedButton.BackColor = Color.Red;

            // Atualiza a l�gica do jogo para adicionar a empresa sorteada e remover da lista de sorteio
            if (selectedButton.Tag is ElementTag tag)
            {
                var selectedElement = tag.Element;

                // Adiciona a empresa � lista de sorteadas no PlayService
                PlayService.AddElement(selectedElement.Id);

                Image elementImage = null;

                // Se houver nome de arquivo no selectedElement.Logo, tenta carregar; sen�o, n�o carrega nada
                if (!string.IsNullOrEmpty(selectedElement.ImageName))
                {
                    elementImage = DataService.LoadImageFromFile(selectedElement.Id);
                }

                // Atualiza o logo na tela principal
                picPlayLogo.Image = elementImage;
                lblLastResult.Text = colun + " - " + selectedElement.CardName;

                // Atualiza o logo na segunda tela, se estiver vis�vel
                if (logoDisplayForm != null && logoDisplayForm.Visible)
                {
                    logoDisplayForm.UpdateLogoAndName(elementImage, colun + " - " + selectedElement.CardName);
                }

                // Buscar cartelas
                List<int> cardNumbers = PlayService.CheckCards(selectedElement.Id);

                // Verificar bingo
                List<int> winningCards = PlayService.CheckBingo(cardNumbers, bingoPhase, selectedElement.Id);
                if (winningCards.Count > 0)
                {
                    string winningCardsText = string.Join(", ", winningCards);
                    lblResults.Text = $"BINGO! Cartelas vencedoras: {winningCardsText}";
                }
                else
                {
                    lblResults.Text = "Sem bingo!";
                }
            }
        }

        // M�todo para pausar o jogo
        private void btnBingo_Click(object sender, EventArgs e)
        {
            if (File.Exists("Images/.Capa.png"))
            {
                defaultImage = Image.FromFile("Images/.Capa.png");
                picPlayLogo.Image = defaultImage;
                lblLastResult.Text = "Jogo Pausado";

                if (logoDisplayForm != null && logoDisplayForm.Visible)
                {
                    logoDisplayForm.UpdateLogoAndName(defaultImage, "Bingo!");
                }
            } else
            {
                defaultImage = Image.FromFile("Images/default_logo.jpg");
                lblLastResult.Text = "Capa n�o encontrada";
                picPlayLogo.Image = defaultImage;

                if (logoDisplayForm != null && logoDisplayForm.Visible)
                {
                    logoDisplayForm.UpdateLogoAndName(defaultImage, "Bingo!");
                }
            }
        }
    }
}
