using Gf.AssistenteIA.Services;
using Gf.AssistenteIA.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Navigation;

namespace Gf.AssistenteIA.ViewModels
{
      public class MainViewModel : ViewModelBase
      {
            // Construtor sem parâmetros para o modo design
            public MainViewModel()
            {
                  base.Titulo = "GF Chat IA";

                  CurrentViewModel = new AssistentesViewModel();
            }

            private readonly INavigationService _navigationService;
            private readonly IDialogService _dialogService;

            public MainViewModel(INavigationService navigationService, IDialogService dialogService)
            {
                  _navigationService = navigationService;
                  _dialogService = dialogService;
                  base.Titulo = "GF Chat IA";

                  _navigationService.OnNavigateTo += HandleNavigation;

                  CurrentViewModel = new AssistentesViewModel(navigationService, dialogService);

                  this.PropertyChangedOn(nameof(Titulo), [nameof(CurrentViewModel)]);
            }

            private void HandleNavigation(ViewModelBase viewModel)
            {
                  CurrentViewModel = viewModel;
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
