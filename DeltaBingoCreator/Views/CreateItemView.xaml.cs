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
    public partial class CreateItemView : UserControl
    {
        private NavService NavService;

        public CreateItemView(NavService navService)
        {
            InitializeComponent();

            NavService = navService;
        }

        private void ReturnCreate(object sender, RoutedEventArgs e)
        {
            var createView = new CreateView(NavService);

            NavService.NavigateTo(createView);
        }
    }
}
