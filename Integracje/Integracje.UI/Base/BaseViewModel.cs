using Prism.Mvvm;
using System.Windows.Navigation;

namespace Integracje.UI.Base
{
    public class BaseViewModel : BindableBase
    {
        #region Properties

        public NavigationService Navigator
        {
            get { return m_Navigator; }
        }

        #endregion Properties

        #region Methods

        public void UpdateNavigationService(NavigationService navigator)
        {
            m_Navigator = navigator;
        }

        #endregion Methods

        #region Fields

        private NavigationService m_Navigator;

        #endregion Fields
    }
}