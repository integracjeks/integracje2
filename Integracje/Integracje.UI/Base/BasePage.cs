using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Integracje.UI.Base
{
    public class BasePage : Page
    {
        #region Properties

        public NavigationService Navigator
        {
            get
            {
                return (Application.Current.MainWindow as MainWindow).Frame.NavigationService;
            }
        }

        public BaseViewModel ViewModel { get; private set; }

        #endregion Properties

        public BasePage()
        {
            DataContextChanged += BasePage_DataContextChanged;
        }

        private void BasePage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = e.NewValue as BaseViewModel;
            if (ViewModel != null)
            {
                ViewModel.UpdateNavigationService(Navigator);
            }
        }
    }
}
