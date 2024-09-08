namespace Gf.AssistenteIA.Models
{
      public class CosineSimilarityInfo
      {
            public CosineSimilarityInfo(double similarity, string doc)
            {
                  this.similarity = similarity;
                  this.doc = doc;
            }
            public double similarity;
            public string doc;
      }
}
