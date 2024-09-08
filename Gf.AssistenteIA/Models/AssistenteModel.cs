namespace Gf.AssistenteIA.ViewModels
{
      public class AssistenteModel : ModelBase
      {
            public Guid Id
            {
                  get => Get<Guid>();
                  set => Set(value);
            }
            public string Titulo
            {
                  get => Get<string>();
                  set => Set(value);
            }
            public string Descricao
            {
                  get => Get<string>();
                  set => Set(value);
            }
            public string Api_Url
            {
                  get => Get<string>();
                  set => Set(value);
            }
            public string Api_Token
            {
                  get => Get<string>();
                  set => Set(value);
            }
            public double Temperature
            {
                  get => Get<double>();
                  set => Set(value);
            }
            public string Instrucoes
            {
                  get => Get<string>();
                  set => Set(value);
            }
            public string InstrucoesSemContexto
            {
                  get => Get<string>();
                  set => Set(value);
            }
            public double ContextSimilarity
            {
                  get => Get<double>();
                  set => Set(value);
            }
      }
}
