using LiteDB;

namespace Shared
{
    public class LiteDbProvider
    {
        private readonly string _connectionPath;
        private bool IsEnsured = false;


        public LiteDbProvider(string connectionPath)
        {
            _connectionPath = connectionPath;
        }

        public LiteDbProvider EnsureIndexes()
        {
            if (IsEnsured)
                return this;

            using (var db = new LiteDatabase(_connectionPath))
            {
                var films = db.GetCollection<Film>();
                films.EnsureIndex(film => film.Title);
                films.EnsureIndex(film => film.Year);
                films.EnsureIndex(film => film.Medias);
            }

            return this;
        }

        public LiteDatabase GetDatabase()
        {
            EnsureIndexes();
            return new LiteDatabase(_connectionPath);
        }
    }
}