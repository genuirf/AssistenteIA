using Gf.AssistenteIA.ViewModels;

namespace Gf.AssistenteIA.Services
{
      public class NavigationService : INavigationService
      {
            public event Action<ViewModelBase>? OnNavigateTo;

            public void NavigateTo(ViewModelBase viewModel)
            {
                  OnNavigateTo?.Invoke(viewModel);
            }
      }
}
