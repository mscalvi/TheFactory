using System;
using System.Collections.Generic;
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
        private string _lastFilePath;
        public MainView()
        {
            InitializeComponent();
        }

        private void btnImportListCharacters_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Selecione o arquivo .xlsm",
                Filter = "Planilhas Excel (*.xlsm;*.xlsx)|*.xlsm;*.xlsx|Todos os arquivos (*.*)|*.*"
            };
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            var filePath = ofd.FileName;

            // 1) Importa personagens
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

            // 2) Gera e salva/imprime PDF
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

        private void btnImportListMaps_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Selecione o arquivo .xlsm",
                Filter = "Planilhas Excel (*.xlsm;*.xlsx)|*.xlsm;*.xlsx|Todos os arquivos (*.*)|*.*"
            };

            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            _lastFilePath = ofd.FileName;

            var maps = _importingService.ImportMapsList(_lastFilePath);
            _maps = maps ?? new List<MapModel>();

            // Feedback simples (ajuste para Label/StatusBar, se tiver)
            MessageBox.Show(
                this,
                $"Importadas {_maps.Count} cartas de Mapas, de:\n{_lastFilePath}",
                "Importação concluída",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
