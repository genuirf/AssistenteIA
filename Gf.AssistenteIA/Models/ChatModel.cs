using System.Collections.ObjectModel;

namespace Gf.AssistenteIA.ViewModels
{
      public class ChatModel : ModelBase
      {
            public ChatModel()
            {
                  Mensagens = [];
            }
            public string Titulo
            {
                  get => Get<string>();
                  set => Set(value);
            }

            public ObservableCollection<MensagemModel> Mensagens
            {
                  get => Get<ObservableCollection<MensagemModel>>();
                  set => Set(value);
            }

            public string Questao
            {
                  get => Get<string>();
                  set => Set(value);
            }
      }
}
