namespace Gf.AssistenteIA.Models
{
      public class ContextoMensagem
      {
            public ContextoMensagem()
            {
                  contextosSimilares = [];
            }
            public List<CosineSimilarityInfo> contextosSimilares {  get; set; }
            public string contexto {  get; set; }
      }
}
