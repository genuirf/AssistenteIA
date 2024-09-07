using Gf.AssistenteIA.Services;
using Gf.AssistenteIA.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Gf.AssistenteIA.ViewModels
{
      public class ChatViewModel : ViewModelBase
      {

            private readonly INavigationService _navigationService;
            private readonly IDialogService _dialogService;

            // Construtor sem parâmetros para o modo design
            public ChatViewModel()
            {
                  Mensagens = [];
            }
            public ChatViewModel(INavigationService navigationService, IDialogService dialogService, AssistenteModel assistente)
            {
                  Mensagens = [];
                  _navigationService = navigationService;
                  _dialogService = dialogService;
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

            public ICommand NavigateToHomeCommand { get; }

            private void NavigateToAssistentes(object arg)
            {
                  _navigationService.NavigateTo(new AssistentesViewModel(_navigationService, _dialogService));
            }

      }
}
