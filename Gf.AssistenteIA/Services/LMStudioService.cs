
using Gf.AssistenteIA.Models;
using Gf.AssistenteIA.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Gf.AssistenteIA.Services
{
      public class LMStudioService : IAssistenteService
      {
            private string _apiUrl;
            private string _token;

            private const string url_completions = "/v1/chat/completions";
            private const string url_embeddings = "/v1/embeddings";

            private bool stopRequested = false;

            public LMStudioService()
            {
            }
            public void ConfigurarUrlApi(string apiUrl)
            {
                  _apiUrl = apiUrl;
            }

            public void ConfigurarToken(string token)
            {
                  _token = token;
            }

            private Body GetBody(string questao, List<MensagemModel> historico, string? contextEmbedding, double temperature, int max_tokens)
            {
                  var body = new Body();
                  body.temperature = temperature;
                  body.max_tokens = max_tokens;
                  foreach (var msg in historico)
                  {
                        body.messages.Add(new() { role = "user", content = msg.MensagemUsuario });
                        body.messages.Add(new() { role = "assistant", content = msg.MensagemAssistente });
                  }
                  if (contextEmbedding?.Length > 0) body.messages.Add(new() { role = "system", content = contextEmbedding });
                  body.messages.Add(new() { role = "user", content = questao });

                  return body;
            }

            public void StopResponse()
            {
                  stopRequested = true;
            }

            public async Task<string> SendQuestionAsync(AssistenteModel assistente, string questao, List<MensagemModel> historico, string? contextEmbedding, Action<string> onStreamMsg, Action onStop)
            {
                  var body = GetBody(questao, historico, contextEmbedding, assistente.Temperature, assistente.MaxTokens);

                  HttpClient client = new();
                  if (_token == null)
                  {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                  }
                  client.Timeout = TimeSpan.FromMinutes(5); // Aumenta o timeout para garantir que longas respostas não sejam interrompidas

                  StringBuilder resposta = new();

                  // Serializa o corpo para JSON
                  string jsonString = JsonSerializer.Serialize(body, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

                  // Cria o conteúdo da requisição
                  var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                  try
                  {

                        // Envia a requisição POST com HttpCompletionOption.ResponseHeadersRead para processar streaming
                        using (var request = new HttpRequestMessage(HttpMethod.Post, Utils.URI.Combine(_apiUrl, url_completions)) { Content = content })
                        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                        {
                              response.EnsureSuccessStatusCode(); // Lança uma exceção se a resposta não for bem-sucedida

                              // Processa a resposta em streaming
                              using (var responseStream = await response.Content.ReadAsStreamAsync())
                              {
                                    using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                                    {
                                          string line;
                                          while ((line = await reader.ReadLineAsync()) != null)
                                          {
                                                if (stopRequested)
                                                {
                                                      stopRequested = false;
                                                      responseStream.Close();
                                                      break;
                                                }
                                                try
                                                {
                                                      string? msgStream = ParseTextResponse(line);
                                                      if (msgStream == null)
                                                      {
                                                            onStreamMsg("\n");
                                                            break;
                                                      }

                                                      onStreamMsg(msgStream);
                                                      resposta.Append(msgStream);
                                                }
                                                catch (JsonException ex)
                                                {
                                                      Console.WriteLine($"Erro ao processar o JSON: {ex.Message}");
                                                      // TODO adicionar ao Logger
                                                }
                                          }
                                    }
                              }
                        }

                        return resposta.ToString();
                  }
                  catch (TaskCanceledException ex)
                  {
                        Console.WriteLine("A requisição foi cancelada devido ao timeout.");
                  }
                  catch (Exception ex)
                  {
                        // Exibe qualquer erro que ocorra
                        Console.WriteLine($"Erro: {ex.Message}");
                  }
                  finally
                  {
                        onStop?.Invoke();
                  }

                  return string.Empty;
            }

            private string? ParseTextResponse(string line)
            {
                  if (line.StartsWith("data:"))
                  {
                        // Remove o prefixo "data:" e faz o trim
                        var json = line.Substring(5).Trim();

                        // Verifica se a string JSON não está vazia
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                              // Verifica se a linha é o final do stream
                              if (json.Equals("[DONE]", StringComparison.OrdinalIgnoreCase))
                              {
                                    return null;
                              }
                              using (JsonDocument jsonDoc = JsonDocument.Parse(json))
                              {
                                    // Tenta acessar o array de escolhas
                                    if (jsonDoc.RootElement.TryGetProperty("choices", out var choicesArray))
                                    {
                                          foreach (var choice in choicesArray.EnumerateArray())
                                          {
                                                if (choice.TryGetProperty("delta", out var delta))
                                                {
                                                      if (delta.TryGetProperty("content", out var contentProp))
                                                      {
                                                            string contentValue = contentProp.GetString() ?? "";
                                                            return contentValue;
                                                      }
                                                }
                                          }
                                    }
                              }
                        }
                  }
                  return string.Empty;
            }

            public async Task<float[]> GetEmbeddingAsync(string text, TimeSpan timeout)
            {
                  HttpClient client = new();
                  if (_token == null)
                  {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                  }
                  client.Timeout = timeout;

                  var requestBody = new { input = text };
                  string jsonRequest = JsonSerializer.Serialize(requestBody);
                  var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                  var response = await client.PostAsync(Utils.URI.Combine(_apiUrl, url_embeddings), content);
                  response.EnsureSuccessStatusCode();

                  var jsonResponse = await response.Content.ReadAsStringAsync();
                  var responseJson = JsonDocument.Parse(jsonResponse);

                  // Extrair o array de embeddings
                  if (responseJson.RootElement.TryGetProperty("data", out var dataArray) &&
                      dataArray[0].TryGetProperty("embedding", out var embeddingArray))
                  {
                        // Converter o array JSON para um array de float
                        var embeddings = embeddingArray.EnumerateArray();
                        var floatEmbeddings = new float[embeddings.Count()];
                        int i = 0;

                        foreach (var item in embeddings)
                        {
                              floatEmbeddings[i++] = item.GetSingle(); // Ou item.GetDouble() se o tipo for double
                        }

                        return floatEmbeddings;
                  }

                  throw new Exception("Embedding não encontrado na resposta.");
            }

      }
}
