using BingoCreator.Models;
using BingoCreator.Services;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
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

            cboCardsModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCardsModel.DisplayMember = "Text";
            cboCardsModel.ValueMember = "Value";
            cboCardsModel.DataSource = new[]
            {
                new { Text = "Quadradas (fundo branco)",           Value = "SQUARE"  },
                new { Text = "Arredondadas (fundo do tema)",       Value = "ROUNDED" }
            };
            cboCardsModel.SelectedValue = "SQUARE";

            cboCardsHeader.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCardsHeader.DisplayMember = "Text";
            cboCardsHeader.ValueMember = "Value";
            cboCardsHeader.DataSource = new[]
            {
                new { Text = "SORTE", Value = "SORTE" },
                new { Text = "BINGO", Value = "BINGO" }
            };
            cboCardsHeader.SelectedValue = "SORTE";
        }


        // Métodos de Criação
        // Criar uma Elemento
        private void btnElementCreat_Clicked(object sender, EventArgs e)
        {
            ElementModel element = new ElementModel();
            var list = new ListModel();

            element.Name = boxElementName.Text.Trim();
            element.CardName = boxElementCardName.Text.Trim();
            element.Note1 = boxElementNote1.Text.Trim();
            element.Note2 = boxElementNote1.Text.Trim();

            if (cboElementList.SelectedIndex > -1)
            {
                list = cboElementList.SelectedItem as ListModel;
            } else
            {
                list.Id = 0;
            }

            try
            {
                var creation = CreatingService.CreateElement(element, list); 

                lblElementMessage.Text = creation.Message;

                if (creation.Success)
                {
                    boxElementName.Text = "";
                    boxElementCardName.Text = "";
                    boxElementNote1.Text = "";
                    boxElementNote2.Text = "";
                    cboElementList.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                lblElementMessage.Text = "Erro inesperado ao criar elemento: " + ex.Message;
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

            try
            {
                var creation = CreatingService.CreateList(list);

                lblListMessage.Text = creation.Message;

                if (creation.Success)
                {
                    boxListName.Text = "";
                    boxListDescription.Text = "";

                    LoadLists();
                }
            }
            catch (Exception ex)
            {
                    lblListMessage.Text = "Erro inesperado ao criar lista: " + ex.Message;
            }
        }

        // Criar Cartelas
        private void btnExportCards_Click(object sender, EventArgs e)
        {
            btnCardsExport.Enabled = false;

            CardSetModel cards = new CardSetModel();

            cards.Name = boxCardsName.Text.Trim();
            cards.Title = boxCardsTitle.Text.Trim();
            cards.End = boxCardsEnd.Text.Trim();
            cards.Quantity = (int)boxCardsQuantity.Value;

            var list = cboCardsList.SelectedItem as ListModel;
            cards.ListId = list.Id;

            if (radCardsSize4.Checked)
            {
                cards.CardsSize = 4;
            } else
            {
                cards.CardsSize = 5;
            }

            cards.Theme = cboCardsTheme.SelectedValue as string ?? "MINIMAL";
            cards.Model = (cboCardsModel.SelectedValue as string) ?? "SQUARE";
            cards.Header = (cboCardsHeader.SelectedValue as string) ?? "SORTE";

            try
            {
                var creation = CreatingService.CreateCards(cards);

                if (creation.Success)
                {
                    boxCardsName.Text = string.Empty;
                    boxCardsQuantity.Value = 100;
                    boxCardsTitle.Text = string.Empty;
                    boxCardsEnd.Text = string.Empty;
                    cboCardsList.SelectedIndex = -1;
                    cboCardsTheme.SelectedIndex = -1;
                    btnCardsExport.Enabled = true;
                    radCardsSize5.Checked = true;
                }

                var printAns = MessageBox.Show(
                    "Deseja imprimir as cartelas agora?",
                    "Imprimir cartelas",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (printAns == DialogResult.Yes)
                {
                    try
                    {
                        PrintingService.PrintCards(creation.Id);
                        lblListMessage.Text = "PDFs gerados com sucesso";

                        var exportAns = MessageBox.Show(
                            "Deseja exportar o banco de dados do jogo também?",
                            "Exportar DB",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (exportAns == DialogResult.Yes)
                        {
                            try
                            {
                                ExportingService.ExportDataBase(creation.Id);
                                lblListMessage.Text = "Banco de dados exportado com sucesso";
                            }
                            catch (Exception ex)
                            {
                                lblListMessage.Text = "Erro inesperado ao exportar o banco de dados: " + ex.Message;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lblListMessage.Text = "Erro inesperado ao gerar PDFs: " + ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                lblListMessage.Text = "Erro inesperado ao criar cartelas: " + ex.Message;
            }
        }

        // Métodos de Importação
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

            // 1) Coleta arquivos de imagem (png primeiro, depois jpg e jpeg)
            var imageFiles = new[] { "*.png", "*.jpg", "*.jpeg" }
                .SelectMany(p => Directory.EnumerateFiles(folder, p))
                .ToList();

            if (imageFiles.Count == 0)
            {
                MessageBox.Show("Nenhuma imagem .png ou .jpg encontrada na pasta selecionada.",
                                "Importar Lista", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1.1) Localiza a capa (filename sem extensão == ".Capa")
            string coverFile = imageFiles
                .FirstOrDefault(f =>
                    Path.GetFileNameWithoutExtension(f)
                        .Equals(".Capa", StringComparison.OrdinalIgnoreCase)
                );

            // 2) Cria a lista no banco
            string coverImageName = coverFile != null
                ? Path.GetFileName(coverFile)
                : null;

            int listId;
            try
            {
                listId = DataService.CreateList(listName, description: "", imagename: coverImageName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Falha ao criar a lista \"{listName}\": {ex.Message}",
                                "Importar Lista", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3) Importa cada arquivo de elemento (todos exceto a capa),
            // registrando não importados e motivos
            var notImported = new List<(string Name, string Reason)>();
            var seenBaseNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int importedCount = 0;

            foreach (var file in imageFiles)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                if (fileNameNoExt.Equals(".Capa", StringComparison.OrdinalIgnoreCase))
                    continue; // pula a capa

                string baseName = fileNameNoExt?.Trim() ?? "";

                // Nome vazio ou só espaços
                if (string.IsNullOrWhiteSpace(baseName))
                {
                    notImported.Add((Path.GetFileName(file), "Nome vazio (arquivo com nome inválido)."));
                    continue;
                }

                // Duplicata dentro da própria pasta (mesmo baseName em png/jpg, etc.)
                if (!seenBaseNames.Add(baseName))
                {
                    notImported.Add((baseName, "Duplicado na pasta (mesmo nome-base)."));
                    continue;
                }

                // Tenta criar o elemento
                int elementId = 0;
                try
                {
                    elementId = DataService.CreateElement(
                        name: baseName,
                        cardName: baseName,
                        note1: "",
                        note2: "",
                        imageName: Path.GetFileName(file),
                        addTime: DateTime.Now.ToString("MMddyyyy - HH:mm:ss")
                    );

                    if (elementId <= 0)
                    {
                        notImported.Add((baseName, "Falha ao criar elemento (ID inválido retornado)."));
                        continue;
                    }
                }
                catch (Exception exCreate)
                {
                    // Se o backend lançar exceção por chave única/duplicata etc., a mensagem vai junto
                    notImported.Add((baseName, $"Erro ao criar: {exCreate.Message}"));
                    continue;
                }

                // Tenta associar na lista
                try
                {
                    DataService.AlocateElements(listId, new List<int> { elementId });
                    importedCount++;
                }
                catch (Exception exLink)
                {
                    // Elemento foi criado mas não conseguiu associar
                    notImported.Add((baseName, $"Criado, mas falha ao associar na lista: {exLink.Message}"));
                    // (Opcional) você poderia tentar desfazer a criação aqui, se tiver método para isso.
                }
            }

            // 4) Monta a mensagem final
            var sb = new StringBuilder();
            sb.AppendLine($"Importação concluída para a lista \"{listName}\".");
            sb.AppendLine($"Itens importados: {importedCount}/{imageFiles.Count - (coverFile != null ? 1 : 0)}");

            if (coverFile == null)
                sb.AppendLine("Atenção: arquivo de capa \".Capa\" não foi encontrado.");

            if (notImported.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Não importados (nome: motivo):");
                // Limita a 30 linhas para não estourar MessageBox; ajuste se quiser
                foreach (var (name, reason) in notImported.Take(30))
                    sb.AppendLine($"- {name}: {reason}");
                if (notImported.Count > 30)
                    sb.AppendLine($"... e mais {notImported.Count - 30} itens.");
            }

            MessageBox.Show(sb.ToString(), "Importar Lista", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Recarrega combobox de listas, se for o caso
            LoadLists();
        }

        // Importar Lista por TXT, remove acentos e caracteres não permitidos
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

        // Habilitar e Desabilitar Header
        private void radCardsSize4_CheckedChanged(object sender, EventArgs e)
        {
            cboCardsHeader.Enabled = false;
            cboCardsHeader.SelectedValue = -1;
        }

        private void radCardsSize5_CheckedChanged(object sender, EventArgs e)
        {
            cboCardsHeader.Enabled = true;
            cboCardsHeader.SelectedValue = 1;
        }
    }
}
