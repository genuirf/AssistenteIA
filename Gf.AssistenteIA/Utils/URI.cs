namespace Gf.AssistenteIA.Utils
{
      public static class URI
      {
            public static string Combine(params string[] parts)
            {
                  if (parts == null || parts.Length == 0)
                        throw new ArgumentException("Pelo menos uma parte deve ser fornecida.", nameof(parts));

                  // Inicializa com a primeira parte
                  string combinedUrl = parts[0];

                  for (int i = 1; i < parts.Length; i++)
                  {
                        string part = parts[i];

                        // Remove o caractere de barra final da URL anterior, se existir
                        if (combinedUrl.EndsWith("/"))
                        {
                              combinedUrl = combinedUrl.TrimEnd('/');
                        }

                        // Remove o caractere de barra inicial da URL atual, se existir
                        if (part.StartsWith("/"))
                        {
                              part = part.TrimStart('/');
                        }

                        // Adiciona a nova parte
                        combinedUrl = $"{combinedUrl}/{part}";
                  }

                  return combinedUrl;
            }
      }
}
