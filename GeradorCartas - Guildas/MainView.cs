using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using GeradorCartas___Guildas.Models;
using GeradorCartas___Guildas.Services;

namespace GeradorCartas___Guildas
{
    public partial class MainView : Form
    {
        private readonly ImportingService _importingService = new();
        private readonly PrintingService _printingService = new();

        private List<MapModel> _maps = new();
        private List<CharacterModel> _characters = new();
        private List<ActionsModel> _actions = new();
        private List<PersonalityModel> _personalities = new();
        private List<RelicModel> _relics = new();

        private string _lastFilePath;

        public MainView()
        {
            InitializeComponent();
        }

        // =========================
        // Characters (já existia)
        // =========================
        private void btnImportListCharacters_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Selecione o arquivo .xlsm",
                Filter = "Planilhas Excel (*.xlsm;*.xlsx)|*.xlsm;*.xlsx|Todos os arquivos (*.*)|*.*"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            var filePath = ofd.FileName;

            // 1) Importa
            List<CharacterModel> characters;
            try
            {
                characters = _importingService.ImportCharactersList(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao importar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (characters == null || characters.Count == 0)
            {
                MessageBox.Show(this, "Nenhum personagem encontrado.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2) Imprime
            try
            {
                _printingService.PrintCharacterCards(characters);
                MessageBox.Show(this, $"Gerado PDF com {characters.Count} cartas.", "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao gerar/imprimir: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // Maps (atualizado p/ imprimir)
        // =========================
        private void btnImportListMaps_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Selecione o arquivo .xlsm",
                Filter = "Planilhas Excel (*.xlsm;*.xlsx)|*.xlsm;*.xlsx|Todos os arquivos (*.*)|*.*"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            var filePath = ofd.FileName;

            // 1) Importa
            List<MapModel> maps;
            try
            {
                maps = _importingService.ImportMapsList(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao importar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (maps == null || maps.Count == 0)
            {
                MessageBox.Show(this, "Nenhum mapa encontrado.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _maps = maps;
            _lastFilePath = filePath;

            // 2) Imprime
            try
            {
                _printingService.PrintMapCards(maps);
                MessageBox.Show(this, $"Gerado PDF com {maps.Count} cartas de Mapa.", "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao gerar/imprimir: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // Actions
        // =========================
        private void btnImportListActions_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Selecione o arquivo .xlsm",
                Filter = "Planilhas Excel (*.xlsm;*.xlsx)|*.xlsm;*.xlsx|Todos os arquivos (*.*)|*.*"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            var filePath = ofd.FileName;

            // 1) Importa
            List<ActionsModel> actions;
            try
            {
                actions = _importingService.ImportActionsList(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao importar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (actions == null || actions.Count == 0)
            {
                MessageBox.Show(this, "Nenhuma carta de Ação encontrada.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _actions = actions;
            _lastFilePath = filePath;

            // 2) Imprime
            try
            {
                _printingService.PrintActionCards(actions);
                MessageBox.Show(this, $"Gerado PDF com {actions.Count} cartas de Ação.", "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao gerar/imprimir: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // Personalities
        // =========================
        private void btnImportListPersonalities_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Selecione o arquivo .xlsm",
                Filter = "Planilhas Excel (*.xlsm;*.xlsx)|*.xlsm;*.xlsx|Todos os arquivos (*.*)|*.*"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            var filePath = ofd.FileName;

            // 1) Importa
            List<PersonalityModel> personalities;
            try
            {
                personalities = _importingService.ImportPersonalitiesList(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao importar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (personalities == null || personalities.Count == 0)
            {
                MessageBox.Show(this, "Nenhuma carta de Personalidade encontrada.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _personalities = personalities;
            _lastFilePath = filePath;

            // 2) Imprime
            try
            {
                _printingService.PrintPersonalityCards(personalities);
                MessageBox.Show(this, $"Gerado PDF com {personalities.Count} cartas de Personalidade.", "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao gerar/imprimir: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // Relics
        // =========================
        private void btnImportListRelics_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Selecione o arquivo .xlsm",
                Filter = "Planilhas Excel (*.xlsm;*.xlsx)|*.xlsm;*.xlsx|Todos os arquivos (*.*)|*.*"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            var filePath = ofd.FileName;

            // 1) Importa
            List<RelicModel> relics;
            try
            {
                relics = _importingService.ImportRelicsList(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao importar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (relics == null || relics.Count == 0)
            {
                MessageBox.Show(this, "Nenhuma carta de Relíquia encontrada.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _relics = relics;
            _lastFilePath = filePath;

            // 2) Imprime
            try
            {
                _printingService.PrintCards(
                    relics,
                    r => "RelicModel1",
                    (r, field) => field.Equals("Art", StringComparison.OrdinalIgnoreCase)
                        ? "RelicModelArt.png"                // ← FORÇADO
                        : ReflectiveGet(r, field),           // ← ORIGINAL
                    outputName: "Relics",
                    title: "Guildas - Cartas (Relíquias)");

                MessageBox.Show(this, $"Gerado PDF com {relics.Count} cartas de Relíquia.", "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Erro ao gerar/imprimir: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========
        // Helper
        // =========
        private static string ReflectiveGet<T>(T obj, string field)
        {
            if (obj == null || string.IsNullOrWhiteSpace(field)) return string.Empty;
            var prop = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .FirstOrDefault(p => string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));
            if (prop == null) return string.Empty;

            var val = prop.GetValue(obj);
            if (val == null) return string.Empty;

            return val is IFormattable f
                ? f.ToString(null, CultureInfo.InvariantCulture)
                : val.ToString();
        }
    }
}
