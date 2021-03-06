﻿namespace Integracje.UI.ViewModel
﻿using Prism.Mvvm;
using System.Windows.Navigation;
using System;

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