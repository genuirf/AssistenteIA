using Gf.AssistenteIA.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gf.AssistenteIA.Views
{
      /// <summary>
      /// Interação lógica para DocsAssistenteView.xam
      /// </summary>
      public partial class DocsAssistenteView : UserControl
      {
            public DocsAssistenteView()
            {
                  InitializeComponent();

            }

            private DocsAssistenteViewModel? model
            {
                  get
                  {
                        return DataContext as DocsAssistenteViewModel;
                  }
            }
            private void Grid_DragEnter(object sender, DragEventArgs e)
            {
                  // Verifica se os dados arrastados são arquivos.
                  if (e.Data.GetDataPresent(DataFormats.FileDrop))
                  {
                        // Muda o efeito do cursor para "Copy" (permitido) se forem arquivos.
                        e.Effects = DragDropEffects.Copy;
                  }
                  else
                  {
                        // Caso contrário, mantém o cursor com o efeito "None" (não permitido).
                        e.Effects = DragDropEffects.None;
                  }
            }

            private void Grid_DragOver(object sender, DragEventArgs e)
            {
                  // Verifica novamente se são arquivos sendo arrastados.
                  if (e.Data.GetDataPresent(DataFormats.FileDrop))
                  {
                        // Permite a operação de cópia enquanto o arquivo está sendo arrastado.
                        e.Effects = DragDropEffects.Copy;
                  }
                  else
                  {
                        e.Effects = DragDropEffects.None;
                  }

                  // Previne o comportamento padrão que poderia bloquear o arraste.
                  e.Handled = true;
            }

            private void Grid_Drop(object sender, DragEventArgs e)
            {
                  // Verifica se os dados soltos são arquivos.
                  if (model != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                  {
                        // Obtém os arquivos soltos.
                        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                        // Filtra apenas os arquivos .txt.
                        var txtFiles = files.Where(file => System.IO.Path.GetExtension(file).ToLower() == ".txt");

                        foreach (var file in txtFiles)
                        {
                              model.AddFile(file);
                        }
                  }
            }

            private void BTAddFiles_Click(object sender, RoutedEventArgs e)
            {
                  if (model == null) return;

                  OpenFileDialog openFileDialog = new OpenFileDialog
                  {
                        Multiselect = true,
                        // Filtra apenas arquivos .txt
                        Filter = "Arquivos de Texto (*.txt)|*.txt"
                  };

                  if (openFileDialog.ShowDialog() == true)
                  {
                        string[] selectedFiles = openFileDialog.FileNames;

                        foreach (string file in selectedFiles)
                        {
                              model.AddFile(file);
                        }
                  }
            }
      }
}
