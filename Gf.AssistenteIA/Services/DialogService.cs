using System.Windows;

namespace Gf.AssistenteIA.Services
{
      public class DialogService : IDialogService
      {
            public bool Confirm(string message, string title)
            {
                  var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                  return result == MessageBoxResult.Yes;
            }

            public void Error(string message, Exception exception)
            {
                  MessageBox.Show(message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
      }
}
