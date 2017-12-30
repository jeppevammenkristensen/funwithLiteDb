using System.Collections.Generic;
using LiteDB;

namespace Shared
{
    public class Storage
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class Film
    {
        public Film()
        {
            Medias = new List<Media>();
            Storages = new List<Storage>();
        }

        public int Id { get; set; }

        public List<Media> Medias { get; set; }

        [BsonRef("Film")]
        public List<Storage> Storages { get; set; }

        public string Title { get; set; }
        public int? Year { get; set; }
    }
}