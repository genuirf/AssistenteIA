using Gf.AssistenteIA.Repositories;
using Gf.AssistenteIA.Services;
using Gf.AssistenteIA.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Gf.AssistenteIA.ViewModels
{
      public class AssistentesViewModel : ViewModelBase
      {
            private readonly IServiceProvider _serviceProvider;
            private readonly INavigationService _navigationService;
            private readonly IDialogService _dialogService;

            // Construtor sem parâmetros para o modo design
            public AssistentesViewModel()
            {
                  // Defina valores padrão ou dados simulados para o modo design
                  Titulo = "Meus Assistentes";
                  Assistentes = [];

                  Assistentes.Add(new() { Titulo = "Teste 1", Descricao = "Descrição teste 1" });
                  Assistentes.Add(new() { Titulo = "Teste 2", Descricao = "Descrição teste 2" });

            }
            public AssistentesViewModel(IServiceProvider serviceProvider, INavigationService navigationService, IDialogService dialogService)
            {
                  Titulo = "Meus Assistentes";
                  Assistentes = [];
                  _serviceProvider = serviceProvider;
                  _navigationService = navigationService;
                  _dialogService = dialogService;
                  OpenChatCommand = new RelayCommand(OpenChat, CanOpenChat);
                  EditAssistenteCommand = new RelayCommand(EditAssistente, CanEditAssistente);
                  AddDocumentsCommand = new RelayCommand(AddDocuments, CanAddDocuments);
                  DeleteAssistenteCommand = new RelayCommand(DeleteAssistente, CanDeleteAssistente);
                  AddAssitenteCommand = new RelayCommand(AddAssitente);

                  Load();
            }

            private void Load()
            {
                  var repository = new Repositories.FileAssistantRepository();

                  foreach (var item in repository.GetAll())
                  {
                        Assistentes.Add(item);
                  }
            }

            public ObservableCollection<AssistenteModel> Assistentes
            {
                  get => Get<ObservableCollection<AssistenteModel>>();
                  set => Set(value);
            }

            public ICommand OpenChatCommand { get; }
            public ICommand EditAssistenteCommand { get; }
            public ICommand AddDocumentsCommand { get; }
            public ICommand DeleteAssistenteCommand { get; }
            public ICommand AddAssitenteCommand { get; }

            private void OpenChat(object parameter)
            {
                  if (parameter is AssistenteModel assistente)
                  {
                        // Navegar para a tela de chat usando o assistente selecionado
                        _navigationService.NavigateTo(new ChatViewModel(App.ServiceProvider, _navigationService, _dialogService, assistente));
                  }
            }
            private bool CanOpenChat(object parameter)
            {
                  return parameter is AssistenteModel;
            }

            private void EditAssistente(object parameter)
            {
                  if (parameter is AssistenteModel assistente)
                  {
                        _navigationService.NavigateTo(new EditAssistenteViewModel(_serviceProvider, _navigationService, _dialogService, this, assistente));
                  }
            }
            private bool CanEditAssistente(object parameter)
            {
                  return parameter is AssistenteModel;
            }

            private void AddDocuments(object parameter)
            {
                  if (parameter is AssistenteModel assistente)
                  {
                        _navigationService.NavigateTo(new DocsAssistenteViewModel(_serviceProvider, _navigationService, _dialogService, this, assistente));
                  }
            }
            private bool CanAddDocuments(object parameter)
            {
                  return parameter is AssistenteModel;
            }

            private void DeleteAssistente(object parameter)
            {
                  if (parameter is AssistenteModel assistente)
                  {
                        bool confirmDelete = _dialogService.Confirm($"Tem certeza de que deseja excluir o assistente '{assistente.Titulo}'?", "Confirmar Exclusão");

                        if (confirmDelete)
                        {
                              try
                              {
                                    var repository = new FileAssistantRepository();
                                    repository.Delete(assistente.Id);

                                    Assistentes.Remove(assistente);
                              }
                              catch (Exception ex)
                              {
                                    // TODO adicionar ao Logger
                              }
                        }
                  }
            }
            private bool CanDeleteAssistente(object parameter)
            {
                  return parameter is AssistenteModel;
            }

            private void AddAssitente(object parameter)
            {
                  _navigationService.NavigateTo(new EditAssistenteViewModel(_serviceProvider, _navigationService, _dialogService, this, new() { Id = Guid.NewGuid()}));
            }
      }
}
