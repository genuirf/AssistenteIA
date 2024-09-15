using ChatIA.Model.Recursos;
using Gf.AssistenteIA.Icones;
using Gf.AssistenteIA.Models;
using Gf.AssistenteIA.Repositories;
using Gf.AssistenteIA.Services;
using Gf.AssistenteIA.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Windows.Input;

namespace Gf.AssistenteIA.ViewModels
{
      public class DocsAssistenteViewModel : ViewModelBase
      {

            private readonly IServiceProvider _serviceProvider;
            private readonly INavigationService _navigationService;
            private readonly IDialogService _dialogService;
            private IAssistenteService _apiService;

            // Construtor sem parâmetros para o modo design
            public DocsAssistenteViewModel()
            {
                  FileEmbeddings = [new() { fileName = "documento-teste.txt" }, new() { fileName = "teste-2.txt" }];
            }
            public DocsAssistenteViewModel(IServiceProvider serviceProvider, INavigationService navigationService, IDialogService dialogService, ViewModelBase backViewModel, AssistenteModel assistente)
            {
                  _serviceProvider = serviceProvider;
                  _navigationService = navigationService;
                  _dialogService = dialogService;
                  BackViewModel = backViewModel;
                  Assistente = assistente;
                  DeleteFileCommand = new RelayCommand(DeleteFile);
                  CancelCommand = new RelayCommand(Cancel);
                  SaveCommand = new RelayCommand(Save);

                  FileEmbeddings = [];

                  Titulo = "Documentos";

                  InitializeAPI();
                  LoadEmbeddings();
            }

            private void InitializeAPI()
            {
                  _apiService = _serviceProvider.GetRequiredService<IAssistenteService>();
                  _apiService.ConfigurarUrlApi(Assistente.Api_Url);
                  _apiService.ConfigurarToken(Assistente.Api_Token);
            }

            private async void LoadEmbeddings()
            {
                  var repository = new EmbeddingService();
                  var embeddings = await repository.LoadAllFilesEmbeddingsAsync(Assistente.Id);

                  foreach (var embedding in embeddings)
                  {
                        FileEmbeddings.Add(embedding);
                  }
            }

            public ObservableCollection<FileEmbeddingData> FileEmbeddings
            {
                  get => Get<ObservableCollection<FileEmbeddingData>>();
                  set => Set(value);
            }

            public AssistenteModel Assistente
            {
                  get => Get<AssistenteModel>();
                  set => Set(value);
            }
            public ViewModelBase BackViewModel
            {
                  get => Get<ViewModelBase>();
                  set => Set(value);
            }
    
            public bool ShowProgress
            {
                  get => Get<bool>();
                  set => Set(value);
            }
            public int ProgressMaximum
            {
                  get => Get<int>();
                  set => Set(value);
            }
            public int ProgressValue
            {
                  get => Get<int>();
                  set => Set(value);
            }

            public ICommand DeleteFileCommand { get; }
            public ICommand CancelCommand { get; }
            public ICommand SaveCommand { get; }

            public void AddFile(string file)
            {
                  var fileInfo = new FileInfo(file);
                  if (fileInfo.Exists)
                  {
                        var repository = new EmbeddingService();
                        repository.SaveFile(Assistente.Id, fileInfo.FullName);

                        var FileEmbedding = FileEmbeddings.FirstOrDefault(f => f.fileName == fileInfo.Name);
                        if (FileEmbedding == null)
                        {
                              FileEmbedding = new FileEmbeddingData();
                              FileEmbeddings.Add(FileEmbedding);
                        }
                        FileEmbedding.fileName = fileInfo.Name;
                        FileEmbedding.FileContent = File.ReadAllText(fileInfo.FullName);
                        FileEmbedding.Embedding = [];

                  }
            }
            private void DeleteFile(object arg)
            {
                  if (arg is FileEmbeddingData file)
                  {
                        bool confirmDelete = _dialogService.Confirm($"Tem certeza de que deseja excluir o arquivo '{file.fileName}'?", "Confirmar Exclusão");

                        if (confirmDelete)
                        {
                              FileEmbeddings.Remove(file);
                        }
                  }
            }
            private void Cancel(object arg)
            {
                  _navigationService.NavigateTo(BackViewModel);
            }
            private async void Save(object arg)
            {
                  try
                  {
                        ProgressValue = 0;
                        ProgressMaximum = FileEmbeddings.Count;
                        ShowProgress = true;

                        var repository = new EmbeddingService();

                        foreach (var file in FileEmbeddings)
                        {
                              ProgressValue++;

                              if (!file.check) file.Embedding = [];

                              if (file.check && (file.Embedding == null || file.Embedding.Length < 1))
                              {
                                    file.Embedding = await _apiService.GetEmbeddingAsync(file.FileContent, TimeSpan.FromMinutes(2));

                              }
                              await repository.SaveEmbeddingAsync(Assistente.Id, file.fileName, file.Embedding);
                        }

                        _navigationService.NavigateTo(BackViewModel);
                  }
                  catch (HttpRequestException e)
                  {
                        // TODO adicionar ao Logger
                        _dialogService.Error("A API não está acessível no momento. Verifique sua conexão de internet ou tente novamente mais tarde.", e);
                  }
                  catch (TaskCanceledException e)
                  {
                        // TODO adicionar ao Logger
                        _dialogService.Error("A conexão com a API expirou. Tente novamente mais tarde.", e);
                  }
                  catch (Exception ex)
                  {
                        // TODO adicionar ao Logger
                        _dialogService.Error("Ocorreu um erro inesperado ao tentar acessar a API.", ex);
                  }
                  finally
                  {
                        ShowProgress = false;
                  }
            }
      }
}
