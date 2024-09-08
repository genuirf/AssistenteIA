namespace Gf.AssistenteIA.Services
{
      public interface IDialogService
      {
            bool Confirm(string message, string title);
            void Error(string message, Exception exception);
      }
}
