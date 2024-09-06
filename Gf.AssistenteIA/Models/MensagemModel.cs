namespace Gf.AssistenteIA.ViewModels
{
      public class MensagemModel : ModelBase
      {
            public string MensagemUsuario
            {
                  get => Get<string>();
                  set => Set(value);
            }
            public string MensagemAssistente
            {
                  get => Get<string>();
                  set => Set(value);
            }
      }
}
