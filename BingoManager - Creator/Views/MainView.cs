using BingoCreator.Models;
using BingoCreator.Services;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BingoCreator
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();

            DataService.InitializeDatabase();

            DesignService.UseDefaultLogo();
            LoadLists();
        }

        private sealed class ThemeOption
        {
            public string Key { get; init; }
            public string Name { get; init; }
        }

        // Métodos de Carregamento
        // Método para carregar as ComboBox de Listas
        private void LoadLists()
        {
            ListModel[] AllLists;
            {
                DataTable dt = DataService.GetLists();
                AllLists = dt.AsEnumerable()
                                  .Select(row => new ListModel
                                  {
                                      Id = Convert.ToInt32(row["Id"]),
                                      Name = row["Name"].ToString(),
                                      Description = row["Description"].ToString(),
                                      ImageName = row["ImageName"].ToString()
                                  })
                                  .ToArray();
            }

            cboElementList.Items.Clear();
            cboCardsList.Items.Clear();

            foreach (var lm in AllLists)
            {
                cboElementList.Items.Add(lm);
                cboCardsList.Items.Add(lm);
            }

            cboElementList.DisplayMember = "Name";
            cboCardsList.DisplayMember = "Name";

            var themeOptions = ThemeCatalog.All
                .Select(kvp => new ThemeOption { Key = kvp.Key, Name = kvp.Value.DisplayName })
                .ToList();

            cboCardsTheme.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCardsTheme.DisplayMember = "Name";
            cboCardsTheme.ValueMember = "Key";
            cboCardsTheme.DataSource = themeOptions;
        }


        // Métodos de Criação
        // Criar uma Elemento
        private void btnElementCreat_Clicked(object sender, EventArgs e)
        {
            ElementModel element = new ElementModel();

            element.Name = boxElementName.Text.Trim();
            element.CardName = boxElementCardName.Text.Trim();
            element.Note1 = boxElementNote1.Text.Trim();
            element.Note2 = boxElementNote1.Text.Trim();

            int maxNameLength = 100;
            int maxCardNameLength = 60;
            int maxNotesLength = 250;

            if (string.IsNullOrEmpty(element.Name) || string.IsNullOrEmpty(element.CardName))
            {
                lblElementMessage.Text = "Nome e Nome para Cartela são obrigatórios.";
                return;
            }

            if (element.Name.Length > maxNameLength)
            {
                lblElementMessage.Text = $"O nome do Elemento deve ter no máximo {maxNameLength} caracteres.";
                return;
            }

            if (element.CardName.Length > maxCardNameLength)
            {
                lblElementMessage.Text = $"O nome para cartela deve ter no máximo {maxCardNameLength} caracteres.";
                return;
            }

            if (element.Note1.Length > maxNotesLength)
            {
                lblElementMessage.Text = $"A anotação 1 deve ter no máximo {maxNotesLength} caracteres.";
                return;
            }

            if (element.Note2.Length > maxNotesLength)
            {
                lblElementMessage.Text = $"A anotação 2 deve ter no máximo {maxNotesLength} caracteres.";
                return;
            }

            try
            {
                string relativePath = Path.Combine("images", element.CardName + "_e.png");
                element.AddDate = DateTime.Now.ToString("MMddyyyy - HH:mm:ss");
                int elementId = DataService.CreateElement(element.Name, element.CardName, element.Note1, element.Note2, relativePath, element.AddDate);

                lblElementMessage.Text = "Elemento " + element.Name + " adicionado com sucesso.";

                if (cboElementList.SelectedIndex > -1)
                {
                    var list = cboElementList.SelectedItem as ListModel;
                    int selectedList = list.Id;

                    List<int> elements = new List<int>();
                    elements.Add(elementId);
                    
                    try
                    {
                        DataService.AlocateElements(selectedList, elements);

                        lblElementMessage.Text = "Elemento " + element.CardName + " adicionado com sucesso à Lista " + list.Name;
                    }
                    catch (Exception ex)
                    {
                        lblElementMessage.Text = "Erro ao adicionar o Elemento à Lista. " + ex.Message;
                    }
                }

                boxElementName.Text = "";
                boxElementCardName.Text = "";
                boxElementNote1.Text = "";
                boxElementNote2.Text = "";
                cboElementList.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                lblElementMessage.Text = "Erro ao adicionar o Elemento: " + ex.Message;
            }
        }

        // Criar Lista
        private void btnListCreate_Clicked(object sender, EventArgs e)
        {
            ListModel list = new ListModel();

            list.Name = boxListName.Text.ToUpper().Trim();
            list.Description = boxListDescription.Text.Trim();

            int maxNameLength = 100;
            int maxDescriptionLength = 300;

            if (!string.IsNullOrEmpty(list.Name))
            {
                if (list.Name.Length > maxNameLength)
                {
                    lblListMessage.Text = $"O nome da Lista deve ter no máximo {maxNameLength} caracteres.";
                    return;
                }

                if (list.Description.Length > maxDescriptionLength)
                {
                    lblListMessage.Text = $"A descrição da Lista deve ter no máximo {maxDescriptionLength} caracteres.";
                    return;
                }

                if (string.IsNullOrEmpty(list.Description))
                {
                    list.Description = "*";
                }

                try
                {
                    string relativePath = Path.Combine("images", list.Name + "_default.png");
                    DataService.CreateList(list.Name, list.Description, relativePath);

                    lblListMessage.Text = "Lista " + list.Name + " adicionada com sucesso.";
                    boxListName.Text = "";
                    boxListDescription.Text = "";
                }
                catch (Exception ex)
                {
                    lblListMessage.Text = "Erro ao conectar ao Banco de Dados: " + ex.Message;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(boxListName.Text))
                {
                    lblListMessage.Text = "Nome da Lista é obrigatório.";
                }
                else
                {
                    lblListMessage.Text = "Erro ao adicionar a Lista.";
                }
            }

            LoadLists();
        }

        // Importar Lista por Pasta de Imagens
        private void btnListImport_Clicked(object sender, EventArgs e)
        {

            using var fbd = new FolderBrowserDialog
            {
                Description = "Selecione a pasta contendo os arquivos da lista (capa .Capa e elementos)."
            };

            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            string folder = fbd.SelectedPath;
            string listName = Path.GetFileName(folder);

            // 1) Localiza o arquivo de capa (filename sem extensão == ".Capa")
            var imageFiles = Directory.EnumerateFiles(folder, "*.png")
                              .Concat(Directory.EnumerateFiles(folder, "*.jpg"))
                              .ToList();

            string coverFile = imageFiles
                .FirstOrDefault(f =>
                    Path.GetFileNameWithoutExtension(f)
                        .Equals(".Capa", StringComparison.OrdinalIgnoreCase)
                );

            // 2) Cria a lista no banco
            string coverImageName = coverFile != null
                ? Path.GetFileName(coverFile)
                : null;

            int listId = DataService.CreateList(listName, description: "", imagename: coverImageName);

            // 3) Importa cada arquivo de elemento (todos exceto a capa)
            foreach (var file in imageFiles)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                if (fileNameNoExt.Equals(".Capa", StringComparison.OrdinalIgnoreCase))
                    continue;   // pula o arquivo de capa

                // Nome base sem extensão
                string baseName = fileNameNoExt;

                // Grava o elemento no banco
                int elementId = DataService.CreateElement(
                    name: baseName,
                    cardName: baseName,
                    note1: "",
                    note2: "",
                    imageName: Path.GetFileName(file),
                    addTime: DateTime.Now.ToString("MMddyyyy - HH:mm:ss")
                );

                // Associa na lista
                DataService.AlocateElements(listId, new List<int> { elementId });
            }

            MessageBox.Show($"Importação concluída para a lista \"{listName}\"!",
                            "Importar Lista", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Recarrega combobox de listas, se for o caso
            LoadLists();
        }

        // Importar Lista por TXT, remove acentos (de preferência não ter acentos) e caracteres não permitidos
        private void btnListTxt_Clicked(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Selecione o arquivo de lista (TXT/CSV)",
                Filter = "Texto/CSV (*.txt;*.csv)|*.txt;*.csv|Todos os arquivos (*.*)|*.*",
                Multiselect = false,
                CheckFileExists = true
            };

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            string path = ofd.FileName;
            string listName = Path.GetFileNameWithoutExtension(path);

            string text;
            try
            {
                text = File.ReadAllText(path, Encoding.UTF8);
            }
            catch (DecoderFallbackException)
            {
                text = File.ReadAllText(path, Encoding.GetEncoding(1252)); // fallback pt-BR comum
            }

            // tokeniza: quebra por linha e também aceita vírgula, ponto-e-vírgula e TAB
            var rawTokens = text
                .Replace("\r\n", "\n").Replace("\r", "\n")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .SelectMany(l => l.Split(new[] { ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .Select(t => t.Trim());

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // dedup após limpeza
            var aprovados = new List<string>();
            var rejeitados = new List<string>();

            foreach (var raw in rawTokens)
            {
                var name = raw.Trim(); // sem limpeza de acentos/especiais

                if (string.IsNullOrWhiteSpace(name))
                    continue;

                if (name.Length > 50)
                {
                    rejeitados.Add($"{raw}  — > 50 caracteres");
                    continue;
                }

                if (!seen.Add(name)) // dedup case-insensitive do texto original
                    continue;

                aprovados.Add(name);
                
                
                //if (raw.Length == 0) continue;

                //var cleaned = CleanName(raw); // remove acentos/especiais; trim e normaliza espaços

                //if (string.IsNullOrWhiteSpace(cleaned))
                //{
                //    rejeitados.Add($"{raw}  — vazio após limpeza");
                //    continue;
                //}
                //if (cleaned.Length > 50)
                //{
                //    rejeitados.Add($"{raw}  — > 50 caracteres (limpo ficou com {cleaned.Length})");
                //    continue;
                //}
                //if (!seen.Add(cleaned)) // dedup por nome limpo
                //    continue;

                //aprovados.Add(cleaned);
            }

            // cria a lista sem imagem de capa
            int listId = DataService.CreateList(listName, description: "", imagename: null);

            foreach (var name in aprovados)
            {
                int elementId = DataService.CreateElement(
                    name: name,               // igual ao cardName
                    cardName: name,
                    note1: "",
                    note2: "",
                    imageName: null,          // sem imagem
                    addTime: DateTime.Now.ToString("MMddyyyy - HH:mm:ss")
                );

                DataService.AlocateElements(listId, new List<int> { elementId });
            }

            // feedback
            var msg = $"Lista \"{listName}\": {aprovados.Count} itens importados";
            if (rejeitados.Count > 0)
            {
                msg += $"\n{rejeitados.Count} rejeitados por regra.";
                // Mostra até 20 exemplos
                var exemplos = string.Join("\n", rejeitados.Take(20));
                MessageBox.Show($"{msg}\n\nExemplos de rejeitados:\n{exemplos}",
                                "Importar Lista", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show(msg, "Importar Lista", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // recarrega UI se necessário
            LoadLists();
        }

        private static string CleanName(string s)
        {
            s = RemoveDiacritics(s);
            // permite letras, dígitos, espaço, hífen e underscore
            s = Regex.Replace(s, @"[^\w \-]", ""); // \w = [A-Za-z0-9_]
            s = Regex.Replace(s, @"\s+", " ").Trim();
            return s;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(capacity: text.Length);
            foreach (var c in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        // Criar Cartelas
        private void btnExportCards_Click(object sender, EventArgs e)
        {
            CardSetModel cards = new CardSetModel();

            int maxNameLength = 50;
            int maxQuantity = 2000;
            int maxTitleLength = 120;
            int maxEndLength = 200;

            cards.Name = boxCardsName.Text.Trim();
            cards.Title = boxCardsTitle.Text.Trim();
            cards.End = boxCardsEnd.Text.Trim();

            if (int.TryParse(boxCardsQuant.Text.Trim(), out int quantidade))
            {
                cards.Quantity = quantidade;
                if (cards.Quantity > maxQuantity)
                {
                    lblCardsMessage.Text = $"Apenas números na quantidade! A quantidade máxima permitida é {maxQuantity}.";
                    return;
                }
            } else
            {
                lblCardsMessage.Text = $"Apenas números na quantidade! A quantidade máxima permitida é {maxQuantity}.";
                return;
            }

            if (string.IsNullOrEmpty(cards.Name) || cards.Name.Length > maxNameLength)
            {
                lblCardsMessage.Text = $"Insira um nome para o conjunto de cartelas com no máximo {maxNameLength} caracteres!";
                return;
            }

            if (string.IsNullOrEmpty(cards.Title) || cards.Title.Length > maxTitleLength)
            {
                lblCardsMessage.Text = $"Insira um título para as Cartelas com no máximo {maxTitleLength} caracteres!";
                return;
            }

            if (string.IsNullOrEmpty(cards.End) || cards.End.Length > maxEndLength)
            {
                lblCardsMessage.Text = $"O final deve ter no máximo {maxEndLength} caracteres!";
                return;
            }

            if (cboCardsList.SelectedItem != null)
            {
                var list = cboCardsList.SelectedItem as ListModel;
                cards.ListId = list.Id;

                List<DataRow> List = DataService.GetElementsInList(cards.ListId);

                int ElementCount = List.Count;

                if (radCardsSize4.Checked == true)
                {
                    cards.CardsSize = 4;

                    if (ElementCount < 35)
                    {
                        lblCardsMessage.Text = $"A Lista deve ter pelo menos 35 Elementos! A Lista {list.Name} tem {ElementCount}.";
                        return;
                    }
                }
                if (radCardsSize5.Checked == true)
                {
                    cards.CardsSize = 5;

                    if (ElementCount < 45)
                    {
                        lblCardsMessage.Text = $"A Lista deve ter pelo menos 45 Elementos! A Lista {list.Name} tem {ElementCount}.";
                        return;
                    }
                }

                var themeKey = cboCardsTheme.SelectedValue as string ?? "MINIMAL";
                cards.Theme = themeKey;

                try
                {
                    btnCardsExport.Enabled = false;
                    int generatedCards = GeneratingService.CreateCards(cards.ListId, cards.Name, cards.Title, cards.End, cards.Quantity, cards.CardsSize, cards.Theme);
                    lblCardsMessage.Text = "Cartelas criadas com sucesso.";

                    try
                    {
                        int generatedDatabase = GeneratingService.CreateDataBase(generatedCards, cards.CardsSize);
                        lblCardsMessage.Text = "Banco de dados gerado com sucesso";
                    }
                    catch
                    {
                        lblCardsMessage.Text = "Erro ao gerar banco de dados";
                    }

                    boxCardsName.Text = string.Empty;
                    boxCardsQuant.Text = string.Empty;
                    boxCardsTitle.Text = string.Empty;
                    boxCardsEnd.Text = string.Empty;
                    cboCardsList.SelectedIndex = -1;
                    cboCardsTheme.SelectedIndex = -1;
                    btnCardsExport.Enabled = true;
                    radCardsSize5.Checked = true;
                }
                catch
                {
                    lblCardsMessage.Text = "Erro ao gerar as cartelas.";
                    btnCardsExport.Enabled = true;
                }
                
            }
            else
            {
                lblCardsMessage.Text = "Selecione uma Lista!";
            }
        }
    }
}
