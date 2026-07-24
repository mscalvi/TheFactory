using DeltaBingoCreator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class CreateView : UserControl
    {
        private NavService NavService;

        public CreateView(NavService navService)
        {
            InitializeComponent();

            NavService = navService;
        }

        public void CreateItemViewOpen(object sender, RoutedEventArgs e)
        {
            var createItemView = new CreateItemView(NavService);

            NavService.NavigateTo(createItemView);
        }

        public void CreateListViewOpen(object sender, RoutedEventArgs e)
        {
            var createListView = new CreateListView(NavService);

            NavService.NavigateTo(createListView);
        }

        public void CreateCardsViewOpen(object sender, RoutedEventArgs e)
        {
            var createView = new CreateView(NavService);

            NavService.NavigateTo(createView);
        }

        private void ReturnMain(object sender, RoutedEventArgs e)
        {
            var mainView = new MainView(NavService);

            NavService.NavigateTo(mainView);
        }
    }
}
