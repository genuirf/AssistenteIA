using Microsoft.Extensions.DependencyInjection;
using Gf.AssistenteIA.Repositories;
using Gf.AssistenteIA.Services;
using Gf.AssistenteIA.ViewModels;
using System.Configuration;
using System.Data;
using System.Windows;
using Gf.AssistenteIA.Views;
using System;
using ChatIA.Model.Recursos;

namespace Gf.AssistenteIA
{
      /// <summary>
      /// Interaction logic for App.xaml
      /// </summary>
      public partial class App : Application
      {
            public static IServiceProvider ServiceProvider { get; private set; }

            protected override void OnStartup(StartupEventArgs e)
            {
                  base.OnStartup(e);

                  var serviceCollection = new ServiceCollection();
                  ConfigureServices(serviceCollection);
                  ServiceProvider = serviceCollection.BuildServiceProvider();

                  // Resolver MainWindow e MainViewModel do ServiceProvider
                  var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                  mainWindow.Show();

            }

            private void ConfigureServices(IServiceCollection services)
            {
                  // Registrando o DialogService como Singleton (uma única instância para toda a aplicação)
                  services.AddTransient<IDialogService, DialogService>();

                  // Registrando o NavigationService, se ele não mantiver estado específico, Singleton é adequado
                  services.AddTransient<INavigationService, NavigationService>();

                  // Registrando o repositório de assistentes. Se ele não tiver estado específico por tela/usuário, Singleton é suficiente
                  services.AddTransient<IAssistantRepository, FileAssistantRepository>();

                  // Registrando o repositório de embeddings.
                  services.AddTransient<EmbeddingService>();

                  // Views são melhor registradas como Transient, pois podem ser criadas e destruídas conforme necessário
                  services.AddSingleton<MainWindow>();
                  services.AddTransient<AssistentesView>();
                  services.AddTransient<EditAssistenteView>();
                  services.AddTransient<ChatView>();

                  // ViewModels são geralmente Transient, já que cada tela/controle pode ter seu próprio ViewModel
                  services.AddSingleton<MainViewModel>();
                  services.AddTransient<AssistentesViewModel>();
                  services.AddTransient<EditAssistenteViewModel>();
                  services.AddTransient<ChatViewModel>();

                  // Registroando API
                  services.AddTransient<IAssistenteService, LMStudioService>();

            }
      }

}
