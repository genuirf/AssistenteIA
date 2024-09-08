namespace Gf.AssistenteIA.Models
{
      public class Body
      {
            public Body()
            {
                  messages = [];
            }
            public string model { get; set; } 
            public bool stream { get; set; } = true;
            public List<BodyMsg> messages { get; set; }
            public double temperature { get; set; } = 0.7;
            public int max_tokens { get; set; } = -1;
      }
}
