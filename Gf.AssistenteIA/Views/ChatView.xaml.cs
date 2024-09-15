using Gf.AssistenteIA.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Gf.AssistenteIA.Views
{
      /// <summary>
      /// Interação lógica para ChatView.xam
      /// </summary>
      public partial class ChatView : UserControl
      {
            public ChatView()
            {
                  InitializeComponent();

                  DataContextChanged += ChatView_DataContextChanged;
                  Loaded += ChatView_Loaded;
            }

            private void ChatView_Loaded(object sender, RoutedEventArgs e)
            {
                  Keyboard.Focus(TbQuestao);
                  TbQuestao.Focus();
            }

            private void ChatView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                  model = (ChatViewModel)DataContext;
                  if (model == null) return;

                  model.onResponseFinalized = () =>
                  {
                        Keyboard.Focus(TbQuestao);
                        TbQuestao.Focus();
                  };
                  model.onGereratingResponse = () =>
                  {
                        scrollViewerMensagens.ScrollToBottom();
                  };
            }

            private ChatViewModel model;

            private void TextBox_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
            {
                  scrollViewerMensagens.ScrollToVerticalOffset(scrollViewerMensagens.VerticalOffset - e.Delta);
                  e.Handled = true;
            }

            private void TbQuestao_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
            {
                  // Verifica se a tecla Shift está pressionada
                  if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                  {
                        // Verifica se a tecla Enter foi pressionada
                        if (e.Key == Key.Enter)
                        {
                              // Obtém o TextBox
                              var textBox = sender as TextBox;

                              if (textBox != null)
                              {
                                    // Adiciona uma quebra de linha no texto do TextBox
                                    int caretIndex = textBox.CaretIndex;
                                    textBox.Text = textBox.Text.Insert(caretIndex, Environment.NewLine);

                                    // Move o caret para a nova linha
                                    textBox.CaretIndex = caretIndex + Environment.NewLine.Length;

                                    // Marca o evento como tratado para evitar o comportamento padrão do Enter
                                    e.Handled = true;
                              }
                        }
                  }
                  else if (e.Key == Key.Enter)
                  {
                        model.SendMsgCommand.Execute(null);
                        e.Handled = true;
                  }
            }
      }
}
