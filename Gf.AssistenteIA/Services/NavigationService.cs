using Gf.AssistenteIA.ViewModels;

namespace Gf.AssistenteIA.Services
{
      public class NavigationService : INavigationService
      {
            private readonly MainViewModel _mainViewModel;

            public NavigationService(MainViewModel mainViewModel)
            {
                  _mainViewModel = mainViewModel;
            }

            public void NavigateTo(ViewModelBase viewModel)
            {
                  _mainViewModel.CurrentViewModel = viewModel;
            }
      }
}
