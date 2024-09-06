using Gf.AssistenteIA.Services;
using Gf.AssistenteIA.Utils;

namespace Gf.AssistenteIA.ViewModels
{
      public class MainViewModel : ViewModelBase
      {
            public MainViewModel()
            {
                  base.Titulo = "GF Chat IA";
                  // Criação do serviço de navegação e configuração do ViewModel inicial
                  var navigationService = new NavigationService(this);

                  // Iniciar com a AssistantsViewModel, passando o serviço de navegação
                  CurrentViewModel = new AssistentesViewModel(navigationService);

                  this.PropertyChangedOn(nameof(Titulo), [nameof(CurrentViewModel)]);
            }
            public new string Titulo
            {
                  get
                  {
                        if (CurrentViewModel == null) return base.Titulo;
                        return $"{base.Titulo} | {CurrentViewModel.Titulo}";
                  }
            }
            public ViewModelBase CurrentViewModel
            {
                  get => Get<ViewModelBase>();
                  set => Set(value);
            }
      }
}
