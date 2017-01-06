using Prism.Mvvm;
using System.Windows.Navigation;

namespace Integracje.UI.Base
{
    public class BaseViewModel : BindableBase
    {
        private NavigationService m_Navigator;

        public NavigationService Navigator
        {
            get { return m_Navigator; }
        }

        public void UpdateNavigationService(NavigationService navigator)
        {
            m_Navigator = navigator;
        }
    }
}
