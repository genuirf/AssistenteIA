using Gf.AssistenteIA.Repositories;
using Gf.AssistenteIA.Services;
using Gf.AssistenteIA.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Gf.AssistenteIA.ViewModels
{
      public class EditAssistenteViewModel : ViewModelBase
      {

            // Construtor sem parâmetros para o modo design
            public EditAssistenteViewModel()
            {
                  Assistente = new() { Titulo = "Assistente 1", Descricao = "Descrição do assistente"};
            }
            public EditAssistenteViewModel(INavigationService navigationService, AssistenteModel assistente)
            {
                  _navigationService = navigationService;
                  Assistente = assistente;
                  CancelCommand = new RelayCommand(Cancel);
                  SaveCommand = new RelayCommand(Save);

                  Titulo = "Assistente";
            }

            public AssistenteModel Assistente
            {
                  get => Get<AssistenteModel>();
                  set => Set(value);
            }

            private readonly INavigationService _navigationService;

            public ICommand CancelCommand { get; }
            public ICommand SaveCommand { get; }

            private void Cancel(object arg)
            {
                  _navigationService.NavigateTo(new AssistentesViewModel(_navigationService));
            }
            private void Save(object arg)
            {
                  try
                  {
                        var repository = new FileAssistantRepository();
                        repository.AddUpdate(Assistente);

                        _navigationService.NavigateTo(new AssistentesViewModel(_navigationService));
                  }
                  catch (Exception ex)
                  {
                        // TODO adicionar ao Logger
                  }
            }
      }
}
