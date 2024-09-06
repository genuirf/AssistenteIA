using Gf.AssistenteIA.Services;
using Gf.AssistenteIA.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Gf.AssistenteIA.ViewModels
{
      public class ChatViewModel : ViewModelBase
      {

            // Construtor sem parâmetros para o modo design
            public ChatViewModel()
            {
                  Mensagens = [];
            }
            public ChatViewModel(INavigationService navigationService, AssistenteModel assistente)
            {
                  Mensagens = [];
                  _navigationService = navigationService;
                  Assistente = assistente;
                  NavigateToHomeCommand = new RelayCommand(NavigateToAssistentes);
                  Titulo = assistente.Titulo;
            }

            public AssistenteModel Assistente
            {
                  get => Get<AssistenteModel>();
                  set => Set(value);
            }

            public ObservableCollection<MensagemModel> Mensagens
            {
                  get => Get<ObservableCollection<MensagemModel>>();
                  set => Set(value);
            }

            private readonly INavigationService _navigationService;

            public ICommand NavigateToHomeCommand { get; }

            private void NavigateToAssistentes(object arg)
            {
                  _navigationService.NavigateTo(new AssistentesViewModel(_navigationService));
            }

      }
}
