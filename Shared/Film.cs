using System.Collections.Generic;

namespace Shared
{
    public class Film
    {
        public Film()
        {
            Medias = new List<Media>();
        }

        public int Id { get; set; }

        public List<Media> Medias { get; set; }

        public string Title { get; set; }
        public int? Year { get; set; }
    }
}