using ChatIA.Model.Recursos;
using Gf.AssistenteIA.Icones;
using Gf.AssistenteIA.Models;
using Gf.AssistenteIA.Services;
using Gf.AssistenteIA.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace Gf.AssistenteIA.ViewModels
{
      public class ChatViewModel : ViewModelBase
      {

            private readonly IServiceProvider _serviceProvider;
            private readonly INavigationService _navigationService;
            private readonly IDialogService _dialogService;
            private IAssistenteService _apiService;

            // Construtor sem parâmetros para o modo design
            public ChatViewModel()
            {
                  Chats = [new() { Titulo = "Teste Chat 1" }];
                  CurrentChat = new();
                  CurrentChat.Mensagens.Add(new() { MensagemUsuario = "Teste", MensagemAssistente = "Teste assistente" });
            }
            public ChatViewModel(IServiceProvider serviceProvider, INavigationService navigationService, IDialogService dialogService, AssistenteModel assistente)
            {
                  Chats = [];
                  _serviceProvider = serviceProvider;
                  _navigationService = navigationService;
                  _dialogService = dialogService;
                  Assistente = assistente;
                  CurrentChat = new();
                  AddDocumentsCommand = new RelayCommand(AddDocuments, CanAddDocuments);
                  NavigateToHomeCommand = new RelayCommand(NavigateToAssistentes);
                  AddChatCommand = new RelayCommand(AddChat, CanAddChat);
                  EditChatCommand = new RelayCommand(EditChat, CanEditChat);
                  DeleteChatCommand = new RelayCommand(DeleteChat, CanDeleteChat);
                  ActivateChatCommand = new RelayCommand(ActivateChat, CanActivateChat);
                  SendMsgCommand = new RelayCommand(SendMsg, CanSendMsg);
                  StopResponseCommand = new RelayCommand(StopResponse, CanStopResponse);
                  Titulo = assistente.Titulo;

                  InitializeAPI();
                  LoadEmbeddings();
            }

            public AssistenteModel Assistente
            {
                  get => Get<AssistenteModel>();
                  set => Set(value);
            }
            public ChatModel CurrentChat
            {
                  get => Get<ChatModel>();
                  set => Set(value);
            }
            public bool GeneratingResponse
            {
                  get => Get<bool>();
                  set => Set(value);
            }

            public ObservableCollection<ChatModel> Chats
            {
                  get => Get<ObservableCollection<ChatModel>>();
                  set => Set(value);
            }

            public ICommand AddDocumentsCommand { get; }
            public ICommand NavigateToHomeCommand { get; }
            public ICommand AddChatCommand { get; }
            public ICommand EditChatCommand { get; }
            public ICommand DeleteChatCommand { get; }
            public ICommand ActivateChatCommand { get; }
            public ICommand SendMsgCommand { get; }
            public ICommand StopResponseCommand { get; }

            private void InitializeAPI()
            {
                  _apiService = _serviceProvider.GetRequiredService<IAssistenteService>();
                  _apiService.ConfigurarUrlApi(Assistente.Api_Url);
                  _apiService.ConfigurarToken(Assistente.Api_Token);
            }

            private List<MensagemModel> GetHistorico()
            {
                  var lista = new List<MensagemModel>();
                  lista.AddRange(CurrentChat.Mensagens);
                  return lista;
            }

            private void AddDocuments(object arg)
            {

            }
            private bool CanAddDocuments(object arg)
            {
                  return !GeneratingResponse;
            }
            private void NavigateToAssistentes(object arg)
            {
                  StopResponse(null);
                  _navigationService.NavigateTo(new AssistentesViewModel(_navigationService, _dialogService));
            }
            private void AddChat(object arg)
            {
                  CurrentChat = new();
            }
            private bool CanAddChat(object arg)
            {
                  return !GeneratingResponse;
            }
            private void EditChat(object arg)
            {

            }
            private bool CanEditChat(object arg)
            {
                  return !GeneratingResponse;
            }
            private void DeleteChat(object arg)
            {

            }
            private bool CanDeleteChat(object arg)
            {
                  return !GeneratingResponse;
            }
            private void ActivateChat(object arg)
            {

            }
            private bool CanActivateChat(object arg)
            {
                  return !GeneratingResponse;
            }

            public async Task LoadEmbeddings()
            {
                  var embeddingService = App.ServiceProvider.GetRequiredService<EmbeddingService>();
                  embeddings = await embeddingService.LoadAllEmbeddingsAsync(Assistente.Id);
            }
            public Dictionary<string, float[]> embeddings = new();

            private async Task GerarTitulo()
            {

            }

            private async Task<ContextoMensagem> GetContexto(string Questao)
            {
                  var contexto = new ContextoMensagem();

                  StringBuilder stringBuilder = new();

                  if (Assistente.Instrucoes?.Length > 0) stringBuilder.AppendLine(Assistente.Instrucoes);

                  List<CosineSimilarityInfo> contextosSimilares = new();
 
                  float[] queryEmbedding = [];

                  if (embeddings.Count > 0)
                  {
                        queryEmbedding = await _apiService.GetEmbeddingAsync(Questao, TimeSpan.FromSeconds(10));
                        foreach (var (doc, embedding) in embeddings)
                        {
                              double similarity = Funcoes.ComputeCosineSimilarity(queryEmbedding, embedding);
                              if (similarity > 0.6)
                              {
                                    contextosSimilares.Add(new(similarity, doc));
                              }
                        }
                  }

                  if (contextosSimilares.Count < 1)
                  {
                        if (Assistente.InstrucoesSemContexto?.Length > 0)
                        {
                              stringBuilder.AppendLine(Assistente.InstrucoesSemContexto);
                        }
                  }
                  else
                  {
                        int count = 1;
                        foreach (var doc in contextosSimilares.OrderByDescending(r => r.similarity).Take(2).Select(r => r.doc))
                        {
                              stringBuilder.AppendLine($"[CONTEXT {count}]\n{doc}\n[END CONTEXT {count}]\n\n");
                              count++;
                        }

                  }

                  contexto.contexto = stringBuilder.ToString();

                  return contexto;
            }

            private async void SendMsg(object arg)
            {
                  try
                  {
                        GeneratingResponse = true;

                        var historico = GetHistorico();

                        var mensagem = new MensagemModel();
                        mensagem.MensagemUsuario = CurrentChat.Questao;
                        CurrentChat.Mensagens.Add(mensagem);

                        CurrentChat.Questao = "";

                        var contexto = await GetContexto(mensagem.MensagemUsuario);

                        string resposta = await _apiService.SendQuestionAsync(Assistente, mensagem.MensagemUsuario, historico, contexto.contexto, (resp) =>
                        {
                              mensagem.MensagemAssistente = $"{mensagem.MensagemAssistente}{resp}";
                        }, async () =>
                        {
                              GeneratingResponse = false;
                              if (CurrentChat.Titulo?.Length < 1 && (contexto.contextosSimilares.Count > 0 || historico?.Count > 0))
                              {
                                    await GerarTitulo();
                              }
                        });

                  }
                  catch (Exception ex)
                  {
                        _dialogService.Error("Erro ao responder a questão!", ex);
                  }
                  finally
                  {
                  }
            }
            private bool CanSendMsg(object arg)
            {
                  return !GeneratingResponse;
            }
            private void StopResponse(object arg)
            {

            }
            private bool CanStopResponse(object arg)
            {
                  return GeneratingResponse;
            }
            private void Save()
            {

            }

      }
}
