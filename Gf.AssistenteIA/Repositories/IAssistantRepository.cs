using Gf.AssistenteIA.ViewModels;

namespace Gf.AssistenteIA.Repositories
{
      public interface IAssistantRepository
      {
            List<AssistenteModel> GetAll();
            void Save(List<AssistenteModel> assistente);
            AssistenteModel? GetById(Guid id);
            void AddUpdate(AssistenteModel assistente);
            void Delete(Guid id);
      }

}
