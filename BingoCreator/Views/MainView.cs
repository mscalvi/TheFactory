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

            CreatePageLoad();
        }

        private sealed class ThemeOption
        {
            public string Key { get; init; }
            public string Name { get; init; }
        }

        // Métodos de Carregamento
        // Método para carregar as ComboBox
        private void CreatePageLoad()
        {
            // Listas para criação
            var dtLists = DataService.GetLists();
            var lists = dtLists.AsEnumerable()
                .Select(r => new ListModel
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Name = r["Name"]?.ToString() ?? "",
                    Description = r["Description"]?.ToString() ?? "",
                    ImageName = r["ImageName"]?.ToString() ?? ""
                }).ToArray();

            cboElementList.Items.Clear();
            cboCardsList.Items.Clear();
            foreach (var lm in lists)
            {
                cboElementList.Items.Add(lm);
                cboCardsList.Items.Add(lm);
            }
            cboElementList.DisplayMember = "Name";
            cboCardsList.DisplayMember = "Name";

            // Temas / modelo / cabeçalho (como você já tinha)
            var themeOptions = ThemeCatalog.All
                .Select(k => new ThemeOption { Key = k.Key, Name = k.Value.DisplayName })
                .ToList();
            cboCardsTheme.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCardsTheme.DisplayMember = "Name";
            cboCardsTheme.ValueMember = "Key";
            cboCardsTheme.DataSource = themeOptions;

            cboCardsModel.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCardsModel.DisplayMember = "Text";
            cboCardsModel.ValueMember = "Value";
            cboCardsModel.DataSource = new[] {
        new { Text = "Quadradas (fundo branco)",     Value = "SQUARE"  },
        new { Text = "Arredondadas (fundo do tema)", Value = "ROUNDED" }
    };
            cboCardsModel.SelectedValue = "SQUARE";

            cboCardsHeader.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCardsHeader.DisplayMember = "Text";
            cboCardsHeader.ValueMember = "Value";
            cboCardsHeader.DataSource = new[] {
        new { Text = "SORTE", Value = "SORTE" },
        new { Text = "BINGO", Value = "BINGO" }
    };
            cboCardsHeader.SelectedValue = "SORTE";
        }
        private void EditPageLoad()
        {
            // cbo1
            cboEdit1.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEdit1.Items.Clear();
            cboEdit1.Items.AddRange(new[] { "Conjuntos", "Listas", "Elementos" });

            // cbo2 / cbo3
            cboEdit2.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEdit3.DropDownStyle = ComboBoxStyle.DropDownList;

            // desinscreve e reinscreve (evita múltiplos handlers)
            cboEdit1.SelectedIndexChanged -= cboEdit1_SelectedIndexChanged;
            cboEdit2.SelectedIndexChanged -= cboEdit2_SelectedIndexChanged;
            cboEdit3.SelectedIndexChanged -= cboEdit3_SelectedIndexChanged;

            cboEdit1.SelectedIndexChanged += cboEdit1_SelectedIndexChanged;
            cboEdit2.SelectedIndexChanged += cboEdit2_SelectedIndexChanged;
            cboEdit3.SelectedIndexChanged += cboEdit3_SelectedIndexChanged;

            cboEdit1.SelectedIndex = -1;
            cboEdit2.DataSource = null; cboEdit2.Enabled = false;
            cboEdit3.DataSource = null; cboEdit3.Enabled = false;

            ClearEditFields();
        }
        private void cboEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearEditFields();

            var sel = cboEdit1.SelectedItem?.ToString();
            cboEdit2.DataSource = null; cboEdit2.Enabled = false;
            cboEdit3.DataSource = null; cboEdit3.Enabled = false;

            if (string.IsNullOrEmpty(sel)) return;

            switch (sel)
            {
                case "Conjuntos":
                    {
                        var dt = DataService.GetAllCardSets();
                        cboEdit2.DisplayMember = "Name";
                        cboEdit2.ValueMember = "Id";
                        cboEdit2.DataSource = dt;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit2.Enabled = true;
                        break;
                    }
                case "Listas":
                    {
                        var dt = DataService.GetLists();
                        cboEdit2.DisplayMember = "Name";
                        cboEdit2.ValueMember = "Id";
                        cboEdit2.DataSource = dt;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit2.Enabled = true;
                        break;
                    }
                case "Elementos":
                    {
                        var dt = DataService.GetAllElements(); // ordenado por Name
                        cboEdit2.DisplayMember = "Name";
                        cboEdit2.ValueMember = "Id";
                        cboEdit2.DataSource = dt;
                        cboEdit2.SelectedIndex = -1;
                        cboEdit2.Enabled = true;

                        // Situação 3: cbo3 desabilitada
                        cboEdit3.DataSource = null;
                        cboEdit3.Enabled = false;
                        break;
                    }
            }
        }
        private void cboEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearEditFields();

            var sel1 = cboEdit1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(sel1)) return;

            if (cboEdit2.SelectedValue == null || !int.TryParse(cboEdit2.SelectedValue.ToString(), out int id))
            {
                cboEdit3.DataSource = null; cboEdit3.Enabled = false;
                return;
            }

            switch (sel1)
            {
                case "Conjuntos":
                    {
                        // carrega set e elementos dele
                        var set = DataService.GetCardSetById(id);
                        var items = new List<(int Id, string Name, string Kind)>();

                        // 1º o próprio conjunto
                        items.Add((set.Id, set.Name, "SET"));

                        // depois os elementos do conjunto (use os GroupB.. ou AllElements)
                        var elems = (set.CardsSize == 5)
                            ? new[] { set.GroupB, set.GroupI, set.GroupN, set.GroupG, set.GroupO }
                                .Where(g => g != null).SelectMany(g => g).ToList()
                            : (set.AllElements ?? new List<ElementModel>());
                        foreach (var el in elems.DistinctBy(x => x.Id))
                            items.Add((el.Id, el.CardName?.Trim().Length > 0 ? el.CardName : el.Name, "ELE"));

                        cboEdit3.DisplayMember = "Name";
                        cboEdit3.ValueMember = "Id";
                        cboEdit3.DataSource = items.Select(t => new { t.Id, t.Name, t.Kind }).ToList();
                        cboEdit3.SelectedIndex = 0;
                        cboEdit3.Enabled = true;

                        ApplySetFields(set);
                        break;
                    }

                case "Listas":
                    {
                        // carrega lista + elementos da lista
                        var list = DataService.GetListById(id);
                        var rows = DataService.GetElementsInList(id);

                        var items = new List<(int Id, string Name, string Kind)>();
                        items.Add((list.Id, list.Name, "LIST"));

                        foreach (var r in rows)
                        {
                            int eid = Convert.ToInt32(r["Id"]);
                            string name = r["CardName"]?.ToString();
                            if (string.IsNullOrWhiteSpace(name))
                                name = r["Name"]?.ToString() ?? "";
                            items.Add((eid, name, "ELE"));
                        }

                        cboEdit3.DisplayMember = "Name";
                        cboEdit3.ValueMember = "Id";
                        cboEdit3.DataSource = items.Select(t => new { t.Id, t.Name, t.Kind }).ToList();
                        cboEdit3.SelectedIndex = 0; // própria lista
                        cboEdit3.Enabled = true;

                        // Preenche campos da lista
                        ApplyListFields(list, rows.Count);
                        break;
                    }

                case "Elementos":
                    {
                        // Sem “filhos”: preenche direto pelo elemento
                        var row = DataService.GetElementById(id);
                        if (row != null)
                        {
                            var em = new ElementModel
                            {
                                Id = id,
                                Name = row["Name"]?.ToString() ?? "",
                                CardName = row["CardName"]?.ToString() ?? "",
                                Note1 = row.Table.Columns.Contains("Note1") ? row["Note1"]?.ToString() ?? "" : "",
                                Note2 = row.Table.Columns.Contains("Note2") ? row["Note2"]?.ToString() ?? "" : ""
                            };
                            ApplyElementFields(em);
                        }
                        cboEdit3.DataSource = null;
                        cboEdit3.Enabled = false;
                        break;
                    }
            }
        }
        private void cboEdit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sel1 = cboEdit1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(sel1)) return;
            if (cboEdit2.SelectedValue == null || !int.TryParse(cboEdit2.SelectedValue.ToString(), out int parentId)) return;

            var data = cboEdit3.SelectedItem;
            if (data == null) return;

            var propId = data.GetType().GetProperty("Id");
            var propKind = data.GetType().GetProperty("Kind");
            int id = (int)(propId?.GetValue(data) ?? 0);
            string kind = propKind?.GetValue(data)?.ToString() ?? "";

            if (sel1 == "Conjuntos")
            {
                if (kind == "SET")
                {
                    var set = DataService.GetCardSetById(id);
                    ApplySetFields(set);
                }
                else
                {
                    var row = DataService.GetElementById(id);
                    if (row != null)
                    {
                        var em = new ElementModel
                        {
                            Id = id,
                            Name = row["Name"]?.ToString() ?? "",
                            CardName = row["CardName"]?.ToString() ?? "",
                            Note1 = row.Table.Columns.Contains("Note1") ? row["Note1"]?.ToString() ?? "" : "",
                            Note2 = row.Table.Columns.Contains("Note2") ? row["Note2"]?.ToString() ?? "" : ""
                        };
                        ApplyElementFields(em);
                    }
                }
            }
            else if (sel1 == "Listas")
            {
                if (kind == "LIST")
                {
                    var list = DataService.GetListById(id);
                    int count = DataService.GetElementsInList(id).Count;
                    ApplyListFields(list, count);
                }
                else // "ELE"
                {
                    var row = DataService.GetElementById(id);
                    if (row != null)
                    {
                        var em = new ElementModel
                        {
                            Id = id,
                            Name = row["Name"]?.ToString() ?? "",
                            CardName = row["CardName"]?.ToString() ?? "",
                            Note1 = row.Table.Columns.Contains("Note1") ? row["Note1"]?.ToString() ?? "" : "",
                            Note2 = row.Table.Columns.Contains("Note2") ? row["Note2"]?.ToString() ?? "" : ""
                        };
                        ApplyElementFields(em);
                    }
                }
            }
        }
        private void ClearEditFields()
        {
            lblEditText1.Text = lblEditText2.Text = lblEditText3.Text = lblEditText4.Text = lblEditText5.Text = "";
            boxEditText1.Text = boxEditText2.Text = boxEditText3.Text = boxEditText4.Text = boxEditText5.Text = "";

            // mostra tudo por padrão
            SetFieldVisible(1, false);
            SetFieldVisible(2, false);
            SetFieldVisible(3, false);
            SetFieldVisible(4, false);
            SetFieldVisible(5, false);
        }

        private void SetFieldVisible(int idx, bool visible)
        {
            switch (idx)
            {
                case 1: lblEditText1.Visible = boxEditText1.Visible = visible; break;
                case 2: lblEditText2.Visible = boxEditText2.Visible = visible; break;
                case 3: lblEditText3.Visible = boxEditText3.Visible = visible; break;
                case 4: lblEditText4.Visible = boxEditText4.Visible = visible; break;
                case 5: lblEditText5.Visible = boxEditText5.Visible = visible; break;
            }
        }
        private void ApplySetFields(CardSetModel set)
        {
            ClearEditFields();

            lblEditText1.Text = "Nome:";
            lblEditText2.Text = "Título:";
            lblEditText3.Text = "Anotação 1:";
            lblEditText4.Text = "Cartelas Criadas:";
            lblEditText5.Text = "Estilo";

            boxEditText1.Text = set.Name ?? "";
            boxEditText2.Text = set.Title ?? "";
            boxEditText3.Text = set.End ?? "";
            boxEditText4.Text = set.Quantity.ToString();
            boxEditText5.Text = $"{(set.Model ?? "SQUARE")}, {(set.Theme ?? "-")}";

            SetFieldVisible(1, true);
            SetFieldVisible(2, true);
            SetFieldVisible(3, true);
            SetFieldVisible(4, true);
            SetFieldVisible(5, true);
        }
        private void ApplyListFields(ListModel list, int elementCount)
        {
            ClearEditFields();

            lblEditText1.Text = "Nome:";
            lblEditText2.Text = "Descrição:";
            lblEditText3.Text = "Total de Elementos:";

            boxEditText1.Text = list?.Name ?? "";
            boxEditText2.Text = list?.Description ?? "";
            boxEditText3.Text = elementCount.ToString();

            UpdatePicEdit(list.ImageName);

            SetFieldVisible(1, true);
            SetFieldVisible(2, true);
            SetFieldVisible(3, true);

            // “os 2 campos não preenchidos devem sumir”
            SetFieldVisible(4, false);
            SetFieldVisible(5, false);
        }
        private void ApplyElementFields(ElementModel em)
        {
            ClearEditFields();

            lblEditText1.Text = "Nome:";
            lblEditText2.Text = "Nome para Cartela:";
            lblEditText3.Text = "Anotação 1:";
            lblEditText4.Text = "Anotação 2:";
            lblEditText5.Text = "Listas:";

            boxEditText1.Text = em?.Name ?? "";
            boxEditText2.Text = em?.CardName ?? "";
            boxEditText3.Text = em?.Note1 ?? "";
            boxEditText4.Text = em?.Note2 ?? "";

            UpdatePicEdit(em.ImageName);

            var lists = DataService.GetListsForElement(em.Id);
            boxEditText5.Text = lists;

            SetFieldVisible(1, true);
            SetFieldVisible(2, true);
            SetFieldVisible(3, true);
            SetFieldVisible(4, true);
            SetFieldVisible(5, true);
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

                    CreatePageLoad();
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
            CreatePageLoad();
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
            CreatePageLoad();
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


        // Helpers
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

        private void UpdatePicEdit(string imageName)
        {
            // Limpa imagem anterior (evita lock)
            if (picEdit.Image != null)
            {
                var old = picEdit.Image;
                picEdit.Image = null;
                old.Dispose();
            }

            if (string.IsNullOrWhiteSpace(imageName))
            {
                picEdit.Image = null;
                return;
            }

            // Resolva caminho: se não for absoluto, torne relativo ao app
            string path = imageName;
            if (!Path.IsPathRooted(path))
            {
                // Se no DB você salva só "images/..." relativo ao app:
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    imageName.Replace('/', Path.DirectorySeparatorChar));
            }

            if (!File.Exists(path))
            {
                picEdit.Image = null; // ou uma imagem “placeholder” se preferir
                return;
            }

            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    picEdit.Image = Image.FromStream(fs);
                }
                picEdit.SizeMode = PictureBoxSizeMode.Zoom; // ajuste se preferir StretchImage, etc.
            }
            catch
            {
                picEdit.Image = null;
            }
        }

    }
}
