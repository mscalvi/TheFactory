using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DeltaBingoCreator.Services
{
    public class NavService
    {
        private readonly ContentControl _content;

        public NavService(ContentControl content)
        {
            _content = content;
        }

        public void NavigateTo(UserControl view)
        {
            _content.Content = view;
        }
    }
}
