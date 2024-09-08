using Gf.AssistenteIA.Models;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ChatIA.Model.Recursos
{
      public class EmbeddingService
      {
            private readonly string fileEmbedding = "embeddings.json";
            public async Task SaveEmbeddingAsync(Guid assistente_id, string fileName, float[] embedding)
            {
                  FileInfo filePath = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "embeddings", assistente_id.ToString(), fileEmbedding));
                  if (!filePath.Directory.Exists)
                  {
                        filePath.Directory.Create();
                  }
           
                  // Lê o conteúdo atual do arquivo, se existir
                  Dictionary<string, EmbeddingData> embeddings;
                  if (filePath.Exists)
                  {
                        var json = await File.ReadAllTextAsync(filePath.FullName);
                        embeddings = JsonSerializer.Deserialize<Dictionary<string, EmbeddingData>>(json);
                  }
                  else
                  {
                        embeddings = new Dictionary<string, EmbeddingData>();
                  }

                  // Adiciona o novo embedding
                  embeddings[fileName] = new EmbeddingData { Embedding = embedding };

                  // Salva de volta no arquivo
                  var jsonString = JsonSerializer.Serialize(embeddings, new JsonSerializerOptions { WriteIndented = true });
                  await File.WriteAllTextAsync(filePath.FullName, jsonString);
            }
            public async Task<float[]> LoadEmbeddingAsync(Guid assistente_id, string fileName)
            {
                  FileInfo filePath = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "embeddings", assistente_id.ToString(), fileEmbedding));
                  if (!filePath.Exists)
                  {
                        return null; // Arquivo não encontrado
                  }

                  var json = await File.ReadAllTextAsync(filePath.FullName);
                  var embeddings = JsonSerializer.Deserialize<Dictionary<string, EmbeddingData>>(json);

                  if (embeddings != null && embeddings.ContainsKey(fileName))
                  {
                        return embeddings[fileName].Embedding;
                  }

                  return null; // Nome do arquivo não encontrado
            }
            public async Task<Dictionary<string, float[]>> LoadAllEmbeddingsAsync(Guid assistente_id)
            {
                  FileInfo filePath = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "embeddings", assistente_id.ToString(), fileEmbedding));
                  if (!filePath.Exists)
                  {
                        return new Dictionary<string, float[]>(); // Arquivo não encontrado, retorna um dicionário vazio
                  }

                  var json = await File.ReadAllTextAsync(filePath.FullName);

                  // Desserializar o arquivo JSON para um dicionário
                  var embeddings = JsonSerializer.Deserialize<Dictionary<string, EmbeddingData>>(json);

                  // Converter de EmbeddingData para float[]
                  var result = new Dictionary<string, float[]>();
                  foreach (var kvp in embeddings)
                  {
                        if (!File.Exists(kvp.Key)) { continue; }
                        var doc = File.ReadAllText(kvp.Key, UTF8Encoding.Default);
                        result[doc] = kvp.Value.Embedding;
                  }

                  return result;
            }
      }
}
