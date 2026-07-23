using DeltaBingoCreator.Views;
using DeltaBingoCreator.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeltaBingoCreator
{
    public partial class MainWindow : Window
    {
        private NavService NavService;

        public MainWindow()
        {
            InitializeComponent();

            NavService = new NavService(MainContent);

            var mainView = new MainView(NavService);

            NavService.NavigateTo(mainView);
        }
    }
}