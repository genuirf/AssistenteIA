using Gf.AssistenteIA.ViewModels;

namespace Gf.AssistenteIA.Services
{
      public interface INavigationService
      {
            event Action<ViewModelBase> OnNavigateTo;
            void NavigateTo(ViewModelBase viewModel);
      }
}
