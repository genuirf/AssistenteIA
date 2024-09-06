using Gf.AssistenteIA.ViewModels;
using Newtonsoft.Json;
using System.IO;

namespace Gf.AssistenteIA.Repositories
{
      public class FileAssistantRepository : IAssistantRepository
      {
            private readonly string _filePath;

            public FileAssistantRepository()
            {
                  _filePath = Path.Combine(Environment.CurrentDirectory, "data", "assistentes.json");
                  Assistentes = [];
                  Load();
            }

            private List<AssistenteModel> Assistentes;

            private void Load()
            {
                  var fileInfo = new FileInfo(_filePath);
                  if (!fileInfo.Directory.Exists)
                  {
                        fileInfo.Directory.Create();
                  }
                  if (fileInfo.Exists)
                  {
                        var json = File.ReadAllText(_filePath);
                        Assistentes =  JsonConvert.DeserializeObject<List<AssistenteModel>>(json) ?? [];
                  }
            }

            public List<AssistenteModel> GetAll()
            {
                  return Assistentes;
            }

            public void Save(List<AssistenteModel> assistentes)
            {
                  var json = JsonConvert.SerializeObject(assistentes, Newtonsoft.Json.Formatting.Indented);
                  File.WriteAllText(_filePath, json);
            }

            public AssistenteModel? GetById(Guid id)
            {
                  var assistentes = GetAll();
                  return assistentes.FirstOrDefault(a => a.Id == id);
            }

            public void AddUpdate(AssistenteModel assistente)
            {
                  var assistentes = GetAll();

                  var assistenteTemp = assistentes.FirstOrDefault(a=> a.Id == assistente.Id);
                  if (assistenteTemp != null)
                  {
                        assistentes.Remove(assistenteTemp);
                  }

                  assistentes.Insert(0, assistente);
                  Save(assistentes);
            }

            public void Delete(Guid id)
            {
                  var assistentes = GetAll();
                  var assistenteToDelete = assistentes.FirstOrDefault(a => a.Id == id);
                  if (assistenteToDelete != null)
                  {
                        assistentes.Remove(assistenteToDelete);
                        Save(assistentes);
                  }
            }
      }

}
