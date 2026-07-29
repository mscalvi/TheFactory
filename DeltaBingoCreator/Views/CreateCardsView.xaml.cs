using BingoCreator.Services;
using DeltaBingoCreator.Database;
using DeltaBingoCreator.Models;
using DeltaBingoCreator.Services;
using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeltaBingoCreator.Views
{
    public partial class CreateCardsView : UserControl
    {

        private NavService NavService;

        public CreateCardsView(NavService navService)
        {
            InitializeComponent();

            NavService = navService;

            InitializeComboBox();
        }

        private void ReturnCreate(object sender, RoutedEventArgs e)
        {
            var createView = new CreateView(NavService);

            NavService.NavigateTo(createView);
        }

        private void InitializeComboBox()
        {
            var dtLists = FinderDataBase.GetLists();
            var lists = dtLists.AsEnumerable()
                .Select(r => new ListModel
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Name = r["Name"]?.ToString() ?? "",
                    Description = r["Description"]?.ToString() ?? "",
                    ImageName = r["ImageName"]?.ToString() ?? ""
                }).ToArray();

            ComboBoxList.Items.Clear();

            foreach (var lm in lists)
            {
                ComboBoxList.Items.Add(lm);
            }
            ComboBoxList.DisplayMemberPath = "Name";

            var themeOptions = ThemeCatalog.All
                .Select(k => new ThemeOption { Key = k.Key, Name = k.Value.DisplayName })
                .ToList();
            ComboBoxColor.DisplayMemberPath = "Name";
            ComboBoxColor.SelectedValuePath = "Key";
            ComboBoxColor.ItemsSource = themeOptions;

            ComboBoxModel.DisplayMemberPath = "Text";
            ComboBoxModel.SelectedValuePath = "Value";
            ComboBoxModel.ItemsSource = new[] {
                new { Text = "Quadradas (fundo branco)",     Value = "SQUARE"  },
                new { Text = "Arredondadas (fundo do tema)", Value = "ROUNDED" }
            };
            ComboBoxModel.SelectedValue = "SQUARE";

            ComboBoxHeader.DisplayMemberPath = "Text";
            ComboBoxHeader.SelectedValuePath = "Value";
            ComboBoxHeader.ItemsSource = new[] {
                new { Text = "SORTE", Value = "SORTE" },
                new { Text = "BINGO", Value = "BINGO" }
            };
            ComboBoxHeader.SelectedValue = "SORTE";

            ComboBoxSize.DisplayMemberPath = "Text";
            ComboBoxSize.SelectedValuePath = "Value";
            ComboBoxSize.ItemsSource = new[] {
                new { Text = "4x4", Value = "4" },
                new { Text = "5x5", Value = "5" }
            };
            ComboBoxSize.SelectedValue = "5";
        }


        // Helpers
        private sealed class ThemeOption
        {
            public string Key { get; init; }
            public string Name { get; init; }
        }
    }
}
