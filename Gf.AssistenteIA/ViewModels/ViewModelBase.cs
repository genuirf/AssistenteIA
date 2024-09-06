using Gf.AssistenteIA.Utils;

namespace Gf.AssistenteIA.ViewModels
{
      public class ViewModelBase : PropChange
      {
            public string Titulo
            {
                  get => Get<string>();
                  set => Set(value);
            }
      }
}
