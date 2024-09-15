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

            public Action? onResponseFinalized {  get; set; }
            public Action? onGereratingResponse {  get; set; }

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
                  EditAssistenteCommand = new RelayCommand(EditAssistente, CanEditAssistente);
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
            public ICommand EditAssistenteCommand { get; }
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
                  _navigationService.NavigateTo(new DocsAssistenteViewModel(_serviceProvider, _navigationService, _dialogService, this, Assistente));
            }
            private bool CanAddDocuments(object arg)
            {
                  return !GeneratingResponse;
            }

            private void EditAssistente(object parameter)
            {
                  _navigationService.NavigateTo(new EditAssistenteViewModel(_serviceProvider, _navigationService, _dialogService, this, Assistente));
            }
            private bool CanEditAssistente(object arg)
            {
                  return !GeneratingResponse;
            }

            private void NavigateToAssistentes(object arg)
            {
                  StopResponse(null);
                  _navigationService.NavigateTo(new AssistentesViewModel(_serviceProvider, _navigationService, _dialogService));
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
                  embeddings = (await embeddingService.LoadAllFilesEmbeddingsAsync(Assistente.Id)).Where(e=> e.Embedding != null && e.Embedding.Length > 0).ToList();
            }
            public List<FileEmbeddingData> embeddings = new();

            private async Task GerarTitulo()
            {
                  try
                  {
                        Titulo = "";
                        var historico = GetHistorico();
                        var contexto = new StringBuilder();
                        contexto.AppendLine($"[CONTEXT]");
                        foreach (var item in historico)
                        {
                              contexto.AppendLine($"[USER] {item.MensagemUsuario}[END USER]");
                              contexto.AppendLine($"[ASSISTANT] {item.MensagemAssistente}[END ASSISTANT]");
                        }
                        contexto.AppendLine($"[END CONTEXT]");

                        string resposta = await _apiService.SendQuestionAsync(Assistente, "No máximo 5 palavras, qual \"assunto\" sugere com base no contexto?", historico, contexto.ToString(), (resp) =>
                        {
                              Titulo = $"{Titulo}{resp.Replace("\n", "")}";
                        }, null);
                  }
                  catch (Exception ex)
                  {
                  }
            }

            private async Task<ContextoMensagem> GetContexto(string Questao)
            {
                  var contexto = new ContextoMensagem();

                  StringBuilder stringBuilder = new();

                  List<CosineSimilarityInfo> contextosSimilares = new();
 
                  float[] queryEmbedding = [];

                  if (embeddings.Count > 0)
                  {
                        queryEmbedding = await _apiService.GetEmbeddingAsync(Questao, TimeSpan.FromSeconds(10));
                        foreach (var embedding in embeddings)
                        {
                              double similarity = Funcoes.ComputeCosineSimilarity(queryEmbedding, embedding.Embedding);
                              if (similarity > Assistente.ContextSimilarity)
                              {
                                    contextosSimilares.Add(new(similarity, embedding.FileContent));
                              }
                        }
                  }

                  if (contextosSimilares.Count < 1 && Assistente.InstrucoesSemContexto?.Length > 0)
                  {
                        stringBuilder.AppendLine(Assistente.InstrucoesSemContexto);
                  }
                  else if (contextosSimilares.Count > 0)
                  {

                        if (Assistente.Instrucoes?.Length > 0) stringBuilder.AppendLine(Assistente.Instrucoes);

                        int count = 1;
                        foreach (var doc in contextosSimilares.OrderByDescending(r => r.similarity).Take(2).Select(r => r.doc))
                        {
                              stringBuilder.AppendLine($"[CONTEXT {count}]\n{doc}\n[END CONTEXT {count}]\n\n");
                              count++;
                        }
                  }
                  else
                  {
                        if (Assistente.Instrucoes?.Length > 0) stringBuilder.AppendLine(Assistente.Instrucoes);
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
                              onGereratingResponse?.Invoke();
                        }, async () =>
                        {
                              GeneratingResponse = false;
                              if (CurrentChat.Titulo?.Length < 1 && (contexto.contextosSimilares.Count > 0 || historico?.Count > 0))
                              {
                                    await GerarTitulo();
                              }

                              onResponseFinalized?.Invoke();
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
                  _apiService.StopResponse();
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
