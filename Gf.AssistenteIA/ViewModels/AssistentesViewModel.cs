﻿using Gf.AssistenteIA.Services;
using Gf.AssistenteIA.Utils;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Gf.AssistenteIA.ViewModels
{
      public class AssistentesViewModel : ViewModelBase
      {
            private readonly INavigationService _navigationService;
           
            // Construtor sem parâmetros para o modo design
            public AssistentesViewModel()
            {
                  // Defina valores padrão ou dados simulados para o modo design
                  Titulo = "Meus Assistentes";
                  Assistentes = [];

                  Assistentes.Add(new() { Titulo = "Teste 1", Descricao = "Descrição teste 1" });
                  Assistentes.Add(new() { Titulo = "Teste 2", Descricao = "Descrição teste 2" });

            }
            public AssistentesViewModel(INavigationService navigationService)
            {
                  Titulo = "Meus Assistentes";
                  Assistentes = [];
                  _navigationService = navigationService;
                  OpenChatCommand = new RelayCommand(OpenChat, CanOpenChat);
                  EditAssistenteCommand = new RelayCommand(EditAssistente, CanEditAssistente);
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
            public ICommand DeleteAssistenteCommand { get; }
            public ICommand AddAssitenteCommand { get; }

            private void OpenChat(object parameter)
            {
                  if (parameter is AssistenteModel assistente)
                  {
                        // Navegar para a tela de chat usando o assistente selecionado
                        _navigationService.NavigateTo(new ChatViewModel(_navigationService, assistente));
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
                        _navigationService.NavigateTo(new EditAssistenteViewModel(_navigationService, assistente));
                  }
            }
            private bool CanEditAssistente(object parameter)
            {
                  return parameter is AssistenteModel;
            }

            private void DeleteAssistente(object parameter)
            {
                  if (parameter is AssistenteModel assistente)
                  {

                  }
            }
            private bool CanDeleteAssistente(object parameter)
            {
                  return parameter is AssistenteModel;
            }

            private void AddAssitente(object parameter)
            {
                  _navigationService.NavigateTo(new EditAssistenteViewModel(_navigationService, new() { Id = Guid.NewGuid()}));
            }
      }
}
