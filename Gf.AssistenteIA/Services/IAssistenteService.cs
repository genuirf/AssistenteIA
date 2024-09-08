using Gf.AssistenteIA.ViewModels;

namespace Gf.AssistenteIA.Services
{
      public interface IAssistenteService
      {
            void ConfigurarUrlApi(string apiUrl);
            void ConfigurarToken(string token);
            void StopResponse();
            Task<string> SendQuestionAsync(AssistenteModel assistente, string questao, List<MensagemModel> historico, string? contextEmbedding, Action<string> onStreamMsg, Action onStop);
            Task<float[]> GetEmbeddingAsync(string text, TimeSpan timeout);
      }
}
