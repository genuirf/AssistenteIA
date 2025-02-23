using Gf.AssistenteIA.Utils;

namespace Gf.AssistenteIA.Models
{
    public class FileEmbeddingData : PropChange
    {
        public bool check
        {
            get => Get<bool>();
            set => Set(value);
        }
        public string fileName
        {
            get => Get<string>();
            set => Set(value);
        }
        public string FileContent
        {
            get => Get<string>();
            set => Set(value);
        }
        public float[] Embedding
        {
            get => Get<float[]>();
            set => Set(value);
        }
    }
}
