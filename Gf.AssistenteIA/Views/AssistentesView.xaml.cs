using Gf.AssistenteIA.ViewModels;
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

namespace Gf.AssistenteIA.Views
{
      /// <summary>
      /// Interação lógica para AssistentesView.xam
      /// </summary>
      public partial class AssistentesView : UserControl
      {
            public AssistentesView()
            {
                  InitializeComponent();

                  Loaded += AssistentesView_Loaded;
            }

            private void AssistentesView_Loaded(object sender, RoutedEventArgs e)
            {
                  var model = (AssistentesViewModel)DataContext;
                  if (model == null) return;

                  model.Load();
            }
      }
}
