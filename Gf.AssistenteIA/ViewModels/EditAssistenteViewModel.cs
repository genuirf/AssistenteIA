using Gf.AssistenteIA.Repositories;
using Gf.AssistenteIA.Services;
using Gf.AssistenteIA.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Gf.AssistenteIA.ViewModels
{
      public class EditAssistenteViewModel : ViewModelBase
      {

            private readonly IServiceProvider _serviceProvider;
            private readonly INavigationService _navigationService;
            private readonly IDialogService _dialogService;

            // Construtor sem parâmetros para o modo design
            public EditAssistenteViewModel()
            {
                  Assistente = new() { Titulo = "Assistente 1", Descricao = "Descrição do assistente"};
            }
            public EditAssistenteViewModel(IServiceProvider serviceProvider, INavigationService navigationService, IDialogService dialogService, ViewModelBase backViewModel, AssistenteModel assistente)
            {
                  _serviceProvider = serviceProvider;
                  _navigationService = navigationService;
                  _dialogService = dialogService;
                  BackViewModel = backViewModel;
                  Assistente = assistente;
                  CancelCommand = new RelayCommand(Cancel);
                  SaveCommand = new RelayCommand(Save);

                  Titulo = "Assistente";
            }

            public ViewModelBase BackViewModel
            {
                  get => Get<ViewModelBase>();
                  set => Set(value);
            }
            public AssistenteModel Assistente
            {
                  get => Get<AssistenteModel>();
                  set => Set(value);
            }

            public ICommand CancelCommand { get; }
            public ICommand SaveCommand { get; }

            private void Cancel(object arg)
            {
                  _navigationService.NavigateTo(BackViewModel);
            }
            private void Save(object arg)
            {
                  try
                  {
                        var repository = new FileAssistantRepository();
                        repository.AddUpdate(Assistente);

                        _navigationService.NavigateTo(BackViewModel);
                  }
                  catch (Exception ex)
                  {
                        // TODO adicionar ao Logger
                  }
            }
      }
}
