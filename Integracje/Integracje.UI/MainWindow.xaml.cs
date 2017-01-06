using Integracje.UI.View;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Integracje.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            (Application.Current.MainWindow as MainWindow).Frame?.Navigate(new MainPage());
        }

        #endregion Constructors

        #region Methods

        public Frame Frame { get { return _mainFrame; } }

        #endregion Methods

        private void mainFrame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            try
            {
                var fadeId = this.FindResource("FadeIn") as Storyboard;
                fadeId.Begin();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}