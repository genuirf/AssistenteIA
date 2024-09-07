using Gf.AssistenteIA.ViewModels;
using System.Windows;

namespace Gf.AssistenteIA
{
      /// <summary>
      /// Interaction logic for MainWindow.xaml
      /// </summary>
      public partial class MainWindow : Window
      {
            public MainWindow(MainViewModel mainViewModel)
            {
                  InitializeComponent();
                  DataContext = mainViewModel;
            }
      }
}