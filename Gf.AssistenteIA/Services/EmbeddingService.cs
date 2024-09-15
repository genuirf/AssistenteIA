using Gf.AssistenteIA.Models;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Controls;

namespace ChatIA.Model.Recursos
{
      public class EmbeddingService
      {
            private readonly string fileEmbedding = "embeddings.json";
            private static string GetDirectoryEmbedding(Guid assistente_id)
            {
                  return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "embeddings", assistente_id.ToString());
            }
            public void SaveFile(Guid assistente_id, string file)
            {
                  FileInfo fileInfo = new(file);
                  FileInfo filePath = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "embeddings", assistente_id.ToString(), fileInfo.Name));
                  if (!filePath.Directory.Exists)
                  {
                        filePath.Directory.Create();
                  }
                  fileInfo.CopyTo(filePath.FullName, true);
            }
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
            public async Task<List<FileEmbeddingData>> LoadAllFilesEmbeddingsAsync(Guid assistente_id)
            {
                  FileInfo filePath = new FileInfo(Path.Combine(GetDirectoryEmbedding(assistente_id), fileEmbedding));
                  if (!filePath.Exists)
                  {
                        return []; // Arquivo não encontrado, retorna um dicionário vazio
                  }

                  var result = new List<FileEmbeddingData>();

                  // listar todos arquivos na pasta
                  var files = filePath.Directory.GetFiles("*.txt");
                  foreach (var file in files)
                  {
                        result.Add(new() { FileContent = File.ReadAllText(file.FullName, UTF8Encoding.Default), fileName = file.Name });
                  }

                  var json = await File.ReadAllTextAsync(filePath.FullName);

                  // Desserializar o arquivo JSON para um dicionário
                  var embeddings = JsonSerializer.Deserialize<Dictionary<string, EmbeddingData>>(json) ?? [];

                  foreach (var kvp in embeddings)
                  {
                        string file = Path.Combine(GetDirectoryEmbedding(assistente_id), kvp.Key);
                        if (!File.Exists(file)) { continue; }

                        var item = result.FirstOrDefault(f=> f.fileName == kvp.Key);
                        if (item == null)
                        {
                              item = new() { Embedding = kvp.Value.Embedding, fileName = kvp.Key };
                              result.Add(item);
                        }

                        item.Embedding = kvp.Value.Embedding;
                        item.check = kvp.Value.Embedding != null && kvp.Value.Embedding.Length > 0;
                  }

                  return result;
            }
      }
}
